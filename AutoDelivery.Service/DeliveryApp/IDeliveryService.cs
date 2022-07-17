using AutoDelivery.Core.Extensions;
using AutoDelivery.Domain;
using ShopifySharp;

namespace AutoDelivery.Service.DeliveryApp
{
    public interface IDeliveryService : IDependency
    {
        Task<List<(string Product, List<Serial>)>> TakeSerialAsync(Order order);
    }
}