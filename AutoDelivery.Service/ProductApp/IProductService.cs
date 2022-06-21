using AutoDelivery.Core;
using AutoDelivery.Domain;

namespace AutoDelivery.Service.ProductApp
{
    public interface IProductService : IocTag
    {
        Task<Product> AddProductAsync(string name, string maker, string? mainName, string? subName, string? edition, string? version, string? commonName, string sku, string? detail, string? category, bool? hasActiveKey, bool? hasSubActiveKey, bool? hasActiveLink, bool? hasSerialNum, bool IsAvailable = false);
        Task<Product> EditProductAsync(int id, string? name);
        // Task<Product> EditProductAsync(int id, string? name, string? maker, string? mainName, string? subName, string? edition, string? version, string? commonName, string? sku, string? detail, string? category, bool? hasActiveKey, bool? hasSubActiveKey, bool? hasActiveLink, bool? hasSerialNum, bool IsAvailable);
        Task<IEnumerable<string>> GetProductCategoriesAsync();
        Task<IEnumerable<Product>> GetProductDtoAsync(string? productCategory, string? productName, string? productSku, string? maker, PageWithSortDto pageWithSortDto);

    }
}