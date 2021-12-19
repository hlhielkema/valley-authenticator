using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Utils;

namespace ValleyAuthenticator.Storage.Utils
{
    /// <summary>
    /// OTP Key URI format
    /// https://github.com/google/google-authenticator/wiki/Key-Uri-Format#issuer
    /// </summary>
    public class KeyUriFormat
    {
        // Constants
        public const string DEFAULT_ALGORITHM = "SHA1";
        public const int DEFAULT_DIGITS = 6;
        public const int DEFAULT_PERIOD = 30;
        public const int DEFAULT_COUNTER = 0;

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
            
            StringBuilder str = new StringBuilder("otpauth://");
            str.Append(data.Type.ToString().ToLower());
            str.Append('/');
            str.Append(Uri.EscapeDataString(data.Label));

            bool first = true;
            foreach (KeyValuePair<string, string> parameter in Serialize(data))
            {
                str.Append(first ? "?":"&");
                first = false;
                str.Append(parameter.Key);
                str.Append('=');
                str.Append(Uri.EscapeDataString(parameter.Value));
            }

            return str.ToString();
        }

        private static IEnumerable<KeyValuePair<string, string>> Serialize(OtpData data)
        {
            // Secret
            yield return new KeyValuePair<string, string>("secret", data.Secret);

            // Issuer
            if (!string.IsNullOrWhiteSpace(data.Issuer))
                yield return new KeyValuePair<string, string>("issuer", data.Issuer);
            
            // Algorithm
            if (!string.IsNullOrWhiteSpace(data.Algorithm) && data.Algorithm != DEFAULT_ALGORITHM)
                yield return new KeyValuePair<string, string>("algorithm", data.Algorithm);

            // Digits
            if (data.Digits != DEFAULT_DIGITS)
                yield return new KeyValuePair<string, string>("digits", data.Digits.ToString());            

            if (data.Type == OtpType.Totp)
            {
                // Period
                if (data.Period != DEFAULT_PERIOD)
                    yield return new KeyValuePair<string, string>("period", data.Period.ToString());
            }
            else if (data.Type == OtpType.Hotp)
            {
                // Counter
                yield return new KeyValuePair<string, string>("counter", data.Counter.ToString());
            }
        }

        public static bool TryParseAppUri(string uriString, out OtpData data)
        {
            // Parse the URI
            data = null;
            if (!Uri.TryCreate(uriString, UriKind.Absolute, out Uri uri))
                return false;                       
            if (uri.Scheme != "otpauth")
                return false;

            // Type
            int index = Array.IndexOf(TotpUtilities.TYPE_NAMES, uri.Host.ToUpper());
            if (index == -1)
                return false;
            OtpType type = TotpUtilities.TYPE_VALUES[index];                

            // Label
            string label = Uri.UnescapeDataString(uri.LocalPath.TrimStart('/'));
            if (string.IsNullOrWhiteSpace(label))
                return false;

            // Parse the URI parameters
            NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);                               

            // Secret
            string secret = query["secret"];
            if (string.IsNullOrWhiteSpace(secret))
                return false;

            // Issuer
            string issuer = query["issuer"];
            if (string.IsNullOrWhiteSpace(issuer))
                return false;

            // Issuer
            string algorithm = "SHA1";
            if (query["digits"] != null)
            {
                algorithm = query["digits"];
                if (!TotpUtilities.SUPPORTED_ALGORITHMS.Contains(algorithm))
                    return false;
            }

            // Digits
            int digits = 6;
            if (query["digits"] != null)
            {
                if (!int.TryParse(query["digits"], out digits) || !TotpUtilities.SUPPORTED_DIGIT_COUNTS.Contains(digits))
                    return false;
            }

            // Counter
            int counter = 0;
            if (query["counter"] != null)
            {
                if (!int.TryParse(query["counter"], out counter) || counter < 0)
                    return false;
            }

            // Period
            int period = 30;
            if (query["period"] != null)
            {
                if (!int.TryParse(query["period"], out period) || !TotpUtilities.SUPPORTED_PERIOD_VALUES.Contains(period))
                    return false;
            }

            data = new OtpData(type, label, secret, issuer, algorithm, digits, counter, period);

            return true;
            
        }
    }
}
