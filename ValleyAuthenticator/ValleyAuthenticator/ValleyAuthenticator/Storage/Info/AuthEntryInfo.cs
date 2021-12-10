using System;
using ValleyAuthenticator.Storage.Otp;

namespace ValleyAuthenticator.Storage.Info
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
