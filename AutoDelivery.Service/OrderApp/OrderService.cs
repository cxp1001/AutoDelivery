using AutoDelivery.Core.Repository;
using AutoDelivery.Domain;
using AutoDelivery.Domain.User;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopifySharp;

namespace AutoDelivery.Service.OrderApp
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<OrderDetail> _orderRepo;
        private readonly IRepository<UserAccount> _userRepo;
        public OrderService(IRepository<OrderDetail> orderRepo, IRepository<UserAccount> userRepo)
        {
            this._orderRepo = orderRepo;
            this._userRepo = userRepo;

        }


        public async Task<OrderDetail> SaveOrdersFromWebhookAsync(Order order)
        {
            // 将body中的信息拉取出来
            var itemQuantities = order.LineItems.Where(l => l.Quantity != null).Select(l => l.Quantity).Sum();
            long? orderId = order.Id;
            var orderName = order.Name;
            DateTimeOffset? createdTime = order.CreatedAt;
            DateTimeOffset? updatedTime = order.UpdatedAt;
            var userName = order.Customer.FirstName + " " + order.Customer.LastName;
            var email = order.Customer.Email;
            var orderDetails = order.LineItems.Select(l => (Product: l.Title, Quantity: l.Quantity)).ToList();
            // var orderDetails = order.LineItems.Select(l => (product: l.Title, sku: l.SKU, quantity: l.Quantity)).ToLookup(o => o.product,o=>o.sku,o =>o.quantity).ToDictionary(l=>l.Key,l=>l.First());;
            var orderDetailsJson = JsonConvert.SerializeObject(orderDetails);

            OrderDetail newOrderDetail = new()
            {
                OrderId = orderId,
                OrderName = orderName,
                CreatedTime = createdTime,
                UpdatedTime = updatedTime,
                ItemQuantity = itemQuantities,
                OrderDetails = orderDetailsJson,
                CustomerName = userName,
                CustomerMail = email,
            };
            // 将order信息保存到数据库中
            return await _orderRepo.InsertAsync(newOrderDetail);
        }


        public async Task<UserAccount> UpdateOrdersOfUser(string shop,OrderDetail order)
        {
            var currentUser = await _userRepo.GetQueryable().SingleAsync(u => u.ShopifyShopDomain == shop);
            currentUser.OrderDetails.Add(order);
            return await _userRepo.UpdateAsync(currentUser);
        }


        public async Task TaskSerial()
        {
            
        }



    }
}