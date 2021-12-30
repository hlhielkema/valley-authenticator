using System;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal;
using ValleyAuthenticator.Storage.Internal.Models;

namespace ValleyAuthenticator.Storage.Impl
{
    internal sealed class SearchContext : ISearchContext
    {
        // Private fields
        private readonly InternalStorageManager _storage;
        private readonly ContextManager _contextManager;
        private ObservableCollectionManager _collectionManager;
        private Guid _directoryId;
        private bool _disposed;
        
        public SearchContext(InternalStorageManager storage, ContextManager contextManager, Guid directoryId)
        {
            // Input validation
            if (directoryId == Guid.Empty)
                throw new ArgumentException(nameof(directoryId));

            // Set private fields
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _contextManager = contextManager ?? throw new ArgumentNullException(nameof(contextManager));
            _directoryId = directoryId;
        }

        public void Dispose()
        {            
            _disposed = true;
            _collectionManager.Clear();
            _collectionManager = null;
        }

        public ObservableCollection<NodeInfo> ListAndSubscribe()
        {
            if (_disposed)
                throw new ObjectDisposedException("SearchContext");
            if (_collectionManager == null)
                _collectionManager = ObservableCollectionManager.CreateEmpty(_storage, _contextManager);

            return _collectionManager.Collection;
        }
        
        public void Update(string searchQuery)
        {
            if (_disposed)
                throw new ObjectDisposedException("SearchContext");
            if (_collectionManager == null)
                _collectionManager = ObservableCollectionManager.CreateEmpty(_storage, _contextManager);
            
            SearchResults searchResults = _storage.Search(_directoryId, searchQuery);
            _collectionManager.ReplaceWithSearchResults(searchResults);
        }
       
        public void Validate()
        {
            if (_disposed)
                throw new ObjectDisposedException("SearchContext");

            if (_collectionManager != null)
                _collectionManager.Validate();
        }       
    }
}
