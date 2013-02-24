using Omegaluz.SimpleOAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinktecture.IdentityServer.Web.Properties;
using Thinktecture.IdentityServer.Web.Providers;

namespace Thinktecture.IdentityServer.Web
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            SimpleOAuthSecurity.SetProvider(new OAuthMembershipProxyProvider());

            SimpleOAuthSecurity.RegisterFacebookClient(Settings.Default.FacebookAppId, Settings.Default.FacebookAppSecret);
            SimpleOAuthSecurity.RegisterTwitterClient(Settings.Default.TwitterConsumerKey, Settings.Default.TwitterConsumerSecret);
            SimpleOAuthSecurity.RegisterLinkedInClient(Settings.Default.LinkedInConsumerKey, Settings.Default.LinkedInConsumerSecret);
            SimpleOAuthSecurity.RegisterGoogleClient();

        }
    }
}