using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebApplication.Api.Models;
using WebApplication.Api.Providers;
using WebApplication.Api.Results;
using WebApplication.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Owin.Security.DataProtection;
using System.Web.Routing;
using System.Text.RegularExpressions;

namespace WebApplication.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationDbContext db;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
            db = new ApplicationDbContext();
            //_userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                //return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        //// POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GoogleLogin")]
        //http://blog.valeriogheri.com/asp-net-webapi-identity-system-how-to-login-with-facebook-access-token
        public async Task<IHttpActionResult> GoogleLogin(AddExternalLoginBindingModel model)
        {
            if (string.IsNullOrEmpty(model.ExternalAccessToken))
            {
                return BadRequest("Invalid OAuth access token");
            }
            ApplicationUser user = null;

            // Get the fb access token and make a graph call to the /me endpoint
            VerifyToken verifyToken = new VerifyToken();
            var fbUser = await verifyToken.VerifyGoogleAccessToken(null, model.ExternalAccessToken);
            if (fbUser == null)
            {
                return BadRequest("Invalid OAuth access token");
            }
            if (!fbUser.aud.Equals(VerifyToken.CLIENT_ID))
            {
                return BadRequest("Invalid OAuth access token");
            }

            try
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    if (model.Email != null)
                    {
                        // model.Email is an application account's email, not google login's email
                        user = await UserManager.FindByEmailAsync(model.Email);
                        if (user == null)
                        {
                            return BadRequest("Invalid Email");
                        }
                        //
                        bool isGoogleLoginAlreadyBelongToTheUser = false;
                        var userLogins = await UserManager.GetLoginsAsync(user.Id);
                        foreach (UserLoginInfo userLoginInfo in userLogins)
                        {
                            if (userLoginInfo.LoginProvider.Equals("Google") && userLoginInfo.ProviderKey.Equals(fbUser.sub))
                            {
                                isGoogleLoginAlreadyBelongToTheUser = true;
                                break;
                            }
                        }
                        if (!isGoogleLoginAlreadyBelongToTheUser)
                        {
                            bool isGoogleLoginAlreadyBelongToSomeOtherUser = db.Users.Where(u => u.Logins.Any(l => l.LoginProvider.Equals("Google") && l.ProviderKey.Equals(fbUser.sub))).Count() > 0;
                            if (isGoogleLoginAlreadyBelongToSomeOtherUser)
                            {
                                return BadRequest("Invalid OAuth access token");
                            }
                            else
                            {
                                if (!user.EmailConfirmed && !user.Email.ToLower().Equals(fbUser.Email.ToLower()))
                                {
                                    return BadRequest("Application account's email is not confirmed yet");
                                }
                                //add login
                                //user.Logins.Add(new IdentityUserLogin() { LoginProvider = "Google", ProviderKey = fbUser.sub });

                                IdentityResult result = await UserManager.AddLoginAsync(user.Id, new UserLoginInfo("Google", fbUser.sub));

                                if (!result.Succeeded)
                                {
                                    return GetErrorResult(result);
                                }
                                //make email confirmed
                                user.EmailConfirmed = true;
                            }
                        }
                        else
                        {
                            //nothing to do
                        }
                    }
                    else
                    {
                        //try to find user
                        user = db.Users.Where(u => u.Logins.Any(l => l.LoginProvider.Equals("Google") && l.ProviderKey.Equals(fbUser.sub))).SingleOrDefault();
                        if (user == null)
                        {
                            // user not found so 
                            //create user and add login
                            string randomPassword;

                            //At least one lower case letter
                            Regex r1 = new Regex("[a-z]+");
                            //At least one upper case letter
                            Regex r2 = new Regex("[A-Z]+");
                            //At least one digit
                            Regex r3 = new Regex("[0-9]+");

                            string NewPassword = System.Web.Security.Membership.GeneratePassword(10, 5);

                            while (!(r1.IsMatch(NewPassword) && r2.IsMatch(NewPassword) && r3.IsMatch(NewPassword)))
                            {
                                //regenerate new password
                                NewPassword = System.Web.Security.Membership.GeneratePassword(10, 5);
                            }
                            randomPassword = NewPassword;
                            user = new ApplicationUser() { UserName = fbUser.Email, Email = fbUser.Email, EmailConfirmed = true };
                            IdentityResult result = await UserManager.CreateAsync(user, randomPassword);
                            if (!result.Succeeded)
                            {
                                return GetErrorResult(result);
                                //return Conflict();
                            }

                            //user.Logins.Add(new IdentityUserLogin() { LoginProvider = "Google", ProviderKey = fbUser.sub });

                            IdentityResult result2 = await UserManager.AddLoginAsync(user.Id, new UserLoginInfo("Google", fbUser.sub));

                            if (!result2.Succeeded)
                            {
                                return GetErrorResult(result2);
                            }

                        }
                        else
                        {
                            //nothing to do
                        }


                    }
                    db.SaveChanges();
                    dbTransaction.Commit();
                }
            }
            catch (Exception e)
            {
                return Conflict();
            }
            // Finally sign-in the user: this is the key part of the code that creates the bearer token and authenticate the user
            var identity = new ClaimsIdentity(Startup.OAuthOptions.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id, null, ClaimsIdentity.DefaultIssuer));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName, null, ClaimsIdentity.DefaultIssuer));
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", null, ClaimsIdentity.DefaultIssuer));
            identity.AddClaim(new Claim("AspNet.Identity.SecurityStamp", user.SecurityStamp, null, ClaimsIdentity.DefaultIssuer));
            AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            var tokenExpirationTimeSpan = TimeSpan.FromDays(14);
            ticket.Properties.ExpiresUtc = currentUtc.Add(tokenExpirationTimeSpan);
            var accesstoken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
            Authentication.SignIn(identity);

            // Create the response
            JObject blob = new JObject(
                new JProperty("userName", user.UserName),
                new JProperty("access_token", accesstoken),
                new JProperty("token_type", "bearer"),
                new JProperty("expires_in", (long)tokenExpirationTimeSpan.TotalSeconds),
                new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
            );
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(blob);
            // Return OK
            return Ok(blob);
        }
        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using (var dbTransaction = db.Database.BeginTransaction())
            {
                ApplicationUser applicationUser = db.Users.Where(b => (b.Email.Equals(model.Email))).SingleOrDefault();
                if (applicationUser != null)
                {
                    return Ok(new CreateUserResponse(CreateUserResult.THE_USER_WITH_REQUESTED_USERNAME_ALREADY_EXISTS, DateTime.UtcNow));
                }
                else
                {
                    var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

                    IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                    if (!result.Succeeded)
                    {
                        return GetErrorResult(result);
                    }

                    try
                    {
                        //UserManager.EmailService = new EmailService();
                        // var provider = new DpapiDataProtectionProvider();
                        // UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("ASP.NET Identity"));
                        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                        var newRouteValues = new RouteValueDictionary(new { userId = user.Id, code = code });
                        newRouteValues.Add("httproute", true);
                        System.Web.Mvc.UrlHelper urlHelper = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext, RouteTable.Routes);
                        string callbackUrl = urlHelper.Action(
                            "ConfirmEmail",
                            "Account",
                            newRouteValues,
                            HttpContext.Current.Request.Url.Scheme
                            );
                        var x = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + "/Account/ConfirmEmail?";
                        var t = HttpContext.Current.Request.Url.Port;
                        callbackUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + ((HttpContext.Current.Request.Url.Port == 80 || HttpContext.Current.Request.Url.Port == 443) ? "" : (":" + HttpContext.Current.Request.Url.Port)) + "/Account/ConfirmEmail?" + "userId=" + HttpUtility.UrlEncode(user.Id) + "&" + "code=" + HttpUtility.UrlEncode(code);
                        string emailBody = "<a href='" + callbackUrl + "'Please click Here to confirm your email</a>";
                        await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");



                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }

                    //  var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", callbackUrl);

                    db.SaveChanges();

                    dbTransaction.Commit();

                    return Ok(new CreateUserResponse(CreateUserResult.SUCCESS, DateTime.UtcNow));
                }
            }
        }

        //
        // GET: api/Account/ConfirmEmail
        //[AllowAnonymous]
        //[HttpGet]
        //[Route("ConfirmEmail")]
        //public IHttpActionResult ConfirmEmail(string userId, string code)
        //{
        //    //if (userId == null || code == null)
        //    //{
        //    //    //return View("Error");
        //    //}
        //    //var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    //return View(result.Succeeded ? "ConfirmEmail" : "Error");

        //    return Redirect("https://advancedcalendar.biz/Account/ConfirmEmail?" + "userId=" + HttpUtility.UrlEncode(userId) + "&" + "code=" + HttpUtility.UrlEncode(code));
        //}

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private async Task<FacebookUserViewModel> VerifyFacebookAccessToken(string accessToken)
        {
            FacebookUserViewModel fbUser = null;
            var path = "https://graph.facebook.com/me?access_token=" + accessToken;
            var client = new HttpClient();
            var uri = new Uri(path);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                fbUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserViewModel>(content);
            }
            //var response = await client.GetStringAsync(uri);
            //if ()
            //{
            //    fbUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserViewModel>(response);
            //}
            return fbUser;
        }
        private async Task<FacebookUserViewModel> VerifyGoogleAccessToken(string accessToken)
        {
            FacebookUserViewModel fbUser = null;
            var path = "https://graph.facebook.com/me?access_token=" + accessToken;
            var client = new HttpClient();
            var uri = new Uri(path);
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                fbUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserViewModel>(content);
            }
            //var response = await client.GetStringAsync(uri);
            //if ()
            //{
            //    fbUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserViewModel>(response);
            //}
            return fbUser;
        }


        public class FacebookUserViewModel
        {
            [JsonProperty("id")]
            public string ID { get; set; }
            [JsonProperty("first_name")]
            public string FirstName { get; set; }
            [JsonProperty("last_name")]
            public string LastName { get; set; }
            [JsonProperty("username")]
            public string Username { get; set; }
            [JsonProperty("email")]
            public string Email { get; set; }
        }

        public class GoogleUserViewModel
        {
            public string aud { get; set; }
            public string azp { get; set; }
            public string Email { get; set; }
            public string gplus_id { get; set; }
            public string sub { get; set; }
        }
        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
