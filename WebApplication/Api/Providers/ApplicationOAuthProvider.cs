using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebApplication.Api.Models;
using WebApplication.Models;
using System.Web.Routing;
using System.Web;

namespace WebApplication.Api.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed)
            {
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                string callbackUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ((HttpContext.Current.Request.Url.Port == 80 || HttpContext.Current.Request.Url.Port == 443) ? "" : (":" + HttpContext.Current.Request.Url.Port)) + "/Account/ConfirmEmail?" + "userId=" + HttpUtility.UrlEncode(user.Id) + "&" + "code=" + HttpUtility.UrlEncode(code);
                string emailBody = "<a href='" + callbackUrl + "'Please click Here to confirm your email</a>";
                await userManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");

                //WebApplication.Controllers.AccountController accountController = new WebApplication.Controllers.AccountController();
                //var callbackUrl = accountController.Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: context.Request.Url.Scheme);
                //await userManager.SendEmailAsync(user.Id, "Confirm your account", callbackUrl);

                context.SetError("email_is_not_confirmed", "Email confirmation is required. Email confirmation link has been sent to the email");
                //TODO: this is not working. The response code will be 400. Investigate this.
                context.Response.StatusCode = 403;
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override System.Threading.Tasks.Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        public override System.Threading.Tasks.Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        public override System.Threading.Tasks.Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return System.Threading.Tasks.Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}