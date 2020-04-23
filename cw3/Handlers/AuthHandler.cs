using cw3.DTOs.Requests;
using cw3.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace cw3.Handlers
{
    public class AuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        SqlServerStudentDbService _dbService;
        public AuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            ) : base(options, logger, encoder, clock)
        {
            _dbService = new SqlServerStudentDbService();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing auth header");

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credsBytes = Convert.FromBase64String(authHeader.Parameter);
            var creds = Encoding.UTF8.GetString(credsBytes).Split(":");
            if (creds.Length != 2)
                return AuthenticateResult.Fail("Incorrect authorization header value");

            if (!_dbService.IsLoginCorrect(new LoginRequest { Login = creds[0], Password = creds[1] }))
                return AuthenticateResult.Fail("Bad login");
            var stud = _dbService.GetStudent(creds[0]);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, stud.Index),
                new Claim(ClaimTypes.Name, stud.FirstName),
                new Claim(ClaimTypes.Role, "employee")
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
