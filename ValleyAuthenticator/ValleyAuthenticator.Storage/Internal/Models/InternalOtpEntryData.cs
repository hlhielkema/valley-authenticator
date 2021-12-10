using Newtonsoft.Json;
using System;

namespace ValleyAuthenticator.Storage.Internal.Model
{
    internal sealed class InternalOtpEntryData
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("parent")]
        public Guid Parent { get; set; }

        [JsonProperty("otp_data")]
        public InternalOtpData Data { get; set; }
    }
}
