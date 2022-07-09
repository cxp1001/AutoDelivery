namespace AutoDelivery.Core
{
    public class PageWithSortDto
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string Sort { get; set; } = "CreatedTime";
        public OrderType OrderType { get; set; } = OrderType.Asc;


    }

    public enum OrderType
    {
        Asc ,
        Desc
    }
}