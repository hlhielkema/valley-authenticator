using System;

namespace ValleyAuthenticator.Storage.Models
{
    public sealed class AuthEntryInfo
    {
        public Guid Id { get; private set; }

        public OtpData Data { get; private set; }

        public AuthEntryInfo(Guid id, OtpData data)
        {
            Id = id;
            Data = data;
        }
    }
}
