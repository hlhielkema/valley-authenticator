using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Common;
using ValleyAuthenticator.Storage.Internal;

namespace ValleyAuthenticator.Storage
{
    public sealed class AuthenticatorStorage : IAuthenticatorStorage
    {        
        // Private fields
        private readonly InternalStorageManager _storage;
        private readonly ContextManager _contextCreator;

        public AuthenticatorStorage()
        {
             IPersistentStorage persistentStorage = new FileSystemStorage();

            _storage = new InternalStorageManager(persistentStorage);            

            if (!_storage.Load())
                _storage.AddTestData();

            _contextCreator = new ContextManager(_storage);
        }

        public IDirectoryContext GetRootDirectoryContext()
            => _contextCreator.GetDirectoryContext(_storage.RootId);
    }
}
