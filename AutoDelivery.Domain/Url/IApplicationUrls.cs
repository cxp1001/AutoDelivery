namespace AutoDelivery.Domain.Url
{
    public interface IApplicationUrls
    {
        string OAuthRedirectUrl { get; }
        string SubscriptionRedirectUrl { get; }
        string AppUninstalledWebhookUrl { get; }
        string ProxyScriptUrl { get; }
    }
}