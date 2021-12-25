using Newtonsoft.Json;
using System.Collections.Generic;
using ValleyAuthenticator.Storage.Internal.Model;

namespace ValleyAuthenticator.Storage.Internal.Models
{
    internal class ExportDirectoryData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("otp_entries")]
        public List<InternalOtpData> OtpEntries { get; set; }

        [JsonProperty("directories")]
        public List<ExportDirectoryData> Directories { get; set; }
    }
}
