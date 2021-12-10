using System;
using System.Collections.Generic;
using System.Text;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Info;

namespace ValleyAuthenticator.Storage.Impl
{
    internal class DirectoryContext : IDirectoryContext
    {
        private AuthenticatorStorage _storage;
        private Guid? _directoryId;

        public DirectoryContext(AuthenticatorStorage storage, Guid? directoryId)
        {            
            _storage = storage;
            _directoryId = directoryId;
        }

        public List<NodeInfo> GetChilds()
        {
            return _storage.GetForDirectory(_directoryId);
        }

        public Guid AddEntry(OtpData data)
        {
            return _storage.AddEntry(_directoryId, data);
        }

        public Guid AddDirectory(string name)
        {
            return _storage.AddDirectory(_directoryId, name);
        }

        public bool Delete()
        {
            if (!_directoryId.HasValue)
                throw new InvalidOperationException("The root directory cannot be deleted");

            return _storage.DeleteDirectory(_directoryId.Value);            
        }
    }
}
