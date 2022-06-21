using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoDelivery.Domain;
using AutoDelivery.Domain.Mail;

namespace ShopifyWebApi.Domain.User
{
    public class UserAccount : BaseModel
    {
        public long ShopifyShopId { get; set; }
        public string ShopifyShopDomain { get; set; }
        public string ShopifyAccessToken { get; set; }
        public long? ShopifyChargeId { get; set; }
        /// <summary>
        /// The date that the customer will next be billed on.
        /// </summary>
        public DateTimeOffset? BillingOn { get; set; }
        // 商户的订单列表
        public List<OrderDetail> OrderDetails { get; set; }
        // 商户的邮件配置
        public MailConfig? Mailconfiguration { get; set; }
        // 商户的产品信息
        public List<Product> Products { get; set; }
        


    }
}