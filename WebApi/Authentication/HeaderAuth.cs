using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;
using WebApi.Service;

namespace WebApi.Authentication
{
    public class HeaderAuth : IAuthenticationHandler {
        
        public AuthenticationScheme Scheme { get; private set; }
        
        public HttpContext CurrentContext { get; private set; }
        public Task<AuthenticateResult> AuthenticateAsync() {
            var token = CurrentContext.Request.Headers[GuidToken.GUID_TOKEN_KEY].ToString();

            var (isValid, tokenEntity) = GuidToken.Valid(token);

            if (!isValid || tokenEntity == null) {
                return Task.FromResult(AuthenticateResult.Fail("未登录或授权已过期。"));
            }
            // 生成 AuthenticationTicket
            AuthenticationTicket ticket = new AuthenticationTicket(tokenEntity.ToClaimsPrincipal(), Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        public Task ChallengeAsync(AuthenticationProperties properties) {
            CurrentContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties) {
            CurrentContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) {
            Scheme = scheme;
            CurrentContext = context;
            return Task.CompletedTask;
        }
    }
}
