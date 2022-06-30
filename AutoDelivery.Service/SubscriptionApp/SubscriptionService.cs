using AutoDelivery.Core.Repository;
using AutoDelivery.Domain.Session;
using AutoDelivery.Domain.User;
using Microsoft.EntityFrameworkCore;

namespace AutoDelivery.Service.SubscriptionApp
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IRepository<UserAccount> _userRepo;

        public SubscriptionService(IRepository<UserAccount> userRepo)
        {
            this._userRepo = userRepo;
        }

        /// <summary>
        /// 从数据库中获取userId与session中的userid相同的用户,并判断用户是否付费
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<UserAccount> CheckUserChargeInfoAsync(Session session)
        {
            var user = await _userRepo.GetQueryable().SingleAsync(u => u.Id == session.UserId);
            return user;
        }



        /// <summary>
        /// 删除用户的chargeId
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<UserAccount> DeleteChargeIdAsync(UserAccount user)
        {
            user.ShopifyChargeId = null;
            return await _userRepo.UpdateAsync(user);
        }

        public async Task UpdateUserAsync(UserAccount user)
        {
            await _userRepo.UpdateAsync(user);
        }


    }
}