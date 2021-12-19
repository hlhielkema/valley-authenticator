using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Utils
{
    public class KeyUriFormat
    {
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
