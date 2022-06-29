using AutoDelivery.Domain.Secrets;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using ShopifySharp;
using AutoDelivery.Api.Extensions;
using System.Text;

namespace AutoDelivery.Api.Attributes
{
    public class ValidateShopifyWebhookAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var rawBody = await context.HttpContext.Request.ReadRawBodyAsync();
            var secrets = context.HttpContext.RequestServices.GetService<ISecrets>() as ISecrets;

            var isAuthentic = AuthorizationService.IsAuthenticWebhook(context.HttpContext.Request.Headers, rawBody, secrets.ShopifySecretKey);

            if (isAuthentic)
            {
                await next();
            }
            else
            {
                context.HttpContext.Response.ContentType = "application/json";
                var body = JsonConvert.SerializeObject(
                    new
                    {
                        message = "Webhook did not pass validation.",
                        ok = false
                    }
                    );

                using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(body)))
                {
                    context.HttpContext.Response.StatusCode = 401;
                    await buffer.CopyToAsync(context.HttpContext.Response.Body);
                }
            }
        }
    }
}