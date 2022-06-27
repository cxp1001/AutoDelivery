using AutoDelivery.Core;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain;
using Microsoft.EntityFrameworkCore;
using ShopifyWebApi.Domain.User;
using AutoDelivery.Core.Extensions;
using AutoDelivery.Core.Core;
using System.Collections;

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

        public async Task<List<Product>> GetAllProductsAsync(int userId, PageWithSortDto pageWithSortDto)
        {
            pageWithSortDto.Sort ??= "ProductName";
            var skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;

            var user = await _userRepo.GetQueryable().AsNoTracking().Include(u => u.Products).SingleOrDefaultAsync(u => u.Id == userId);

            if (user != null)
            {

                List<Product> products;
                if (pageWithSortDto.OrderType == OrderType.Asc)
                {
                    products = user.Products.OrderBy(p => p.ProductName).Skip(skip).Take(pageWithSortDto.PageSize).ToList();

                }
                else
                {
                    products = user.Products.OrderByDescending(p => p.ProductName).Skip(skip).Take(pageWithSortDto.PageSize).ToList();

                }

                return products;

            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        ///  获取当前用户设置的所有产品分类信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageWithSortDto"></param>
        /// <returns></returns>

        public async Task<IEnumerable> GetProductCategoriesAsync(int userId, PageWithSortDto pageWithSortDto)
        {
            pageWithSortDto.Sort ??= "Category";

            var skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;

            var user = await _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().SingleAsync(u => u.Id == userId);
            var userProductsId = user.Products.Select(p => p.Id);
            var product = _productRepo.GetQueryable().Include(p => p.ProductCategory).Where(p => p.ProductCategory != null).AsNoTracking();

            var res = from pId in userProductsId join q in product on pId equals q.Id select new { Categories = q.ProductCategory.Category };


            if (pageWithSortDto.OrderType == OrderType.Asc)
            {

                return res.Distinct().OrderBy(c => c.Categories).Skip(skip).Take(pageWithSortDto.PageSize);

            }
            else
            {
                return res.Distinct().OrderByDescending(c => c.Categories).Skip(skip).Take(pageWithSortDto.PageSize);
            }

        }



    }
}