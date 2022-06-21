using AutoDelivery.Domain;
using Microsoft.EntityFrameworkCore;

namespace AutoDelivery.Service.ProductApp
{
    public partial class ProductService:IProductService
    {

           public async Task<IEnumerable<string>> GetProductCategoriesAsync()
        {
            var categories = await _categoryRepo.GetQueryable().Select(p => p.Category).ToListAsync();
            return categories;
        }
        
    }
}