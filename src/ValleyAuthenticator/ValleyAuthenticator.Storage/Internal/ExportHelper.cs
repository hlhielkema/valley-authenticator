using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal.Model;
using ValleyAuthenticator.Storage.Internal.Models;
using ValleyAuthenticator.Storage.Utils;

namespace ValleyAuthenticator.Storage.Internal
{
    internal static class ExportHelper
    {        
        public static string ExportEntry(InternalOtpEntryData entry, ExportFormat format)
        {
            switch (format)
            {
                case ExportFormat.KeyUri:
                    return KeyUriFormat.GenerateAppUri(new OtpData(entry.Data));

                case ExportFormat.Json:
                    return JsonConvert.SerializeObject(entry.Data, Formatting.Indented);

                default:
                    throw new NotSupportedException();
            }
        }

        public static string ExportDirectory(InternalDirectoryData directory, ExportFormat format)
        {
            switch (format)
            {
                case ExportFormat.KeyUri:
                    StringBuilder str = new StringBuilder();
                    ConstructKeyUriList(str, directory);
                    return str.ToString();

                case ExportFormat.Json:
                    return JsonConvert.SerializeObject(ConstructExportData(directory), Formatting.Indented);

                default:
                    throw new NotSupportedException();
            }
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

        private static void ConstructKeyUriList(StringBuilder str , InternalDirectoryData directory, string path = null)
        {
            if (directory.OtpEntries.Any())
            {
                if (path != null)
                    str.AppendLine(string.Format("[{0}]", path));
                foreach (InternalOtpEntryData entry in directory.OtpEntries)
                    str.AppendLine(KeyUriFormat.GenerateAppUri(new OtpData(entry.Data)));
                str.AppendLine();
            }

            string prefix = "";
            if (path != null)
                prefix = string.Format("{0}/", path);
            foreach (InternalDirectoryData childDirectory in directory.Directories)            
                ConstructKeyUriList(str, childDirectory, prefix + childDirectory.Name);            
        }
    }
}
