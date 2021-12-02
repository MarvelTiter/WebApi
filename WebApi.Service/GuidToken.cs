using System;
using System.Collections.Generic;
using System.Security.Claims;
using WebApi.Common.ExtensionHelper;

namespace WebApi.Service
{
    public class GuidToken
    {
        public const string GUID_TOKEN_NAME = "MtGuidTokenAuthentication";
        public const string DEFAULT_AUTHENTICATION_TYPE = "local";
        public const int TOKEN_LENGTH = 32;
        public const string GUEST = "GUEST";
        public const string DEFAULT_ROLE = "USER";
        public const string DEFAULT_OPENID = "DEFAULT_OPENID";
        public const string GUID_TOKEN_KEY = "Token";
        private static int expireDuration = 0;
        public string OpenId { get; set; }
        public string Role { get; set; }
        public DateTime Expire { get; set; }

        //public User User { get; set; }

        private static readonly Dictionary<string, GuidToken> tokenCache = new Dictionary<string, GuidToken>();

        public static (bool, GuidToken) Valid(string token)
        {
            if (string.IsNullOrEmpty(token) || token.Length != TOKEN_LENGTH)
            {
                return (false, null);
            }

            // 从 Session 中获取令牌实体
            GuidToken tokenEntity = GetTokenCache(token);

            if (tokenEntity == null)
            {
                return (false, null);
            }
            else
            {
                tokenEntity.Expire = DateTime.Now.AddMinutes(expireDuration);
            }

            return (true, tokenEntity);

        }

        public static GuidToken GetTokenCache(string token)
        {
            if (!token.IsLegal())
            {
                return null;
            }
            if (tokenCache.TryGetValue(token, out var val))
            {
                if (val.Expire > DateTime.Now) return val;
                else tokenCache.Remove(token);
            }
            return null;
        }

        public static string Create(object user, string openId = DEFAULT_OPENID, string role = DEFAULT_ROLE, int minutes = 30)
        {
            var token = Guid.NewGuid().ToString("N");
            expireDuration = minutes;
            var entity = new GuidToken
            {
                //User = user,
                OpenId = openId,
                Role = role,
                Expire = DateTime.Now.AddMinutes(expireDuration)
            };
            tokenCache.Add(token, entity);
            return token;
        }

        /// <summary>
        /// 令牌实体 转 ClaimsPrincipal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal ToClaimsPrincipal()
        {
            var claimsIdentity = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Name, OpenId),
                new Claim(ClaimTypes.Role, Role),
            }, DEFAULT_AUTHENTICATION_TYPE);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }
    }
}
