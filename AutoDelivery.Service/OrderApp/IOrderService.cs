using AutoDelivery.Domain;
using AutoDelivery.Domain.User;
using ShopifySharp;

namespace AutoDelivery.Service.OrderApp
{
    public interface IOrderService : IocTag
    {
        Task<OrderDetail> SaveOrdersFromWebhookAsync(Order order, string shop);
    }
}