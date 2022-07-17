namespace AutoDelivery.Domain
{
    // 产品信息
    public partial class Product : BaseModel
    {

        public string ProductName { get; set; } = null!;
        public string? Maker { get; set; } = null!;
        public string? MainName { get; set; } = null!;
        public string? SubName { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset? EditTime { get; set; }
        public string? ProductEdition { get; set; } = null!;
        public string? ProductVersion { get; set; } = null!;
        public string? ProductCommonName { get; set; } = null!;
        public string? ProductSku { get; set; } = null!;
        public string? ProductDetails { get; set; }
        public bool? HasSerialNum { get; set; }
        public bool? HasActiveLink { get; set; }
        public bool? HasActiveKey { get; set; }
        public bool? HasSubActiveKey { get; set; }
        public bool IsAvailable { get; set; } = false!;
        public List<Serial>? SerialsInventory { get; set; }
        public ProductCategory? ProductCategory { get; set; }
        public MailContent? MailContent { get; set; }
    }
}
