﻿using AutoDelivery.Domain.Secrets;
using System.Text.RegularExpressions;

namespace AutoDelivery.Domain.Url
{
    public class ApplicationUrls : IApplicationUrls
    {
        public string OAuthRedirectUrl { get; }
        public string SubscriptionRedirectUrl { get; }
        public string AppUninstalledWebhookUrl { get; }
        public string ProxyScriptUrl { get; }

        public ApplicationUrls(ISecrets secrets)
        {
            OAuthRedirectUrl = JoinUrls(secrets.HostDomain, "/shopify/authresult");
            SubscriptionRedirectUrl = JoinUrls(secrets.HostDomain, "/subscription/chargeresult");
            AppUninstalledWebhookUrl = JoinUrls(secrets.HostDomain, "/webhooks/appuninstalled");


        }

        string JoinUrls(string left, string right)
        {
            var trimTrailingSlash = new Regex("/+$");
            var trimLeadingSlash = new Regex("^/+");

            return trimTrailingSlash.Replace(left, "") +
            "/" +
            trimLeadingSlash.Replace(right, "");
        }
    }
}
