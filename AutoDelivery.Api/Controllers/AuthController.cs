using System.Text.Json;
using AutoDelivery.Core.Core;
using AutoDelivery.Domain.Result;
using AutoDelivery.Domain.Secrets;
using AutoDelivery.Domain.Url;
using AutoDelivery.Domain.User;
using AutoDelivery.Service.AuthApp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopifySharp;
using Swashbuckle.AspNetCore.Annotations;

namespace AutoDelivery.Api.Controllers
{

    public class AuthController : BaseController
    {
        private readonly ISecrets _secrets;
        private readonly IAuthService _authService;
        private readonly AutoDeliveryContext _dbContex;
        private readonly IApplicationUrls _applicationUrls;
        public AuthController(AutoDeliveryContext dbContex, IApplicationUrls applicationUrls, ISecrets secrets, IAuthService authService)
        {
            this._applicationUrls = applicationUrls;
            this._dbContex = dbContex;
            this._secrets = secrets;
            this._authService = authService;
        }


        [HttpGet("Login")]
        [SwaggerOperation(Summary = "接收shop参数并携带该参数跳转到/auth/login页面")]
        public string Login(string shop = null)
        {
            // 接收shop参数并携带该参数跳转到/auth/login页面
            return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "redirect to login",
                    Status = 1,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "/auth/login",
                    Data = $"ShopDomain = {shop}"
                }
            );
        }


        [HttpGet("Logout")]
        public async Task<string> Logout()
        {
            await HttpContext.SignOutAsync();
            return JsonConvert.SerializeObject(
               new ShopifyResult
               {
                   ErrorMessage = "redirect to login",
                   Status = 8,
                   Time = DateTimeOffset.Now,
                   RedirectPath = "/auth/login",
               }, setting
           );
        }

        [HttpPost]
        public async Task<string> HandleLogin(string shop)
        {
            // 验证用户输入的shop是一个真实的Shopify店铺
            if (!await AuthorizationService.IsValidShopDomainAsync(shop))
            {
                // shop错误，跳转到登录页面并提示用户
                return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "redirect to login",
                    Status = 9,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "/auth/login",
                    Data = $"It looks like {shop} is not a valid Shopify shop domain."

                }, setting
           );
            }

            // 生成权限申请列表
            var requiredPermissions = new[] { "read_orders" };

            // 生成OAuth状态记录并保存到数据库中，确保本次登录请求只能被使用一次
            OAuthState newState = await _authService.CreateOAuthStateRecord(shop);


            var oAuthUrl = AuthorizationService.BuildAuthorizationUrl(
                requiredPermissions,
                shop,
                _secrets.ShopifyPublicKey,
                _applicationUrls.OAuthRedirectUrl,
                newState.Token
            );

             return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "redirect to OAuthURL",
                    Status = 10,
                    Time = DateTimeOffset.Now,
                    RedirectPath = oAuthUrl.ToString()
                }, setting
           );

        }

    }
}