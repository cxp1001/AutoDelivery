using System.Collections;
using AutoDelivery.Core;
using AutoDelivery.Core.Extensions;
using AutoDelivery.Domain;

namespace AutoDelivery.Service.UserApp
{
    public interface IUserService:IDependency
    {
         Task<List<Product>> GetAllProductsAsync(int userId, PageWithSortDto pageWithSortDto);
        Task<List<ProductCategory>> GetProductCategoriesAsync(int userId, PageWithSortDto pageWithSortDto);
        Task<int> CountAllProductsAsync(int userId);
        Task<int> CountCategories(int userId);
    }
}