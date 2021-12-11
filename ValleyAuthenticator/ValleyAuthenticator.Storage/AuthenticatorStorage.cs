using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Internal;

namespace ValleyAuthenticator.Storage
{
    public sealed class AuthenticatorStorage
    {
        // Private fields
        private static AuthenticatorStorage _instance;        
        private readonly InternalStorageManager _storage;
        private readonly ContextManager _contextCreator;

        public AuthenticatorStorage()
        {
            _storage = new InternalStorageManager();            
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
