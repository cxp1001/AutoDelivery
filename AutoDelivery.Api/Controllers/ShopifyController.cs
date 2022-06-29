using AutoDelivery.Api.Attributes;
using AutoDelivery.Core.Core;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain.Secrets;
using AutoDelivery.Domain.Url;
using AutoDelivery.Service.ShopifyApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopifySharp;
using AutoDelivery.Domain.User;
using AutoDelivery.Api.Extensions;
using Swashbuckle.AspNetCore.Annotations;
using AutoDelivery.Domain.Session;
using Newtonsoft.Json;
using AutoDelivery.Domain.Result;

namespace AutoDelivery.Api.Controllers
{
/*
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

        [HttpGet, ValidateShopifyRequest]
        [SwaggerOperation(Summary = "App的入口路径，进行握手。验证是否为Shopify官方请求，验证用户的登录和订阅状态")]
        public async Task<string> HandleHandShakeAsync(string shop)
        {

            Session session = HttpContext.User.GetUserSession();
            var res = await _shopifyService.HandShakeAsync(shop, session, HttpContext);

            if (res == 1)
            {
                return JsonConvert.SerializeObject(
                      new ShopifyResult
                      {
                          ErrorMessage = "Request is missing shop querystring parameter.",
                          Status = 2,
                          Time = DateTimeOffset.Now,
                          RedirectPath = "handshake/error",
                      }

              );
                //Problem("Request is missing shop querystring parameter.", statusCode: 422);
                //    httpContext.Response.Redirect("handshake/error?status=422");
            }
            if (res == 2)
            {
                return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "ok",
                    Status = 1,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "/dashboard/index",
                }
            );

            }
            if (res == 3)
            {
                return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "authentication error",
                    Status = 3,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "/auth/login",
                }
                 );
            }
            else
            {
                return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "error",
                    Status = 4,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "error",
                }
                 );
            }

        }


        [HttpGet, ValidateShopifyRequest]
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
                 );
            }

            // 使用access token获取用户的商店信息
            var shopService = new ShopService(shop, accessToken);
            var shopData = await shopService.GetAsync();

            var user = await _shopifyService.UserUpdate(shop, shopData, accessToken);

            await HttpContext.SignInAsync(user);

            //todo
            return "aaa";
        }
    }


*/
}
