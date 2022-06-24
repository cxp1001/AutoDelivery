using AutoDelivery.Core;
using AutoDelivery.Domain;

namespace AutoDelivery.Service.ProductApp
{
    public interface IProductService : IocTag
    {
      
        Task<Product> AddProductAsync(int userId, string name, string maker, string? mainName, string? subName, string? edition, string? version, string? commonName, string sku, string? detail, string? category, bool? hasActiveKey, bool? hasSubActiveKey, bool? hasActiveLink, bool? hasSerialNum, bool IsAvailable = false);
        Task<Product> EditProductAsync(int userId, int id, string? name, string? maker, string? mainName, string? subName, string? edition, string? version, string? commonName, string? sku, string? detail, string? category, int? categoryId, bool? hasActiveKey, bool? hasSubActiveKey, bool? hasActiveLink, bool? hasSerialNum, bool IsAvailable);
        Task<IEnumerable<Product>> GetProductDtoAsync(int userId, string? productCategory, string? productName, string? productSku, string? maker, PageWithSortDto pageWithSortDto);
        Task<Product> DeleteProductAsync(int userId, int id);
    }
}