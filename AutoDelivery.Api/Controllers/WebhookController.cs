using AutoDelivery.Core.Core;
using AutoDelivery.Domain;
using AutoDelivery.Domain.Mail;
using AutoDelivery.Service.MailHelper;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopifySharp;
using AutoDelivery.Api.Extensions;

namespace AutoDelivery.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly AutoDeliveryContext _dbContext;

        public WebhookController(AutoDeliveryContext dbContext)
        {
            this._dbContext = dbContext;

        }


        /// <summary>
        /// 处理 orders_paid Webhook
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OrdersPaidAsync([FromQuery] string shop)
        {
            // 从HTTP的请求体中获取webhook数据
            var order = await Request.DeserializeBodyAsync<Order>();

            // 将body中的信息拉取出来
            var itemQuantities = order.LineItems.Where(l => l.Quantity != null).Select(l => l.Quantity).Sum();
            long? orderId = order.Id;
            var orderName = order.Name;
            DateTimeOffset? createdTime = order.CreatedAt;
            DateTimeOffset? updatedTime = order.UpdatedAt;
            var userName = order.Customer.FirstName + " " + order.Customer.LastName;
            var email = order.Customer.Email;
            var orderDetails = order.LineItems.Select(l => (product: l.Title, quantity: l.Quantity)).ToList();
            // var orderDetails = order.LineItems.Select(l => (product: l.Title, sku: l.SKU, quantity: l.Quantity)).ToLookup(o => o.product,o=>o.sku,o =>o.quantity).ToDictionary(l=>l.Key,l=>l.First());;
            var orderDetailsJson = JsonConvert.SerializeObject(orderDetails);

            // 拉取用户
            var user = await _dbContext.Users.SingleAsync(u => u.ShopifyShopDomain == shop);
            var userAccountId = user.Id;

            // 存储webhook对象
            OrderDetail newOrder = new()
            {
                OrderId = orderId,
                OrderName = orderName,
                CreatedTime = createdTime,
                UpdatedTime = updatedTime,
                ItemQuantity = itemQuantities,
                OrderDetails = orderDetailsJson,
                CustomerName = userName,
                CustomerMail = email,
                //UserAccountId = userAccountId
            };

            _dbContext.Add<OrderDetail>(newOrder);
            await _dbContext.SaveChangesAsync();



            // 根据用户的订单信息从数据库中提取相应的序列号

            // 1. 获取订单中的产品信息
            List<string> productNames = order.LineItems.Select(l => l.Title).ToList();

            // 2. 根据产品信息从数据库中提取相对应的序列号等激活信息
            List<Serial> serials = new();
            //MailService.SendActiveEmail();

            // 拉取用户的邮箱配置
            MailConfig mailConfig = user.Mailconfiguration;

            return Ok();
        }
    }
}