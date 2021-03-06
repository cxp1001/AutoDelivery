using AutoDelivery.Core;
using AutoDelivery.Core.Core;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain;
using AutoDelivery.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using AutoDelivery.Domain.User;
using Newtonsoft.Json;
using System.Security.Authentication;

namespace AutoDelivery.Service.ProductApp
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductCategory> _categoryRepo;
        private readonly AutoDeliveryContext _dbContext;
        private readonly IRepository<UserAccount> _userRepo;
        public ProductService(IRepository<UserAccount> userRepo, IRepository<Product> productRepo, IRepository<ProductCategory> categoryRepo, AutoDeliveryContext dbContext)
        {
            this._userRepo = userRepo;
            this._productRepo = productRepo;
            this._categoryRepo = categoryRepo;
            this._dbContext = dbContext;
        }


        public async Task<int> CountProductsAsync(int userId,
            string? productCategory,
            string? productName,
            string? productSku,
            string? maker)
        {
            var currentUser = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);
            if (currentUser == null)
            {
                throw new NullReferenceException(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 2,
                        ErrorMessage = "user is null",
                        Time = DateTimeOffset.Now
                    }
                    )

                );
            }
            var userProducts = currentUser.Products;
            var uPId = userProducts.Select(p => p.Id).ToList();


            var allProducts = _productRepo.GetQueryable().Where(p =>
                (p.ProductCategory.Category == productCategory || string.IsNullOrWhiteSpace(productCategory)) &&
                ((!string.IsNullOrWhiteSpace(productName) && !string.IsNullOrWhiteSpace(p.ProductName) && p.ProductName.ToLower().Contains(productName.ToLower())) || string.IsNullOrWhiteSpace(productName)) &&
                ((!string.IsNullOrWhiteSpace(productSku) && !string.IsNullOrWhiteSpace(p.ProductSku) && p.ProductSku.ToLower().Contains(productSku.ToLower())) || string.IsNullOrWhiteSpace(productSku)) &&
                ((!string.IsNullOrWhiteSpace(maker) && !string.IsNullOrWhiteSpace(p.Maker) && p.Maker.ToLower().Contains(maker.ToLower())) || string.IsNullOrWhiteSpace(maker))).Include(p => p.ProductCategory)
               .AsNoTracking();

            var productsCount = allProducts.Where(p => uPId.Contains(p.Id)).Count();
            return productsCount;
        }



        /// <summary>
        /// ????????????
        /// </summary>
        /// <param name="productCategory"></param>
        /// <param name="productName"></param>
        /// <param name="productSku"></param>
        /// <param name="maker"></param>
        /// <param name="pageWithSortDto"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetProductDtoAsync(int userId,
            string? productCategory,
            string? productName,
            string? productSku,
            string? maker,
            PageWithSortDto pageWithSortDto
        )
        {


            pageWithSortDto.Sort ??= "ProductName";
            var skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;


            var currentUser = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);

            if (currentUser == null)
            {
                throw new NullReferenceException(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 2,
                        ErrorMessage = "user is null",
                        Time = DateTimeOffset.Now
                    }
                    )

                );
            }

            var userProducts = currentUser.Products;
            var uPId = userProducts.Select(p => p.Id).ToList();


            var allProducts = _productRepo.GetQueryable().Where(p =>
                (p.ProductCategory.Category == productCategory || string.IsNullOrWhiteSpace(productCategory)) &&
                ((!string.IsNullOrWhiteSpace(productName) && !string.IsNullOrWhiteSpace(p.ProductName) && p.ProductName.ToLower().Contains(productName.ToLower())) || string.IsNullOrWhiteSpace(productName)) &&
                ((!string.IsNullOrWhiteSpace(productSku) && !string.IsNullOrWhiteSpace(p.ProductSku) && p.ProductSku.ToLower().Contains(productSku.ToLower())) || string.IsNullOrWhiteSpace(productSku)) &&
                ((!string.IsNullOrWhiteSpace(maker) && !string.IsNullOrWhiteSpace(p.Maker) && p.Maker.ToLower().Contains(maker.ToLower())) || string.IsNullOrWhiteSpace(maker))).Include(p => p.ProductCategory)
               .AsNoTracking();

            var resultProducts = allProducts.Where(p => uPId.Contains(p.Id)).OrderBy(pageWithSortDto.Sort).Skip(skip).Take(pageWithSortDto.PageSize);


            return await resultProducts.ToListAsync();

        }


        /// <summary>
        ///     ??????????????????
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="maker"></param>
        /// <param name="mainName"></param>
        /// <param name="subName"></param>
        /// <param name="productEdition"></param>
        /// <param name="productVersion"></param>
        /// <param name="productCommonName"></param>
        /// <param name="productSku"></param>
        /// <param name="detail"></param>
        /// <param name="category"></param>
        /// <param name="hasActiveKey"></param>
        /// <param name="hasSubActiveKey"></param>
        /// <param name="hasActiveLink"></param>
        /// <param name="hasSerialNum"></param>
        /// <param name="IsAvailable"></param>
        /// <returns></returns>
        public async Task<Product> AddProductAsync(
            int userId,
            string productName,
            string maker,
            string? mainName,
            string? subName,
            string? productEdition,
            string? productVersion,
            string? productCommonName,
            string productSku,
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
            category = category == null ? "Default" : category;

            var currentCategory = await _categoryRepo.GetQueryable().FirstOrDefaultAsync(c => c.Category == category);

            //category??????????????????????????????????????????????????????id=1?????????Default???category??? ?????????????????????????????????category

            // ??????????????????????????????????????????????????????????????????
            if (currentCategory == null)
            {
                newProduct = new()
                {
                    ProductName = productName,
                    Maker = maker,
                    MainName = mainName,
                    SubName = subName,
                    ProductEdition = productEdition,
                    ProductVersion = productVersion,
                    ProductCommonName = productCommonName,
                    ProductSku = productSku,
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
                    ProductName = productName,
                    Maker = maker,
                    MainName = mainName,
                    SubName = subName,
                    ProductEdition = productEdition,
                    ProductVersion = productVersion,
                    ProductCommonName = productCommonName,
                    ProductSku = productSku,
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
            var InsertedProduct = await _productRepo.InsertAsync(newProduct);
            var currentUser = _userRepo.GetQueryable().Include(u => u.Products).FirstOrDefault(u => u.Id == userId);

            if (currentUser != null)
            {
                var currentProducts = currentUser.Products;
                currentProducts.Add(InsertedProduct);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new NullReferenceException(
                   JsonConvert.SerializeObject(new
                   {
                       Status = 2,
                       ErrorMessage = "user is null",
                       Time = DateTimeOffset.Now
                   }
                   )

               );
            }

            return InsertedProduct;


        }



        public async Task<Product> EditProductAsync(int userId,
            int id,
            string? name,
            string? maker,
            string? mainName,
            string? subName,
            string? edition,
            string? version,
            string? commonName,
            string? sku,
            string? detail,
            string? category,
            int? categoryId,
            bool? hasActiveKey,
            bool? hasSubActiveKey,
            bool? hasActiveLink,
            bool? hasSerialNum,
            bool IsAvailable
        )
        {
            var targetProduct = await _productRepo.GetQueryable().FirstOrDefaultAsync(p => p.Id == id);
            if (targetProduct == null)
            {
                throw new NullReferenceException(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 8,
                        ErrorMessage = "target product is not exist.",
                        Time = DateTimeOffset.Now
                    }));

            }

            // ??????????????????????????????????????????????????????
            var currentUser = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);

            if (currentUser == null)
            {
                throw new NullReferenceException(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 2,
                        ErrorMessage = "user is null",
                        Time = DateTimeOffset.Now
                    }
                    )

                );
            }



            if (!currentUser.Products.Select(p => p.Id).Contains(targetProduct.Id))
            {
                throw new Exception(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 5,
                        ErrorMessage = "The current user does not have permission to change the current product configuration",
                        Time = DateTimeOffset.Now
                    }));
            }


            targetProduct.ProductName = name ?? targetProduct.ProductName;
            targetProduct.Maker = maker ?? targetProduct.Maker;
            targetProduct.MainName = mainName ?? targetProduct.MainName;
            targetProduct.SubName = subName ?? targetProduct.SubName;
            targetProduct.ProductEdition = edition ?? targetProduct.ProductEdition;
            targetProduct.ProductVersion = version ?? targetProduct.ProductVersion;
            targetProduct.ProductCommonName = commonName ?? targetProduct.ProductCommonName;
            targetProduct.ProductSku = sku ?? targetProduct.ProductSku;
            targetProduct.ProductDetails = detail ?? targetProduct.ProductDetails;

            // ????????????????????????????????????????????????????????????({category,categoryId})????????????
            if (!string.IsNullOrWhiteSpace(category) && categoryId.HasValue)
            {
                ProductCategory newCategory = new()
                {
                    Id = (int)categoryId,
                    Category = category
                };
                targetProduct.ProductCategory = newCategory;
            }


            targetProduct.HasActiveKey = hasActiveKey ?? targetProduct.HasActiveKey;
            targetProduct.HasSubActiveKey = hasSubActiveKey ?? targetProduct.HasSubActiveKey;
            targetProduct.HasActiveLink = hasActiveLink ?? targetProduct.HasActiveLink;
            targetProduct.HasSerialNum = hasSerialNum ?? targetProduct.HasSerialNum;
            targetProduct.IsAvailable = IsAvailable;
            targetProduct.EditTime = DateTimeOffset.Now;

            return await _productRepo.UpdateAsync(targetProduct);


        }



        public async Task<Product> DeleteProductAsync(int userId, int id)
        {
            var targetProduct = await _productRepo.GetQueryable().FirstOrDefaultAsync(p => p.Id == id);
            if (targetProduct == null)
            {
                throw new NullReferenceException(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 8,
                        ErrorMessage = "target product is not exist.",
                        Time = DateTimeOffset.Now
                    }));

            }

            // ??????????????????????????????????????????????????????
            var currentUser = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);

            if (currentUser == null)
            {
                throw new NullReferenceException(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 2,
                        ErrorMessage = "user is null",
                        Time = DateTimeOffset.Now
                    }
                    )

                );
            }

            if (!currentUser.Products.Select(p => p.Id).Contains(targetProduct.Id))
            {
                throw new Exception(
                    JsonConvert.SerializeObject(new
                    {
                        Status = 5,
                        ErrorMessage = "The current user does not have permission to change the current product configuration",
                        Time = DateTimeOffset.Now
                    }));
            }

            return await _productRepo.DeleteAsync(targetProduct);

        }


    }
}