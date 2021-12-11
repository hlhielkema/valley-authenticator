using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Common;
using ValleyAuthenticator.Storage.Internal;

namespace ValleyAuthenticator.Storage
{
    public sealed class AuthenticatorStorage
    {
        // TODO: Add this as a service in the app

        // Private fields
        private static AuthenticatorStorage _instance;        
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
     
        public static IDirectoryContext GetRootDirectoryContext()
        {
            if (_instance == null)
                _instance = new AuthenticatorStorage();

            return _instance._contextCreator.GetDirectoryContext(_instance._storage.RootId);
        }
    }
}
