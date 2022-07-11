namespace AutoDelivery.Domain
{
    public class SerialsInfoList
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public List<Serial>? SerialInfo { get; set; }
        public int SerialsCount => SerialInfo.Count();



    }
}