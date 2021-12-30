using System;
using System.Collections.Generic;
using ValleyAuthenticator.Storage.Impl;

namespace ValleyAuthenticator.Storage.Internal
{
    internal class ContextManager
    {
        // Private fields
        private readonly InternalStorageManager _storage;
        private readonly Dictionary<Guid, DirectoryContext> _directories;
        private readonly Dictionary<Guid, OtpEntryContext> _otpEntries;

        public ContextManager(InternalStorageManager storage)
        {
            _storage = storage;
            _directories = new Dictionary<Guid, DirectoryContext>();
            _otpEntries = new Dictionary<Guid, OtpEntryContext>();
        }

        public DirectoryContext GetDirectoryContext(Guid directoryId)
        {
            if (!_directories.TryGetValue(directoryId, out DirectoryContext directoryContext))
            {
                directoryContext = new DirectoryContext(_storage, this, directoryId);
                _directories.Add(directoryId, directoryContext);
            }
            return directoryContext;
        }

        public OtpEntryContext GetOtpEntryContext(Guid entryId)
        {
            if (!_otpEntries.TryGetValue(entryId, out OtpEntryContext entryContext))
            {
                entryContext = new OtpEntryContext(_storage, this, entryId);
                _otpEntries.Add(entryId, entryContext);
            }
            return entryContext;
        }

        public SearchContext CreateSearchContext(Guid directoryId)
            => new SearchContext(_storage, this, directoryId);

        public void Reset()
        {
            _directories.Clear();
            _otpEntries.Clear();
        }
    }
}
