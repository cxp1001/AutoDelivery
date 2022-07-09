namespace AutoDelivery.Domain
{
    public  class OrderDetail : BaseModel
    {
        // 订单Id
        public long? OrderId { get; set; }
        // 订单号 name
        public string? OrderName { get; set; }
        // 订单生成时间
        public DateTimeOffset? CreatedTime { get; set; }
        // 更新时间  updated_at
        public DateTimeOffset? UpdatedTime { get; set; }
        // 订单中产品总数
        public int? ItemQuantity { get; set; } = 0;
        // 订单信息 <产品名，sku,数量>
        public string? OrderDetails { get; set; }
        // 用户名 first name + last name
        public string CustomerName { get; set; } = null!;
        // 用户邮箱 email
        public string? CustomerMail { get; set; }
        // 分配的序列号
        public List<Serial> RelatedSerials { get; set; }



    }
}
