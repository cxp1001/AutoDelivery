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
        private readonly AutoDeliveryContext _dbContext;

        public UserService(IRepository<UserAccount> userRepo, AutoDeliveryContext dbContext)
        {
            this._userRepo = userRepo;
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable> GetAllProducts(int userId, PageWithSortDto pageWithSortDto)
        {
            pageWithSortDto.Sort ??= "ProductName";
            var skip = (pageWithSortDto.PageIndex - 1) * pageWithSortDto.PageSize;

            var user = await _userRepo.GetQueryable().AsNoTracking().SingleAsync(u => u.Id == userId);

            if (user != null)
            {
                IEnumerable products;
                if (pageWithSortDto.OrderType == OrderType.Asc)
                {
                    products = _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().FirstOrDefault(u => u.Id == userId).Products.Select(p => new { p.ProductName, p.Id }).OrderBy(p => p.ProductName).Skip(skip).Take(pageWithSortDto.PageSize);

                }
                else
                {
                    products = _userRepo.GetQueryable().Include(u => u.Products).AsNoTracking().FirstOrDefault(u => u.Id == userId).Products.Select(p => new { p.ProductName, p.Id }).OrderByDescending(p => p.ProductName).Skip(skip).Take(pageWithSortDto.PageSize);

                }

                return products;

            }
            else
            {
                throw new Exception();
            }
        }

    }
}