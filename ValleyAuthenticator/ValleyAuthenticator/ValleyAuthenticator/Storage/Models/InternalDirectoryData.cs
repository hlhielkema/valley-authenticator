using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ValleyAuthenticator.Storage.Models
{
    internal sealed class InternalDirectoryData
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonIgnore]
        public Guid? Parent { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("otp_entries")]
        public List<InternalOtpEntryData> OtpEntries { get; set; }

        [JsonProperty("directories")]
        public List<InternalDirectoryData> Directories { get; set; }
    }
}
