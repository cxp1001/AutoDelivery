using AutoDelivery.Domain;
using AutoDelivery.Domain.Session;
using AutoDelivery.Domain.User;

namespace AutoDelivery.Service.SubscriptionApp
{
    public interface ISubscriptionService : IocTag
    {
        Task<UserAccount> CheckUserChargeInfoAsync(Session session);
        Task<UserAccount> DeleteChargeIdAsync(UserAccount user);
        Task UpdateUserAsync(UserAccount user);
    }
}