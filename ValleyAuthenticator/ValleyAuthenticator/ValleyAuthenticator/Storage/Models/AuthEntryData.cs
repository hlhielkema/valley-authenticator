using System;

namespace ValleyAuthenticator.Storage.Models
{
    public sealed class AuthEntryData
    {
        public Guid Id { get; set; }

        public Guid Parent { get; set; }

        public OtpData Data { get; set; }
    }
}
