using System;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Info;

namespace ValleyAuthenticator.Storage.Impl
{
    internal class OtpEntryContext : IOtpEntryContext
    {
        private AuthenticatorStorage _storage;
        private Guid _entryId;

        public OtpEntryContext(AuthenticatorStorage storage, Guid entryId)
        {
            _storage = storage;
            _entryId = entryId;
        }

        public bool Delete()
        {
            return _storage.DeleteEntry(_entryId);
        }

        public OtpData GetOtpData()
        {
            return _storage.GetEntryOtpData(_entryId);
        }
    }
}
