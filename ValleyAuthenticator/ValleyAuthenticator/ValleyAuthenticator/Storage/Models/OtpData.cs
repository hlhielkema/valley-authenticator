namespace ValleyAuthenticator.Storage.Models
{
    /// <summary>
    /// Data for TOTP or HOTP
    /// </summary>
    public sealed class OtpData
    {
        /// <summary>
        /// Time-based one-time password (TOTP) or counter-based one-time password (HOTP).
        /// </summary>
        public EntryType Type { get; set; }

        /// <summary>
        /// The label is used to identify which account a key is associated with.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The secret parameter is an arbitrary key value encoded in Base32 according to RFC 3548.
        /// The padding specified in RFC 3548 section 2.2 is not required and should be omitted.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// STRONGLY RECOMMENDED: The issuer parameter is a string value indicating the provider or service this account is associated with.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// OPTIONAL: The algorithm may have the values: SHA1(Default), SHA256 or SHA512
        /// </summary>
        public string Algorithm { get; set; } = "SHA1";

        /// <summary>
        /// OPTIONAL: The digits parameter may have the values 6 or 8, and determines how long of a one-time passcode to display to the user.
        /// The default is 6.
        /// </summary>
        public int Digits { get; set; } = 6;

        /// <summary>
        /// The counter parameter is required when provisioning a key for use with HOTP (required for HOTP).
        /// It will set the initial counter value.
        /// </summary>
        public int Counter { get; set; } = 0;

        /// <summary>
        /// The period parameter defines a period that a TOTP code will be valid for, in seconds.
        /// The default value is 30.
        /// </summary>
        public int Period { get; set; } = 30;

        public OtpData()
        { }

        public OtpData(EntryType type, string label, string secret, string issuer)
        {
            Type = type;
            Label = label;
            Secret = secret;
            Issuer = issuer;
        }
    }
}
