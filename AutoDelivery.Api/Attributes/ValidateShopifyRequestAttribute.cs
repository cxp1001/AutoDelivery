using AutoDelivery.Domain.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopifySharp;

namespace AutoDelivery.Api.Attributes
{
    public class ValidateShopifyRequestAttribute:ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var secrets = context.HttpContext.RequestServices.GetService(typeof(ISecrets)) as ISecrets;
            var querystring = context.HttpContext.Request.Query;
            var isAuthentic = AuthorizationService.IsAuthenticRequest(querystring, secrets.ShopifySecretKey);
            if (isAuthentic)
            {
                await next();
            }
            else
            {
                context.Result = new ForbidResult();
            }

        }
    }
}