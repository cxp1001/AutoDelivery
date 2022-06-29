using AutoDelivery.Core.Core;
using AutoDelivery.Core.Repository;
using AutoDelivery.Domain.Result;
using AutoDelivery.Domain.Secrets;
using AutoDelivery.Domain.Session;
using AutoDelivery.Domain.Url;
using AutoDelivery.Domain.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopifySharp;

namespace AutoDelivery.Service.ShopifyApp
{
    public class ShopifyService : IShopifyService
    {
        private readonly ISecrets _secrets;
        private readonly AutoDeliveryContext _dbContext;
        private readonly IApplicationUrls _applicationUrls;
        private readonly IRepository<UserAccount> _userRepo;
        private readonly IRepository<OAuthState> _stateRepo;

        public ShopifyService(IRepository<UserAccount> userRepo, IRepository<OAuthState> stateRepo, AutoDeliveryContext dbContex, IApplicationUrls applicationUrls, ISecrets secrets)
        {
            this._userRepo = userRepo;
            this._stateRepo = stateRepo;
            this._applicationUrls = applicationUrls;
            this._dbContext = dbContex;
            this._secrets = secrets;
        }


        public async Task<int> HandShakeAsync(string shop, Session session, HttpContext httpContext)
        {

            if (string.IsNullOrWhiteSpace(shop))
            {
                return 1;
                //Problem("Request is missing shop querystring parameter.", statusCode: 422);
            }

            // 检查当前用户是否已登录
            if (httpContext.User.Identity.IsAuthenticated)
            {
                // 确认用户输入的商店和他们已登录的商店一致
                var user = await _userRepo.GetQueryable().AsNoTracking().FirstAsync(u => u.Id == session.UserId);

                // 用户已登录，且输入的商店和当前登录的商店一致，可以跳转到主界面
                if (user.ShopifyShopDomain == shop)
                {
                    return 2;
                    //RedirectToPageResult("/dashboard/index");
                }

                //The domains do not match, so the user likely owns two or more Shopify shops and they're trying
                // to log in to a separate one.
                // Log them out and erase their auth cookie and send them to the login/install page
                // 商店域名不匹配，用户可能拥有多个商店且正在尝试登录另一个商店。
                // 注销用户的登录并清除cookie，将用户跳转到登录/安装页面
                await httpContext.SignOutAsync();
            }

            // 用户未安装应用，或未登录，或是登录到了另外的商店，将用户跳转到登录/安装页面
            return 3;
            //return RedirectToPage("/auth/login")
        }


        // 查看token是否存在于数据库中
        public async Task<OAuthState> CheckTokenAsync(string state)
        {

            var dbToken = await _stateRepo.GetQueryable().FirstOrDefaultAsync(l => l.Token == state);
            return dbToken;

        }


        public async Task DeleteTokenAsync(OAuthState dbToken)
        {
            // 删除token防止被再次使用
            await _stateRepo.DeleteAsync(dbToken);
        }


        public async Task<UserAccount> UserUpdate(string shop, Shop shopData, string accessToken)
        {
            // 查找包含当前商店的用户是否存在并更新其信息，若不存在则创建用户
            var user = await _userRepo.GetQueryable().FirstOrDefaultAsync(u => u.ShopifyShopDomain == shop);

            if (user == null)
            {
                // 创建新用户
                user = new UserAccount
                {
                    ShopifyAccessToken = accessToken,
                    ShopifyShopDomain = shop,
                    ShopifyShopId = shopData.Id.Value
                };
                await _userRepo.InsertAsync(user);
            }
            else
            {
                // 更新用户的账号
                user.ShopifyAccessToken = accessToken;
                user.ShopifyShopDomain = shop;
                user.ShopifyShopId = shopData.Id.Value;
            }

            await _userRepo.UpdateAsync(user);
            return user;
        }







    }
}
