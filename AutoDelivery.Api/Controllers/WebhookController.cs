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
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain.User;
using AutoDelivery.Service.OrderApp;

namespace AutoDelivery.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public WebhookController(IOrderService orderService)
        {
            this._orderService = orderService;

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

            // 将webhook中的订单信息保存到数据库中
            var newOrderDetail = await _orderService.SaveOrdersFromWebhookAsync(order);

            // 更新用户的订单信息
            await _orderService.UpdateOrdersOfUser(shop, newOrderDetail);

            // 根据用户的订单信息从数据库中提取相应的序列号

            // 1. 获取订单中的产品信息
            var orderDetails = order.LineItems.Select(l => (Product: l.Title, Quantity: l.Quantity)).ToList();
           
            // 2. 根据产品信息从数据库中提取相对应的序列号等激活信息
          
       

            // 拉取用户的邮箱配置
           // MailConfig mailConfig = user.Mailconfiguration;

            return Ok();
        }
    }
}