using Newtonsoft.Json;
using System;
using ValleyAuthenticator.Storage.Internal.Model;

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
            Algorithm = "SHA1";
            Digits = 6;
            Counter = 0;
            Period = 30;
        }
    }
}
