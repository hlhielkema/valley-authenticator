using OtpNet;
using System;
using System.Collections.Specialized;
using System.Web;
using ValleyAuthenticator.Storage.Info;

namespace ValleyAuthenticator.Utils
{
    public class TotpUtilities
    {
        /// <summary>
        /// Create a secret for the code validation method.
        /// </summary>
        /// <returns></returns>
        public static string CreateSecret()
        {
            // Generate a new random key and encode is as base-32
            byte[] key = KeyGeneration.GenerateRandomKey(20);
            return Base32Encoding.ToString(key);
        }

        /// <summary>
        /// Create a URI which can be used to import the secret into an authenticator app.
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="secret">
        ///     Secret string.
        ///     Use <see cref="CreateSecret"/> to create a new secret.
        /// </param>
        /// <returns>
        ///     URI as a string
        /// </returns>
        public static string GenerateAppUri(OtpData data)
        {
            // Input validation
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // otpauth://TYPE/LABEL?PARAMETERS
            // https://github.com/google/google-authenticator/wiki/Key-Uri-Format

            // Encode the parameters
            string encodedSecret = Uri.EscapeDataString(data.Secret);
            string encodedLabel = Uri.EscapeDataString(data.Label);
            string encodedIssuer = Uri.EscapeDataString("Valley");

            // Format the URI
            return string.Format("otpauth://totp/{0}?secret={1}&issuer={2}", encodedLabel, encodedSecret, encodedIssuer);
        }

        public static bool TryParseAppUri(string uriString, out OtpData data)
        {
            data = null;
            if (Uri.TryCreate(uriString, UriKind.Absolute, out Uri uri))
            {
                if (uri.Host != "totp")
                    return false;

                string label = Uri.UnescapeDataString(uri.LocalPath.TrimStart('/'));
                NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);
                string secret = query["secret"];
                string issuer = query["issuer"];

                data = new OtpData(OtpType.Totp, label, secret, issuer);                

                // TODO: Move input validation

                return true;
            }

            return false;
        }
    }
}
