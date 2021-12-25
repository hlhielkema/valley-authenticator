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

        public static bool TryImport(InternalStorageManager storage, Guid directoryId, ExportFormat format, string data, bool multiple)
        {
            switch (format)
            {
                case ExportFormat.KeyUri:
                    return TryImportKeyUriList(storage, directoryId, data);

                case ExportFormat.Json:
                    return TryImportJson(storage, directoryId, data, multiple);

                default:
                    throw new NotSupportedException();
            }
            
        }

        public static bool TryImportKeyUriList(InternalStorageManager storage, Guid directoryId, string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return false;       

            string[] lines = data.Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            List<OtpData> root = new List<OtpData>();
            Dictionary<string, List<OtpData>> subItems = new Dictionary<string, List<OtpData>>();

            string directory = null;
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                        directory = line.Substring(1, line.Length - 2);
                    else if (Uri.TryCreate(line, UriKind.Absolute, out Uri uri))
                    {
                        if (!KeyUriFormat.TryParseAppUri(uri, out OtpData otpData))
                            return false;

                        if (directory == null)
                            root.Add(otpData);
                        else
                        {
                            if (!subItems.TryGetValue(directory, out List<OtpData> list))
                                subItems.Add(directory, list = new List<OtpData>());
                            list.Add(otpData);
                        }
                    }
                    else
                        return false;
                }
            }

            foreach (OtpData entry in root)
                storage.AddEntry(directoryId, entry);

            foreach (KeyValuePair<string, List<OtpData>> sub in subItems)
            {                
                Guid childDirectoryId = storage.AddOrGetDirectory(directoryId, sub.Key.Split('/'));
                foreach (OtpData entry in sub.Value)
                    storage.AddEntry(childDirectoryId, entry);
            }

            return true;
        }

        public static bool TryImportJson(InternalStorageManager storage, Guid directoryId, string data, bool multiple)
        {
            if (multiple)
            {
                ExportDirectoryData obj;
                try
                {
                    obj = JsonConvert.DeserializeObject<ExportDirectoryData>(data);
                }
                catch
                {
                    return false;
                }

                if (ValidateJsonData(obj))
                {
                    ImportJsonData(storage, directoryId, obj);
                    return true;
                }
            }
            else
            {
                InternalOtpData entry;
                try
                {
                    entry = JsonConvert.DeserializeObject<InternalOtpData>(data);
                }
                catch
                {
                    return false;
                }

                if (!OtpData.Validate(entry))
                    return false;

                storage.AddEntry(directoryId, new OtpData(entry));
                return true;
            }

            return false;
        }

        private static bool ValidateJsonData(ExportDirectoryData data)
        {
            if (string.IsNullOrWhiteSpace(data.Name))
                return false;
            
            

            if (data.Directories != null)
            {
                foreach (InternalOtpData entry in data.OtpEntries)
                {
                    if (!OtpData.Validate(entry))
                        return false;
                }
            }                

            if (data.Directories != null)
            {
                foreach (ExportDirectoryData directory in data.Directories)
                {
                    if (!ValidateJsonData(directory))
                        return false;
                }
            }                

            return true;
        }

        private static void ImportJsonData(InternalStorageManager storage, Guid directoryId, ExportDirectoryData data)
        {
            if (data.OtpEntries != null)
            {
                foreach (InternalOtpData entry in data.OtpEntries)
                    storage.AddEntry(directoryId, new OtpData(entry));
            }

            if (data.Directories != null)
            {
                foreach (ExportDirectoryData directory in data.Directories)
                {
                    Guid childDirectoryId = storage.AddOrGetDirectory(directoryId, directory.Name);
                    ImportJsonData(storage, childDirectoryId, directory);
                }
            }
        }
    }
}
