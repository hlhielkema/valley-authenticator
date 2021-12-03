using System;

namespace ValleyAuthenticator.Storage.Models
{
    public sealed class AuthEntryInfo
    {
        public Guid Id { get; set; }

        public string Label { get; set; }

        public string Secret { get; set; }

        public string Issuer { get; set; }

        public AuthEntryInfo(Guid id, string label, string secret, string issuer)
        {
            Id = id;
            Label = label;
            Secret = secret;
            Issuer = issuer;
        }
    }
}
