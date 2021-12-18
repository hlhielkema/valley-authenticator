using System.Collections.Generic;
using ValleyAuthenticator.Storage.Internal.Model;

namespace ValleyAuthenticator.Storage.Internal.Models
{
    internal class SearchResults
    {
        public List<InternalDirectoryData> Directories { get; private set; }

        public List<InternalOtpEntryData> OtpEntries { get; private set; }

        public SearchResults()
        {
            Directories = new List<InternalDirectoryData>();
            OtpEntries = new List<InternalOtpEntryData>();
        }
    }
}
