using Newtonsoft.Json;

namespace ValleyAuthenticator.Storage.Internal.Model
{
    /// <summary>
    /// Data for TOTP or HOTP
    /// </summary>
    internal sealed class InternalOtpData
    {
        /// <summary>
        /// Time-based one-time password (TOTP) or counter-based one-time password (HOTP).
        /// The value should be "TOTP" or "HOTP".
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// The label is used to identify which account a key is associated with.
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <summary>
        /// The secret parameter is an arbitrary key value encoded in Base32 according to RFC 3548.
        /// The padding specified in RFC 3548 section 2.2 is not required and should be omitted.
        /// </summary>
        [JsonProperty("secret")]
        public string Secret { get; set; }

        /// <summary>
        /// STRONGLY RECOMMENDED: The issuer parameter is a string value indicating the provider or service this account is associated with.
        /// </summary>
        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        /// <summary>
        /// OPTIONAL: The algorithm may have the values: SHA1(Default), SHA256 or SHA512
        /// </summary>
        [JsonProperty("algoritm")]
        public string Algorithm { get; set; }

        /// <summary>
        /// OPTIONAL: The digits parameter may have the values 6 or 8, and determines how long of a one-time passcode to display to the user.
        /// The default is 6.
        /// </summary>
        [JsonProperty("digits")]
        public int Digits { get; set; }

        /// <summary>
        /// The counter parameter is required when provisioning a key for use with HOTP (required for HOTP).
        /// It will set the initial counter value.
        /// </summary>
        [JsonProperty("counter")]
        public int Counter { get; set; }

        /// <summary>
        /// The period parameter defines a period that a TOTP code will be valid for, in seconds.
        /// The default value is 30.
        /// </summary>
        [JsonProperty("period")]
        public int Period { get; set; }
    }
}
