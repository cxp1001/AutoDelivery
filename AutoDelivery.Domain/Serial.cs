using System;
using System.Collections.Generic;

namespace AutoDelivery.Domain
{
    public partial class Serial:BaseModel
    {
        
        // 商品名
        public string ProductName { get; set; } = null!;
        // 商品のSKU
        public string ProductSku { get; set; } = null!;
        // 該当商品のシリアルキー
        public string? SerialNumber { get; set; }
        // アクティブキー
        public string? ActiveKey { get; set; }
        // サブアクティブキー
        public string? SubActiveKey { get; set; }
        // アクティブリンク
        public string? ActiveLink { get; set; }
        // 追加された時間
        public DateTimeOffset CreatedTime { get; set; }
        // お客様に発送された時間
        public DateTimeOffset? ShippedTime { get; set; }
        // 使い済み
        public bool Used { get; set; } = false;
        public byte[] RowVersion { get; set; }
        
        

        // 関連注文
       // public OrderDetail RelatedOrder { get; set; } = null;
    }
}
