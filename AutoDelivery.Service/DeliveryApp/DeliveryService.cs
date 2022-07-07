using AutoDelivery.Core.Core;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain;
using Microsoft.EntityFrameworkCore;
using ShopifySharp;

namespace AutoDelivery.Service.DeliveryApp
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IRepository<Serial> _serialRepo;
        private readonly IRepository<OrderDetail> _orderRepo;
        private readonly AutoDeliveryContext _dbContext;

        public DeliveryService(IRepository<Serial> serialRepo, IRepository<OrderDetail> orderRepo, AutoDeliveryContext dbContext)
        {
            this._dbContext = dbContext;
            this._serialRepo = serialRepo;
            this._orderRepo = orderRepo;
        }

        public async Task<List<(string Product, List<Serial>)>> TakeSerialAsync(Order order)
        {

            // 1. 获取订单中的产品信息
            var itemInfos = order.LineItems.Select(l => (Product: l.Title, Quantity: l.Quantity)).ToList();

            var orderId = order.Id;

            var validOrders = itemInfos.Where(o => o.Quantity != 0);
            var currentOrder = await  _orderRepo.GetQueryable().Include(o => o.RelatedSerials).SingleAsync(o => o.OrderId == orderId);

            List<(string Product, List<Serial>)> serialList = new();


            foreach (var validOrder in validOrders)
            {
                var serials = _serialRepo.GetQueryable().Where(s => s.ProductName == validOrder.Product && s.Used == false).OrderBy(s => s.Id).Take((int)validOrder.Quantity);

                foreach (var serial in serials)
                {
                    serial.Used = true;
                    serial.ShippedTime = DateTimeOffset.Now;
                }

               
                currentOrder.RelatedSerials.AddRange(serials);

                serialList.Add((validOrder.Product, serials.ToList()));
                await _dbContext.SaveChangesAsync();
            }

            // 将序列号信息添加到订单中


            return serialList;


        }



    }
}