using AutoDelivery.Core;
using AutoDelivery.Core.Extensions;
using AutoDelivery.Domain;

namespace AutoDelivery.Service.ProductApp
{
    public interface IProductService : IDependency
    {

        Task<Product> AddProductAsync(int userId, string productName, string maker, string? mainName, string? subName, string? productEdition, string? productVersion, string? productCommonName, string productSku, string? detail, string? category, bool? hasActiveKey, bool? hasSubActiveKey, bool? hasActiveLink, bool? hasSerialNum, bool IsAvailable = false);
        Task<int> CountProductsAsync(int userId, string? productCategory, string? productName, string? productSku, string? maker);
        Task<Product> DeleteProductAsync(int userId, int id);
        Task<Product> EditProductAsync(int userId, int id, string? name, string? maker, string? mainName, string? subName, string? edition, string? version, string? commonName, string? sku, string? detail, string? category, int? categoryId, bool? hasActiveKey, bool? hasSubActiveKey, bool? hasActiveLink, bool? hasSerialNum, bool IsAvailable);
        Task<IEnumerable<Product>> GetProductDtoAsync(int userId, string? productCategory, string? productName, string? productSku, string? maker, PageWithSortDto pageWithSortDto);
      }
}