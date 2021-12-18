using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
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
