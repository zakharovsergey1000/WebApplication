using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApplication.Models;
using WebApplication.Api.Providers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication
{
    public partial class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                //AllowInsecureHttp = true
            };

            // This is a key step of the solution as we need to supply a meaningful and fully working
            // implementation of the OAuthBearerOptions object when we configure the OAuth Bearer authentication mechanism. 
            // The trick here is to reuse the previously defined OAuthOptions object that already
            // implements almost everything we need
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            OAuthBearerOptions.AccessTokenFormat = OAuthOptions.AccessTokenFormat;
            OAuthBearerOptions.AccessTokenProvider = OAuthOptions.AccessTokenProvider;
            OAuthBearerOptions.AuthenticationMode = OAuthOptions.AuthenticationMode;
            OAuthBearerOptions.AuthenticationType = OAuthOptions.AuthenticationType;
            OAuthBearerOptions.Description = OAuthOptions.Description;
            // The provider is the only object we need to redefine. See below for the implementation
            OAuthBearerOptions.Provider = new CustomBearerAuthenticationProvider();
            OAuthBearerOptions.SystemClock = OAuthOptions.SystemClock;

            // Enable the application to use bearer tokens to authenticate users
            // app.UseOAuthBearerTokens(OAuthOptions);
            app.UseOAuthAuthorizationServer(OAuthOptions);

            //Here we use the OAuthBearerOptions object 
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = VerifyToken.CLIENT_ID,
                ClientSecret = "ApSUNIdcC3blIq-xR6v2g9Vx"
            });
        }
    }

    public class CustomBearerAuthenticationProvider : OAuthBearerAuthenticationProvider
    {
        // This validates the identity based on the issuer of the claim.
        // The issuer is set in the API endpoint that logs the user in
        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            UserManager<ApplicationUser> userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(context.Ticket.Identity.GetUserId());

            var claims = context.Ticket.Identity.Claims;
            if (user == null || (claims.FirstOrDefault(claim => claim.Type == "AspNet.Identity.SecurityStamp") == null || claims.Any(claim => claim.Type == "AspNet.Identity.SecurityStamp" && !claim.Value.Equals(user.SecurityStamp))))
            {
                // Client could not be validated.
                context.SetError("invalid_token", "Client token is invalid.");
                context.Rejected();
            }

            return Task.FromResult<object>(null);
            //return Simple.FromResult(0);
        }
    }
}