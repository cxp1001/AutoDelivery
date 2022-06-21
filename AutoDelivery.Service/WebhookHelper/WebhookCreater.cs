using ShopifySharp;

namespace AutoDelivery.Service.WebhookHelper
{
    public class WebhookCreater
    {
        public async Task<Webhook> CreateOrdersPaidWebhookAsync(string myShopifyUrl,string shopAccessToken)
        {
            var webhookService = new WebhookService(myShopifyUrl, shopAccessToken);
            var topic = "orders/paid";
            Webhook ordersPaidWebhook = new Webhook()
            {
                Address = "https://pitaya.xyz/ad/webhook/orders/paid",
                CreatedAt = DateTimeOffset.Now,
                Format = "json",
                Topic = topic
            };

            var  hook = await webhookService.CreateAsync(ordersPaidWebhook);
            return hook;
        }
    }
}