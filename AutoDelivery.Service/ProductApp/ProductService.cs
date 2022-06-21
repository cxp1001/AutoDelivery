using AutoDelivery.Core;
using AutoDelivery.Core.Core;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain;
using AutoDelivery.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AutoDelivery.Service.ProductApp
{
    public partial class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductCategory> _categoryRepo;
        private readonly AutoDeliveryContext _dbContext;
        public ProductService(IRepository<Product> productRepo, IRepository<ProductCategory> categoryRepo, AutoDeliveryContext dbContext)
        {
            this._productRepo = productRepo;
            this._categoryRepo = categoryRepo;
            this._dbContext = dbContext;
        }


        public async Task<IEnumerable<Product>> GetProductDtoAsync(
            string? productCategory,
            string? productName,
            string? productSku,
            string? maker,
            PageWithSortDto pageWithSortDto
        )
        {
            pageWithSortDto.Sort ??= "ProductName";
            var skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;

            var products = _productRepo.GetQueryable().Where(p =>
                (p.ProductCategory.Category == productCategory || string.IsNullOrWhiteSpace(productCategory)) &&
                ((!string.IsNullOrWhiteSpace(productName) && !string.IsNullOrWhiteSpace(p.ProductName) && p.ProductName.ToLower().Contains(productName.ToLower())) || string.IsNullOrWhiteSpace(productName)) &&
                ((!string.IsNullOrWhiteSpace(productSku) && !string.IsNullOrWhiteSpace(p.ProductSku) && p.ProductSku.ToLower().Contains(productSku.ToLower())) || string.IsNullOrWhiteSpace(productSku)) &&
                ((!string.IsNullOrWhiteSpace(maker) && !string.IsNullOrWhiteSpace(p.Maker) && p.Maker.ToLower().Contains(maker.ToLower())) || string.IsNullOrWhiteSpace(maker)))
               .OrderBy(pageWithSortDto.Sort).Skip(skip).Take(pageWithSortDto.PageSize).AsNoTracking();

            return await products.ToListAsync();

        }

        public async Task<Product> AddProductAsync(
            string name,
            string maker,
            string? mainName,
            string? subName,
            string? edition,
            string? version,
            string? commonName,
            string sku,
            string? detail,
            string? category,
            bool? hasActiveKey,
            bool? hasSubActiveKey,
            bool? hasActiveLink,
            bool? hasSerialNum,
            bool IsAvailable = false
        )
        {
            Product newProduct;
            var currentCategory = await _categoryRepo.GetQueryable().FirstOrDefaultAsync(c => c.Category == category);
            if (currentCategory == null)
            {
                newProduct = new()
                {
                    ProductName = name,
                    Maker = maker,
                    MainName = mainName,
                    SubName = subName,
                    ProductEdition = edition,
                    ProductVersion = version,
                    ProductCommonName = commonName,
                    ProductSku = sku,
                    ProductDetails = detail,
                    IsAvailable = IsAvailable,
                    CreatedTime = DateTime.Now,
                    EditTime = DateTime.Now,
                    HasSerialNum = hasSerialNum,
                    HasActiveKey = hasActiveKey,
                    HasSubActiveKey = hasSubActiveKey,
                    HasActiveLink = hasActiveLink,
                    ProductCategory = new ProductCategory
                    {
                        Category = category
                    }
                };
            }
            else
            {
                newProduct = new()
                {
                    ProductName = name,
                    Maker = maker,
                    MainName = mainName,
                    SubName = subName,
                    ProductEdition = edition,
                    ProductVersion = version,
                    ProductCommonName = commonName,
                    ProductSku = sku,
                    ProductDetails = detail,
                    IsAvailable = IsAvailable,
                    CreatedTime = DateTime.Now,
                    EditTime = DateTime.Now,
                    HasSerialNum = hasSerialNum,
                    HasActiveKey = hasActiveKey,
                    HasSubActiveKey = hasSubActiveKey,
                    HasActiveLink = hasActiveLink,
                    ProductCategory = currentCategory
                };
            }
            return await _productRepo.InsertAsync(newProduct);

        }



        public async Task<Product> EditProductAsync(int id,
            string? name
            // string? maker,
            // string? mainName,
            // string? subName,
            // string? edition,
            // string? version,
            // string? commonName,
            // string? sku,
            // string? detail,
            // string? category,
            // bool? hasActiveKey,
            // bool? hasSubActiveKey,
            // bool? hasActiveLink,
            // bool? hasSerialNum,
            // bool IsAvailable
        )
        {
            var targetProduct = await  _productRepo.GetQueryable().FirstOrDefaultAsync(p => p.Id == id);
            if (targetProduct == null)
            {
                throw new NullReferenceException("target product is not exist.");
            }
            else
            {
                targetProduct.ProductName = name??targetProduct.ProductName;
                return await _productRepo.UpdateAsync(targetProduct);
               
            }
        }


    }
}