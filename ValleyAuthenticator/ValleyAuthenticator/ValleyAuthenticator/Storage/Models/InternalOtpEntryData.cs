using Newtonsoft.Json;
using System;
using ValleyAuthenticator.Storage.Otp;

namespace ValleyAuthenticator.Storage.Models
{    
    internal sealed class InternalOtpEntryData
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("parent")]
        public Guid Parent { get; set; }

        [JsonProperty("otp_data")]
        public OtpData Data { get; set; }
    }
}
