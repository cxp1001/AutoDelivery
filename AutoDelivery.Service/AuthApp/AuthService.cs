using AutoDelivery.Core.Repository;
using AutoDelivery.Domain.User;

namespace AutoDelivery.Service.AuthApp
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<OAuthState> _oAuthRepo;
        public AuthService(IRepository<OAuthState> oAuthRepo)
        {
            this._oAuthRepo = oAuthRepo;

        }


        public async Task<OAuthState> CreateOAuthStateRecord(string shop)
        {
            var newOAuthState = new OAuthState
            {
                ShopifyShopDomain = shop,
                DateCreated = DateTimeOffset.Now,
                Token = Guid.NewGuid().ToString()
            };

            await _oAuthRepo.InsertAsync(newOAuthState);
            return newOAuthState;
        }

    }
}