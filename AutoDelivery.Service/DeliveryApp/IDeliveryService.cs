using AutoDelivery.Domain;
using ShopifySharp;

namespace AutoDelivery.Service.DeliveryApp
{
    public interface IDeliveryService:IocTag
    {
        Task<List<(string Product, List<Serial>)>> TakeSerialAsync(Order order);
    }
}