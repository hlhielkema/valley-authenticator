using Newtonsoft.Json;
using System;
using System.Linq;
using ValleyAuthenticator.Storage.Internal.Model;
using ValleyAuthenticator.Storage.Utils;
using ValleyAuthenticator.Utils;

namespace ValleyAuthenticator.Storage.Abstract.Models
{
    /// <summary>
    /// Data for TOTP or HOTP
    /// </summary>
    public sealed class OtpData
    {
        /// <summary>
        /// Time-based one-time password (TOTP) or counter-based one-time password (HOTP).
        /// </summary>
        public OtpType Type { get; private set; }

        /// <summary>
        /// The label is used to identify which account a key is associated with.
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// The secret parameter is an arbitrary key value encoded in Base32 according to RFC 3548.
        /// The padding specified in RFC 3548 section 2.2 is not required and should be omitted.
        /// </summary>
        public string Secret { get; private set; }

        /// <summary>
        /// STRONGLY RECOMMENDED: The issuer parameter is a string value indicating the provider or service this account is associated with.
        /// </summary>
        public string Issuer { get; private set; }

        /// <summary>
        /// OPTIONAL: The algorithm may have the values: SHA1(Default), SHA256 or SHA512
        /// </summary>
        public string Algorithm { get; private set; }

        /// <summary>
        /// OPTIONAL: The digits parameter may have the values 6 or 8, and determines how long of a one-time passcode to display to the user.
        /// The default is 6.
        /// </summary>
        public int Digits { get; private set; }

        /// <summary>
        /// The counter parameter is required when provisioning a key for use with HOTP (required for HOTP).
        /// It will set the initial counter value.
        /// </summary>
        public int Counter { get; private set; }

        /// <summary>
        /// The period parameter defines a period that a TOTP code will be valid for, in seconds.
        /// The default value is 30.
        /// </summary>
        public int Period { get; private set; }

        public static OtpData Default
          => new OtpData(OtpType.Totp, "", "", "");

        internal OtpData(InternalOtpData source)
        {
            if (!Enum.TryParse<OtpType>(source.Type, out OtpType type))
                throw new Exception("Invalid OtpType");

            Type = type;
            Label = source.Label;
            Secret = source.Secret;
            Issuer = source.Issuer;
            Algorithm = source.Algorithm;
            Digits = source.Digits;
            Counter = source.Counter;
            Period = source.Period;
        }

        internal static bool Validate(InternalOtpData source)
        {
            if (source == null)
                return false;

            // Type
            if (!Enum.TryParse(source.Type, out OtpType _))
                return false;

            // Label
            if (string.IsNullOrWhiteSpace(source.Label))
                return false;

            // Secret
            if (string.IsNullOrWhiteSpace(source.Secret) || !TotpUtilities.ValidateSecret(source.Secret))
                return false;

            // Issuer
            if (string.IsNullOrWhiteSpace(source.Issuer))
                return false;

            // Algorithm
            if (string.IsNullOrWhiteSpace(source.Algorithm) || !TotpUtilities.SUPPORTED_ALGORITHMS.Contains(source.Algorithm))
                return false;

            // Digits
            if (source.Digits <= 0 || !TotpUtilities.SUPPORTED_DIGIT_COUNTS.Contains(source.Digits))
                return false;

            // Counter
            if (source.Counter < 0)
                return false;

            // Period
            if (!TotpUtilities.SUPPORTED_PERIOD_VALUES.Contains(source.Period))
                return false;

            return true;
        }

        internal InternalOtpData AsData()
        {
            return new InternalOtpData()
            { 
                Type = Type.ToString(),
                Label = Label,
                Secret = Secret,
                Issuer = Issuer,
                Algorithm = Algorithm,
                Digits = Digits,
                Counter = Counter,
                Period = Period,                   
            };            
        }

        public OtpData(OtpType type, string label, string secret, string issuer)
        {
            Type = type;
            Label = label;
            Secret = secret;
            Issuer = issuer;
            Algorithm = KeyUriFormat.DEFAULT_ALGORITHM;
            Digits = KeyUriFormat.DEFAULT_DIGITS;
            Counter = KeyUriFormat.DEFAULT_COUNTER;
            Period = KeyUriFormat.DEFAULT_PERIOD;
        }

        public OtpData(OtpType type, string label, string secret, string issuer, string algorithm, int digits, int counter, int period)
        {
            Type = type;
            Label = label;
            Secret = secret;
            Issuer = issuer;
            Algorithm = algorithm;
            Digits = digits;
            Counter = counter;
            Period = period;
        }      

        public OtpData Next()
            => new OtpData(Type, Label, Secret, Issuer, Algorithm, Digits, Counter + 1, Period);
    }
}
