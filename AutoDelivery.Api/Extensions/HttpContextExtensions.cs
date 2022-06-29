using System.Security.Claims;
using AutoDelivery.Domain.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using AutoDelivery.Domain.User;

namespace AutoDelivery.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static async Task SignInAsync(this HttpContext httpContext, Session session)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId",session.UserId.ToString(),ClaimValueTypes.Integer32),
                new Claim("IsSubscribed",session.IsSubscribed.ToString(),ClaimValueTypes.Boolean)
            };

            var authScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            var identity = new ClaimsIdentity(claims, authScheme);
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(principal);
        }

        public static async Task SignInAsync(this HttpContext httpContext, UserAccount userAccount)
        {
            await httpContext.SignInAsync(new Session(userAccount));
        }

        /// <summary>
        /// 获取用户的Id
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static int GetCurrentUserId(this HttpContext httpContext)
        {


            try
            {
                int userId = httpContext.User.GetUserSession().UserId;
                return userId;
            }
            catch (Exception)
            {
                throw new Exception("id error");
            }

        }


        public static Session GetUserSession(this ClaimsPrincipal userPrincipal)
        {
            if (!userPrincipal.Identity.IsAuthenticated)
            {
                throw new Exception("User is not authenticated, can't get user session");
            }

            T Find<T>(string sessionPropertyName, Func<string, T> valueConverter)
            {
                var claim = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == sessionPropertyName);
                if (claim == null)
                {
                    throw new NullReferenceException($"Session claim {sessionPropertyName} was not found.");
                }
                return valueConverter(claim.Value);
            }

            var session = new Session
            {
                UserId = Find("UserId", int.Parse),
                IsSubscribed = Find("IsSubscribed", bool.Parse)
            };

            return session;
        }
    }
}