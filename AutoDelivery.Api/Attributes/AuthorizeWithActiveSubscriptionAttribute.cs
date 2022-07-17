using AutoDelivery.Api.Extensions;
using AutoDelivery.Domain.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoDelivery.Api.Attributes
{
    public class AuthorizeWithActiveSubscriptionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 用户是否已完成鉴权
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            // 获取session并检查用户是否已订阅服务
            var session = context.HttpContext.User.GetUserSession();
            if (!session.IsSubscribed)
            {
                // 将用户跳转到/subscription/start ，开始订阅流程
                var res = new ShopifyResult()
                {
                    RedirectPath = "/subscription/start",
                };
                context.Result = new JsonResult(res) { StatusCode = 403 };
            }
        }
    }
}