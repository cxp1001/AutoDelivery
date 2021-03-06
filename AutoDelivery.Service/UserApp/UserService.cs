using AutoDelivery.Core;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain;
using Microsoft.EntityFrameworkCore;
using AutoDelivery.Domain.User;
using AutoDelivery.Core.Extensions;
using AutoDelivery.Core.Core;
using System.Collections;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace AutoDelivery.Service.UserApp
{
    public class UserService : IUserService
    {
        private readonly IRepository<UserAccount> _userRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly AutoDeliveryContext _dbContext;

        public UserService(IRepository<UserAccount> userRepo, IRepository<Product> productRepo, AutoDeliveryContext dbContext)
        {
            this._userRepo = userRepo;
            this._productRepo = productRepo;
            this._dbContext = dbContext;
        }


        public async Task<int> CountAllProductsAsync(int userId)
        {
            var currentUser = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);
            if (currentUser != null)
            {

                var productsCount = currentUser.Products.Count();

                return productsCount;

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

        }

        public async Task<List<Product>> GetAllProductsAsync(int userId, PageWithSortDto pageWithSortDto)
        {
            pageWithSortDto.Sort ??= "ProductName";
            var skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;

            var currentUser = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);


            if (currentUser != null)
            {
                List<Product> products;
                if (pageWithSortDto.OrderType == OrderType.Asc)
                {
                    products = currentUser.Products.OrderBy(p => p.ProductName).Skip(skip).Take(pageWithSortDto.PageSize).ToList();
                }
                else
                {
                    products = currentUser.Products.OrderByDescending(p => p.ProductName).Skip(skip).Take(pageWithSortDto.PageSize).ToList();

                }
                return products;

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
        }


        public async Task<int> CountCategories(int userId)
        {
            var user = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);
            var userProductsId = user.Products.Select(p => p.Id);
            var product = _productRepo.GetQueryable().Include(p => p.ProductCategory).Where(p => p.ProductCategory != null).AsNoTracking();
            var categoryCount = product.Where(p => userProductsId.Contains(p.Id)).Select(p => p.ProductCategory).Count();
            return categoryCount;
        }

        /// <summary>
        ///  ???????????????????????????????????????????????????
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageWithSortDto"></param>
        /// <returns></returns>

        public async Task<List<ProductCategory>> GetProductCategoriesAsync(int userId, PageWithSortDto pageWithSortDto)
        {
            pageWithSortDto.Sort ??= "Category";

            var skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;

            var user = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleOrDefaultAsync(u => u.Id == userId);
            var userProductsId = user.Products.Select(p => p.Id);
            var product = _productRepo.GetQueryable().Include(p => p.ProductCategory).Where(p => p.ProductCategory != null).AsNoTracking();
            var categories = product.Where(p => userProductsId.Contains(p.Id)).Select(p => p.ProductCategory);


            if (pageWithSortDto.OrderType == OrderType.Asc)
            {

                return categories.Distinct().OrderBy(c => c.Category).Skip(skip).Take(pageWithSortDto.PageSize).ToList();

            }
            else
            {
                return categories.Distinct().OrderByDescending(c => c.Category).Skip(skip).Take(pageWithSortDto.PageSize).ToList();
            }

        }
    }

}
