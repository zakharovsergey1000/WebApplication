using System;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Routing;
using System.Web.SessionState;
using Microsoft.IdentityModel.Tokens.JWT;
using System.Threading.Tasks;

namespace WebApplication
{
    /// <summary>
    ///  This is a minimal implementation of OAuth V2 verification that
    ///  demonstrates:
    ///    - ID Token validation
    ///    - Access token validation
    /// </summary>
    /// @author class@google.com (Gus Class)
    public class VerifyToken : IRequiresSessionState, IRouteHandler
    {
        // Get this from your app at https://code.google.com/apis/console
        static public string CLIENT_ID = "371943187438-3898288fpejdpui9dndhdb46d7tu63vp.apps.googleusercontent.com";

        // Values returned in the response
        access_token_status ats = new access_token_status();
        id_token_status its = new id_token_status();

        /// <summary>
        /// Processes the request based on the path.
        /// </summary>
        /// <param name="context">Contains the request and response.</param>
        public async Task<WebApplication.Api.Controllers.AccountController.GoogleUserViewModel> VerifyGoogleAccessToken(string accessToken, string idToken)
        {
            // Validate the ID token
            if (idToken != null)
            {
                JWTSecurityToken token = new JWTSecurityToken(idToken);
                JWTSecurityTokenHandler jwt = new JWTSecurityTokenHandler();

                // Configure validation
                Byte[][] certBytes = getCertBytes();
                for (int i = 0; i < certBytes.Length; i++)
                {
                    X509Certificate2 certificate = new X509Certificate2(certBytes[i]);
                    X509SecurityToken certToken = new X509SecurityToken(certificate);

                    // Set up token validation 
                    TokenValidationParameters tvp = new TokenValidationParameters();
                    tvp.AllowedAudience = CLIENT_ID;
                    tvp.SigningToken = certToken;
                    tvp.ValidIssuer = "accounts.google.com";

                    // Enable / disable tests                
                    tvp.ValidateNotBefore = false;
                    tvp.ValidateExpiration = true;
                    tvp.ValidateSignature = true;
                    tvp.ValidateIssuer = true;

                    // Account for clock skew. Look at current time when getting the message
                    // "The token is expired" in try/catch block.
                    // This is relative to GMT, for example, GMT-8 is:
                    tvp.ClockSkewInSeconds = 3600 * 13;

                    try
                    {
                        // Validate using the provider
                        ClaimsPrincipal cp = jwt.ValidateToken(token, tvp);
                        if (cp != null)
                        {
                            its.valid = true;
                            its.message = "Valid ID Token.";
                            its.aud = cp.FindFirst("aud").Value;
                            its.azp = cp.FindFirst("azp").Value;
                            its.email = cp.FindFirst("email").Value;
                            its.sub = cp.FindFirst("sub").Value;
                        }
                    }
                    catch (Exception e)
                    {
                        // Multiple certificates are tested.
                        if (its.valid != true)
                        {
                            its.message = "Invalid ID Token.";
                        }
                        if (e.Message.IndexOf("The token is expired") > 0)
                        {
                            // TODO: Check current time in the exception for clock skew.
                        }
                    }
                }

                // Get the Google+ id for this user from the "gplus_id" claim.
                Claim[] claims = token.Claims.ToArray<Claim>();
                for (int i = 0; i < claims.Length; i++)
                {
                    if (claims[i].Type.Equals("gplus_id"))
                    {
                        its.gplus_id = claims[i].Value;
                    }
                }
            }

            // Use the wrapper to return JSON
            token_status_wrapper tsr = new token_status_wrapper();
            tsr.id_token_status = its;
            tsr.access_token_status = ats;


            if (its.valid)
            {
                WebApplication.Api.Controllers.AccountController.GoogleUserViewModel fbUser = new Api.Controllers.AccountController.GoogleUserViewModel();
                fbUser.Email = its.email;
                fbUser.aud = its.aud;
                fbUser.azp = its.azp;
                fbUser.gplus_id = its.gplus_id;
                fbUser.sub = its.sub;
                return fbUser;
            }
            return null;
        }


        // Used for string parsing the Certificates from Google
        private const string beginCert = "-----BEGIN CERTIFICATE-----\\n";
        private const string endCert = "\\n-----END CERTIFICATE-----\\n";

        /// <summary>
        /// Retrieves the certificates for Google and returns them as byte arrays.
        /// </summary>
        /// <returns>An array of byte arrays representing the Google certificates.</returns>
        public byte[][] getCertBytes()
        {
            // The request will be made to the authentication server.
            WebRequest request = WebRequest.Create(
                "https://www.googleapis.com/oauth2/v1/certs"
            );

            StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());

            string responseFromServer = reader.ReadToEnd();

            String[] split = responseFromServer.Split(':');

            // There are two certificates returned from Google
            byte[][] certBytes = new byte[2][];
            int index = 0;
            UTF8Encoding utf8 = new UTF8Encoding();
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].IndexOf(beginCert) > 0)
                {
                    int startSub = split[i].IndexOf(beginCert);
                    int endSub = split[i].IndexOf(endCert) + endCert.Length;
                    certBytes[index] = utf8.GetBytes(split[i].Substring(startSub, endSub).Replace("\\n", "\n"));
                    index++;
                }
            }
            return certBytes;
        }


        /// <summary>
        /// Stores the result data for the ID token verification.
        /// </summary>
        class id_token_status
        {
            public Boolean valid = false;
            public String gplus_id = null;
            public String message = "";
            public String aud = null;
            public String azp = null;
            public String email = null;
            public String sub = null;
        }

        /// <summary>
        /// Stores the result data for the access token verification.
        /// </summary>
        class access_token_status
        {
            public Boolean valid = false;
            public String gplus_id = null;
            public String message = "";
        }

        class token_status_wrapper
        {
            public id_token_status id_token_status = null;
            public access_token_status access_token_status = null;
        }

        /// <summary>
        /// Implements IRouteHandler interface for mapping routes to this
        /// IHttpHandler.
        /// </summary>
        /// <param name="requestContext">Information about the request.
        /// </param>
        /// <returns></returns>
        public IHttpHandler GetHttpHandler(RequestContext
            requestContext)
        {
            var page = BuildManager.CreateInstanceFromVirtualPath
                 ("~/verifytoken.ashx", typeof(IHttpHandler)) as IHttpHandler;
            return page;
        }

        public bool IsReusable { get { return false; } }
    }
}