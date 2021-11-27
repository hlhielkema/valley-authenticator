using System;

namespace ValleyAuthenticator.Storage.Models
{
    public sealed class AuthEntryInfo
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Secret { get; set; }

        public AuthEntryInfo(Guid id, string name, string secret)
        {
            Id = id;
            Name = name;
            Secret = secret;
        }
    }
}
