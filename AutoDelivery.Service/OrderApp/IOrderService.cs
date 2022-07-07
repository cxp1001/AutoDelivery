using AutoDelivery.Core.Extensions;
using AutoDelivery.Domain;
using AutoDelivery.Domain.User;
using ShopifySharp;

namespace AutoDelivery.Service.OrderApp
{
    public interface IOrderService : IDependency
    {
        Task<OrderDetail> SaveOrdersFromWebhookAsync(Order order, string shop);
    }
}