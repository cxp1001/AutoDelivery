namespace AutoDelivery.Service.AutoMapper
{
    public class SerialDto
    {
        public int SerialId { get; set; }
        public string ProductName { get; set; } = null!;
        // 商品SKU
        public string ProductSku { get; set; } = null!;
        // 商品序列号
        public string? SerialNumber { get; set; }
        // 商品激活码
        public string? ActiveKey { get; set; }
        // 商品的第二个激活码
        public string? SubActiveKey { get; set; }
        // 商品的激活链接
        public string? ActiveLink { get; set; }
        // 序列号生成日期
        public DateTimeOffset CreatedTime { get; set; }
        // 序列号交付日期
        public DateTimeOffset? ShippedTime { get; set; }
        // 已使用
        public bool Used { get; set; } = false;
    }
}