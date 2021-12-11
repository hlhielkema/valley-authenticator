using System;
using System.Collections.Generic;
using System.Text;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Impl;

namespace ValleyAuthenticator.Storage.Internal
{
    internal class ContextManager
    {
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

        // Private fields
        private readonly InternalStorageManager _storage;
        private Dictionary<Guid, DirectoryContext> _directories;
        private Dictionary<Guid, OtpEntryContext> _otpEntries;

        public ContextManager(InternalStorageManager storage)
        {
            _storage = storage;
            _directories = new Dictionary<Guid, DirectoryContext>();
            _otpEntries = new Dictionary<Guid, OtpEntryContext>();
        }

        public void Reset()
        {
            _directories.Clear();
            _otpEntries.Clear();
        }
    }
}
