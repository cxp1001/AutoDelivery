using System.Net;
using AutoDelivery.Api.Attributes;
using AutoDelivery.Api.Extensions;
using AutoDelivery.Domain.Result;
using AutoDelivery.Domain.Url;
using AutoDelivery.Service.SubscriptionApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopifySharp;
using Swashbuckle.AspNetCore.Annotations;

namespace AutoDelivery.Api.Controllers
{
    [Authorize]
    public class SubscriptionController : BaseController
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IApplicationUrls _applicationUrls;

        public SubscriptionController(ISubscriptionService subscriptionService, IApplicationUrls applicationUrls)
        {
            this._subscriptionService = subscriptionService;
            this._applicationUrls = applicationUrls;
        }

        [AuthorizeWithActiveSubscription, HttpGet]
        [SwaggerOperation(Summary = "获取用户的订阅信息")]
        public async Task<string> GetSubscriptionInfo()
        {
            var session = HttpContext.User.GetUserSession();
            var user = await _subscriptionService.CheckUserChargeInfoAsync(session);

            // 用户没有付费信息，跳转到订阅开始的流程
            if (!user.ShopifyChargeId.HasValue)
            {
                return JsonConvert.SerializeObject(
                  new ShopifyResult
                  {
                      ErrorMessage = "subscription error",
                      Status = 11,
                      Time = DateTimeOffset.Now,
                      RedirectPath = "/subscription/start",
                  }, setting
                   );
            }

            // 获取用户的订阅详情
            var chargeService = new RecurringChargeService(user.ShopifyShopDomain, user.ShopifyAccessToken);
            RecurringCharge charge;

            try
            {
                charge = await chargeService.GetAsync(user.ShopifyChargeId.Value);
            }
            catch (ShopifyException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
            {
                // 用户的订阅不存在，删除该订阅的id
                await _subscriptionService.DeleteChargeIdAsync(user);

                // 更新用户的session，并将用户跳转到订阅页面
                await HttpContext.SignInAsync(user);
                return JsonConvert.SerializeObject(
                  new ShopifyResult
                  {
                      ErrorMessage = "subscription error",
                      Status = 11,
                      Time = DateTimeOffset.Now,
                      RedirectPath = "/subscription/start",
                  }, setting
                   );
            }

            var chargeDetails = JsonConvert.SerializeObject(charge, setting);


            // 成功获取用户的订阅信息并跳转展示
            return JsonConvert.SerializeObject(
                 new ShopifyResult
                 {
                     ErrorMessage = "pull in  subscription details",
                     Status = 12,
                     Time = DateTimeOffset.Now,
                     RedirectPath = "/subscription/index",
                     Data = chargeDetails
                 }, setting
                  );
        }


        // 用户请求订阅 /start
        [HttpGet("Start")]
        public async Task<string> StartAsync()
        {
            // 确定用户还没有订阅，避免多次付费
            var session = HttpContext.User.GetUserSession();
            var user = await _subscriptionService.CheckUserChargeInfoAsync(session);

            // 用户存在付费信息
            if (user.ShopifyChargeId.HasValue)
            {
                // 用户已订阅，跳转到主页面
                return JsonConvert.SerializeObject(
                  new ShopifyResult
                  {
                      ErrorMessage = "user is already subscribed",
                      Status = 13,
                      Time = DateTimeOffset.Now,
                      RedirectPath = "/home/index",
                  }, setting
                   );
            }
            else
            {
                return JsonConvert.SerializeObject(
                  new ShopifyResult
                  {
                      ErrorMessage = "redirect to subscription page to start charge",
                      Status = 14,
                      Time = DateTimeOffset.Now,
                      RedirectPath = "/subscription/start",
                  }, setting
                   );
            }
        }


        // 用户提交确认订阅的表单后，生成新的订阅付款信息并跳转到Shopify进行订阅
        [HttpPost]
        public async Task<string> HandleStartSubscription()
        {
            // 确定用户还没有订阅，避免多次付费
            var session = HttpContext.User.GetUserSession();
            var user = await _subscriptionService.CheckUserChargeInfoAsync(session);

            // 用户存在付费信息
            if (user.ShopifyChargeId.HasValue)
            {
                // 更新用户的session避免循环跳转
                await HttpContext.SignInAsync(user);
                // 将用户跳转到主页面
                return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "user is already subscribed",
                    Status = 13,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "/home/index",
                }, setting
                 );
            }


            // 生成新的订阅并将用户跳转到订阅url
            var service = new RecurringChargeService(user.ShopifyShopDomain, user.ShopifyAccessToken);
            var charge = await service.CreateAsync(new RecurringCharge
            {
                TrialDays = 7,
                Name = "AutoDelivery Subscription Plan",
                Price = 9.9M,
                ReturnUrl = _applicationUrls.SubscriptionRedirectUrl,
                // If the app is running in development mode, make this a test charge
                //Test = _environment.IsDevelopment()
            }
            );

            // 跳转到Shopify订阅确认页面
            return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "redirect to charge confirmation url",
                    Status = 15,
                    Time = DateTimeOffset.Now,
                    RedirectPath = charge.ConfirmationUrl
                }, setting
           );

        }

        // 获取用户订阅结果，并将成功的订阅信息保存到数据库
        [HttpGet("HandleChargeResult")]
        public async Task<string> HandleChargeResultAsync(long charge_id)
        {
            // 再次确认用户是否已订阅
            var session = HttpContext.User.GetUserSession();
            var user = await _subscriptionService.CheckUserChargeInfoAsync(session);

            // 用户存在付费信息
            if (user.ShopifyChargeId.HasValue)
            {
                // 将用户跳转到主页面
                return JsonConvert.SerializeObject(
                new ShopifyResult
                {
                    ErrorMessage = "user is already subscribed",
                    Status = 13,
                    Time = DateTimeOffset.Now,
                    RedirectPath = "/home/index",
                }, setting
                 );
            }

            // 获取当前订阅的完成情况
            var service = new RecurringChargeService(user.ShopifyShopDomain, user.ShopifyAccessToken);
            var charge = await service.GetAsync(charge_id);


            switch (charge.Status)
            {
                case "pending":
                    // 用户没有接受或取消了支付。跳转到Shopify
                    return JsonConvert.SerializeObject(
                         new ShopifyResult
                         {
                             ErrorMessage = "redirect to charge confirmation url",
                             Status = 15,
                             Time = DateTimeOffset.Now,
                             RedirectPath = charge.ConfirmationUrl
                         }, setting
                        );

                case "expired":
                case "declined":
                    // 重新跳转到开始订阅的页面
                    return JsonConvert.SerializeObject(
                        new ShopifyResult
                        {
                            ErrorMessage = "charge has expired or was declined",
                            Status = 16,
                            Time = DateTimeOffset.Now,
                            RedirectPath = "/subscription/start",
                        }, setting
                        );

                case "active":
                    // 用户完成了支付，更新账号和session
                    user.ShopifyChargeId = charge_id;
                    user.BillingOn = charge.BillingOn;

                    await _subscriptionService.UpdateUserAsync(user);
                    await HttpContext.SignInAsync(user);

                    // 用户可以开始使用app了，跳转到app主页面
                    return JsonConvert.SerializeObject(
                        new ShopifyResult
                        {
                            ErrorMessage = "subscription has been activated",
                            Status = 17,
                            Time = DateTimeOffset.Now,
                            RedirectPath = "/home/index",
                        }, setting
                        );

                default:
                    var message = $"Unhandled charge status of {charge.Status}";
                    throw new ArgumentOutOfRangeException(nameof(charge.Status), message);
            }

        }


    }
}