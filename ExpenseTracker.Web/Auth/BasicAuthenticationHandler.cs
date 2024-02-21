using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using expense_tracker.web.Data;
using expense_tracker.web.Data.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace expense_tracker.web.Auth;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly UserManager<CustomUserEntity> _userManager;

    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ApplicationDbContext applicationDbContext,
        UserManager<CustomUserEntity> userManager) : base(options, logger, encoder)
    {
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("No Authorization Field");
        }

        string authorizationHeader = Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return AuthenticateResult.Fail("Empty Authorization Field");
        }

        if (!authorizationHeader.StartsWith("basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("No \"basic\" keyword");
        }

        var token = authorizationHeader.Substring(6);
        var credentialAsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));

        var credentials = credentialAsString.Split(":");
        if (credentials.Length != 2)
        {
            return AuthenticateResult.Fail("No user:password in the field");
        }

        var username = credentials[0].ToUpper();
        var password = credentials[1];

        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => username.Equals(u.NormalizedUserName));
        if (user == null)
        {
            return AuthenticateResult.Fail("No such user");
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return AuthenticateResult.Fail("Wrong password");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        var identity = new ClaimsIdentity(claims, "Basic");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }
}