using System;
using System.Collections.Generic;

namespace ValleyAuthenticator.Storage.Models
{
    public sealed class AuthDirectoryData
    {
        public Guid Id { get; set; }

        public Guid? Parent { get; set; }

        public string Name { get; set; }

        public List<AuthEntryData> Entries { get; set; }

        public List<AuthDirectoryData> Directories { get; set; }
    }
}
