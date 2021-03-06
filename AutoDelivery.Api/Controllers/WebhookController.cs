using AutoDelivery.Core.Core;
using AutoDelivery.Domain;
using AutoDelivery.Domain.Mail;
using AutoDelivery.Service.MailApp;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopifySharp;
using AutoDelivery.Api.Extensions;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain.User;
using AutoDelivery.Service.OrderApp;
using AutoDelivery.Service.DeliveryApp;
using Swashbuckle.AspNetCore.Annotations;

namespace AutoDelivery.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IDeliveryService _deliveryService;
        private readonly IMailService _mailService;

        public WebhookController(IOrderService orderService, IDeliveryService deliveryService, IMailService mailService)
        {
            this._mailService = mailService;
            this._orderService = orderService;
            this._deliveryService = deliveryService;
        }


        /// <summary>
        /// 处理 orders_paid Webhook
        /// </summary>
        /// <param name="shop"></param>
        /// <returns></returns>
        [HttpPost("OrderPaid"),SwaggerOperation(Summary ="处理'orderspaid'Webhook")]
        public async Task<IActionResult> HandleOrdersPaidWebhookAsync([FromQuery] string shop)
        {
            // 从HTTP的请求体中获取webhook数据
            var order = await Request.DeserializeBodyAsync<Order>();

            // 将webhook中的订单信息保存到数据库中
            var newOrderDetail = await _orderService.SaveOrdersFromWebhookAsync(order, shop);

            // 根据用户的订单信息从数据库中提取相应的序列号
            var serialList = await _deliveryService.TakeSerialAsync(order);

            string mailContent = serialList.ToString();
            // 拉取用户的邮箱配置
            _mailService.SendActiveEmail("251088569@qq.com","Active Message",mailContent);

            return Ok();
        }
    }
}