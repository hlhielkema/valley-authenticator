using OtpNet;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Utils
{
    public class TotpUtilities
    {
        public static readonly int[] SUPPORTED_DIGIT_COUNTS = new int[] { 6,7,8,9,10,11,12 };
        public static readonly string[] SUPPORTED_ALGORITHMS = new string[] { "SHA1", "SHA256", "SHA512" };
        public static readonly string[] SUPPORTED_PERIOD_NAMES = new string[] { "15s", "30s", "1m", "2m", "5m", "10m" };
        public static readonly int[] SUPPORTED_PERIOD_VALUES = new int[] { 15, 30, 60, 120, 300, 600 };
        public static readonly string[] TYPE_NAMES = new string[] { "TOTP", "HOTP" };
        public static readonly OtpType[] TYPE_VALUES = new OtpType[] { OtpType.Totp, OtpType.Hotp };

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

        public static bool TryParsePeriodName(string name, out int period)
        {
            int index = Array.IndexOf(SUPPORTED_PERIOD_NAMES, name);
            if (index == -1)
            {
                period = 0;
                return false;
            }
            else
            {
                period = SUPPORTED_PERIOD_VALUES[index];
                return true;
            }
        }

        public static bool TryParseTypeName(string name, out OtpType type)
        {
            int index = Array.IndexOf(TYPE_NAMES, name);
            if (index == -1)
            {
                type = 0;
                return false;
            }
            else
            {
                type = TYPE_VALUES[index];
                return true;
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> ListInformation(OtpData data)
        {
            string typeName = TYPE_NAMES[Array.IndexOf(TYPE_VALUES, data.Type)];
            string periodName = TYPE_NAMES[Array.IndexOf(SUPPORTED_PERIOD_VALUES, data.Period)];

            yield return new KeyValuePair<string, string>("Type", typeName);
            yield return new KeyValuePair<string, string>("Issuer", data.Issuer);
            yield return new KeyValuePair<string, string>("Label", data.Label);
            yield return new KeyValuePair<string, string>("Secret", data.Secret);
            yield return new KeyValuePair<string, string>("Algorithm", data.Algorithm);
            yield return new KeyValuePair<string, string>("Digits", data.Digits.ToString());
            if (data.Type == OtpType.Hotp)
                yield return new KeyValuePair<string, string>("Counter", data.Counter.ToString());
            if (data.Type == OtpType.Totp)
                yield return new KeyValuePair<string, string>("Period", periodName);
        }
    }
}
