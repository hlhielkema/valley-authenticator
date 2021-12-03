using System;

namespace ValleyAuthenticator.Storage.Models
{
    public sealed class AuthEntryData
    {
        public Guid Id { get; set; }

        public Guid Parent { get; set; }

        public string Label { get; set; }

        public string Secret { get; set; }

        public string Issuer { get; set; }
    }
}
