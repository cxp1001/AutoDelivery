using AutoDelivery.Api.Attributes;
using AutoDelivery.Api.Extensions;
using AutoDelivery.Domain.Result;
using AutoDelivery.Domain.Secrets;
using AutoDelivery.Domain.Session;
using AutoDelivery.Domain.Url;
using AutoDelivery.Service.ShopifyApp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopifySharp;
using ShopifySharp.Filters;
using Swashbuckle.AspNetCore.Annotations;

namespace AutoDelivery.Api.Controllers
{

    public class ShopifyController : BaseController
    {
        private readonly IShopifyService _shopifyService;
        private readonly ISecrets _secrets;
        private readonly IApplicationUrls _applicationUrls;

        public ShopifyController(IShopifyService shopifyService, ISecrets secrets, IApplicationUrls applicationUrls)
        {
            this._shopifyService = shopifyService;
            this._secrets = secrets;
            this._applicationUrls = applicationUrls;
        }

        [HttpGet("HandShake"), ValidateShopifyRequest]
        [SwaggerOperation(Summary = "App的入口路径，进行握手。验证是否为Shopify官方请求，验证用户的登录和订阅状态")]
        public async Task<string> HandShakeAsync(string shop)
        {

            Session session = HttpContext.User.GetUserSession();

            if (string.IsNullOrWhiteSpace(shop))
            {
                return JsonConvert.SerializeObject(
                      new ShopifyResult
                      {
                          ErrorMessage = "Request is missing shop querystring parameter.",
                          Status = 2,
                          Time = DateTimeOffset.Now,
                          RedirectPath = "handshake/error",
                      }, setting

              );
                //Problem("Request is missing shop querystring parameter.", statusCode: 422);
                //    httpContext.Response.Redirect("handshake/error?status=422");
            }

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var sameShop = await _shopifyService.CheckShopAsync(session, shop);

                // 用户已登录，且输入的商店和当前登录的商店一致，可以跳转到主界面
                if (sameShop)
                {
                    return JsonConvert.SerializeObject(
                    new ShopifyResult
                    {
                        ErrorMessage = "ok",
                        Status = 1,
                        Time = DateTimeOffset.Now,
                        RedirectPath = "/dashboard/index",
                    }, setting
                );
                }
                // 商店域名不匹配，用户可能拥有多个商店且正在尝试登录另一个商店。
                // 注销用户的登录并清除cookie，将用户跳转到登录/安装页面
                await HttpContext.SignOutAsync();

            }

            // 用户未安装应用，或未登录，或是登录到了另外的商店，将用户跳转到登录/安装页面
            return JsonConvert.SerializeObject(
                   new ShopifyResult
                   {
                       ErrorMessage = "authentication error",
                       Status = 3,
                       Time = DateTimeOffset.Now,
                       RedirectPath = "/auth/login",
                   }, setting
                    );

        }




        [HttpGet("AuthResult"), ValidateShopifyRequest]
        public async Task<string> AuthResultAsync(string shop, string code, string state)
        {


            var dbToken = await _shopifyService.CheckTokenAsync(state);

            if (dbToken == null)
            {
                // token已被使用或不存在，用户必须重新开始oauth认证过程
                return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "error",
                    Status = 4,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "/auth/login",
                    Data = $"ShopDomain = {shop}"
                }
                 );
            }


            // 删除token防止被再次使用
            await _shopifyService.DeleteTokenAsync(dbToken);


            // 使用获取到的code交换得到商店的access token
            string accessToken;
            try
            {
                accessToken = await AuthorizationService.Authorize(code, shop, _secrets.ShopifyPublicKey, _secrets.ShopifySecretKey);
            }
            catch (ShopifyException ex) when ((int)ex.HttpStatusCode == 400)
            {
                // code已被使用或已过期。用户必须重新进行oauth认证
                return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "error",
                    Status = 4,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "/auth/login",
                    Data = $"ShopDomain = {shop}"
                }
                , setting);
            }

            // 使用access token获取用户的商店信息
            var shopService = new ShopService(shop, accessToken);
            var shopData = await shopService.GetAsync();

            var user = await _shopifyService.UserUpdate(shop, shopData, accessToken);

            // 登录用户
            await HttpContext.SignInAsync(user);

            //检查app卸载的webhook是否已注册
            var service = new WebhookService(shop, accessToken);
            var topic = "app/uninstalled";
            var existingHooks = await service.ListAsync(new WebhookFilter
            {
                Topic = topic
            });

            if (!existingHooks.Items.Any())
            {
                // webhook不存在，注册新的webhook
                await service.CreateAsync(
                    new Webhook()
                    {
                        Topic = topic,
                        Address = _applicationUrls.AppUninstalledWebhookUrl,
                    }
                );
            }

            // 检查用户是否需要激活订阅
            if (!user.ShopifyChargeId.HasValue)
            {
                return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "subscription error",
                    Status = 6,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "/subscription/start",
                    Data = $"ShopDomain = {shop}"
                }
                , setting);
            }

            // 用户已订阅，跳转到主页面
            return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "ok",
                    Status = 7,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "/home/index",
                    Data = $"ShopDomain = {shop}"
                }
                , setting);
        }
    }
}




