using System.Collections;
using AutoDelivery.Core;
using AutoDelivery.Domain;

namespace AutoDelivery.Service.UserApp
{
    public interface IUserService:IocTag
    {
         Task<List<Product>> GetAllProductsAsync(int userId, PageWithSortDto pageWithSortDto);
        Task<IEnumerable> GetProductCategoriesAsync(int userId, PageWithSortDto pageWithSortDto);
    }
}