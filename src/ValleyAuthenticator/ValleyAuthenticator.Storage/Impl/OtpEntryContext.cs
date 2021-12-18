using System;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal;

namespace ValleyAuthenticator.Storage.Impl
{
    internal class OtpEntryContext : IOtpEntryContext
    {
        public string TypeDisplayName { get; } = "OTP entry";

        public bool Exists => _storage.OtpEntryExists(_entryId);

        // Private fields
        private InternalStorageManager _storage;
        private ContextManager _contextManager;
        private Guid _entryId;

        public OtpEntryContext(InternalStorageManager storage, ContextManager contextManager, Guid entryId)
        {
            _storage = storage;
            _contextManager = contextManager;
            _entryId = entryId;
        }

        public IOtpFormContext CreateEditFormContext()
            => new EditExistingOtpFormContext(this);

        public bool Delete()
        {
            Guid parent = _storage.GetOtpEntry(_entryId).Parent;            

            bool deleted = _storage.DeleteOtpEntry(_entryId);

            if (deleted)
                _contextManager.GetDirectoryContext(parent).OnItemDeleted(_entryId);

            return deleted;
        }

        public OtpData GetOtpData()
        {
            return new OtpData(_storage.GetOtpEntry(_entryId).Data);
        }

        public void SetOtpData(OtpData data)
        {
            _storage.GetOtpEntry(_entryId).Data = data.AsData();
        }
    }
}
