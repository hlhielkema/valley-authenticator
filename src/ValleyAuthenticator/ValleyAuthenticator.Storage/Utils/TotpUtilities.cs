using OtpNet;
using System;
using System.Collections.Generic;
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

        public static bool ValidateSecret(string secret)
        {
            if (string.IsNullOrEmpty(secret))
                return false;
            foreach (char c in secret)
            {
                if (!(c < '[' && c > '@') && !(c < '8' && c > '1') && !(c < '{' && c > '`'))                
                    return false;
            }
            return true;
        }

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
            string periodName = SUPPORTED_PERIOD_NAMES[Array.IndexOf(SUPPORTED_PERIOD_VALUES, data.Period)];

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
