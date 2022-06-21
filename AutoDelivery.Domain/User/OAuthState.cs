using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoDelivery.Domain;

namespace ShopifyWebApi.Domain.User
{
    public class OAuthState:BaseModel
    {
         public DateTimeOffset DateCreated { get; set; }
        public string Token { get; set; }
         public string ShopifyShopDomain { get; set; }
    }
}