using System;
using System.Collections.Generic;
using System.Text;
using ValleyAuthenticator.Storage.Internal.Model;

namespace ValleyAuthenticator.Storage.Utils
{
    internal class DisplayUtilities
    {
        public static string FormatDirectoryLabel(InternalDirectoryData directory = null)
        {
            if (directory != null)
            {
                int entries = directory.OtpEntries.Count;
                int directories = directory.Directories.Count;

                if (entries > 0)
                {
                    string entriesLabel = (entries == 1) ? "1 entry" :
                                                           string.Format("{0} entries", entries);

                    if (directories > 0)
                    {
                        return (directories == 1) ? string.Format("{0}, 1 directory", entriesLabel) :
                                                    string.Format("{0}, {1} directories", entriesLabel, directories);
                    }

                    return entriesLabel;
                }
                else if (directories > 0)
                {
                    return (directories == 1) ? "1 directory" :
                                                string.Format("{0} directories", directories);
                }
            }

            return "empty";
        }
    }
}
