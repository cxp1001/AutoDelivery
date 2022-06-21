using Microsoft.Extensions.Configuration;

namespace AutoDelivery.Domain.Secrets
{
    public class Secrets : ISecrets
    {
        private readonly IConfiguration _configuration;
        public string ShopifySecretKey { get; }
        public string ShopifyPublicKey { get; }
        public string HostDomain { get; }

        public Secrets(IConfiguration configuration)
        {
            this._configuration = configuration;
            ShopifyPublicKey = Find("SHOPIFY_PUBLIC_KEY");
            ShopifySecretKey = Find("SHOPIFY_SECRET_KEY");
            HostDomain = Find("HOST-DOMAIN");

        }

        string Find(string key)
        {
            var value = _configuration.GetValue<string>($"Secrets:{key}");
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NullReferenceException(key);
            }
            return value;
        }
    }
}
