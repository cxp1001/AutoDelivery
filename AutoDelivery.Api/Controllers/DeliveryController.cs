using AutoDelivery.Api.Extensions;
using AutoDelivery.Service.DeliveryApp;
using AutoDelivery.Service.MailApp;
using AutoDelivery.Service.OrderApp;
using Microsoft.AspNetCore.Mvc;
using ShopifySharp;
using Swashbuckle.AspNetCore.Annotations;

namespace AutoDelivery.Api.Controllers
{

    public class DeliveryController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IDeliveryService _deliveryService;
        private readonly IMailService _mailService;

        public DeliveryController(IOrderService orderService, IDeliveryService deliveryService, IMailService mailService)
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
        [HttpPost("OrderPaid"), SwaggerOperation(Summary = "处理'orderspaid'Webhook")]
        public async Task<IActionResult> HandleOrdersPaidWebhookAsync([FromQuery] string shop)
        {
            // 从HTTP的请求体中获取webhook数据
            var order = await Request.DeserializeBodyAsync<Order>();

            // 将webhook中的订单信息保存到数据库中
            var newOrderDetail = await _orderService.SaveOrdersFromWebhookAsync(order, shop);

            // 根据用户的订单信息从数据库中提取相应的序列号
            var serialList = await _deliveryService.TakeSerialAsync(order);

            var mailContent = "aaa";

            // 拉取用户的邮箱配置
            _mailService.SendActiveEmail("251088569@qq.com", "Active Message", mailContent);

            return Ok();
        }





    }
}