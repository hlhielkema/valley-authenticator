using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ValleyAuthenticator.Storage.Internal.Model;
using ValleyAuthenticator.Storage.Internal.Models;

namespace ValleyAuthenticator.Storage.Internal
{
    internal static class ExportHelper
    {
        public static string ExportDirectoryToJson(InternalDirectoryData directory)
        {
            return JsonConvert.SerializeObject(ConstructExportData(directory), Formatting.Indented); 
        }

        public static string ExportEntryToJson(InternalOtpEntryData entry)
        {
            return JsonConvert.SerializeObject(entry.Data, Formatting.Indented);
        }

        private static ExportDirectoryData ConstructExportData(InternalDirectoryData directory)
        {
            ExportDirectoryData data = new ExportDirectoryData()
            { 
                Name = directory.Name,
                OtpEntries = new List<InternalOtpData>(),
                Directories = new List<ExportDirectoryData>(),
            };

            foreach (InternalDirectoryData childDirectory in directory.Directories)            
                data.Directories.Add(ConstructExportData(childDirectory));

            foreach (InternalOtpEntryData entry in directory.OtpEntries)
                data.OtpEntries.Add(entry.Data);

            return data;
        }
    }
}
