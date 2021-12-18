using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal;
using ValleyAuthenticator.Storage.Internal.Model;
using ValleyAuthenticator.Storage.Internal.Models;
using ValleyAuthenticator.Storage.Utils;

namespace ValleyAuthenticator.Storage.Impl
{
    internal sealed class SearchContext : ISearchContext
    {
        // Private fields
        private InternalStorageManager _storage;
        private ContextManager _contextManager;
        private ObservableCollection<NodeInfo> _collection;
        private Guid _directoryId;
        private bool _disposed;

        // Private constants
        private string IMAGE_OTP_ENTRY = "key.png";
        private string IMAGE_DIRECTORY = "folder.png";

        public SearchContext(InternalStorageManager storage, ContextManager contextManager, Guid directoryId)
        {
            _storage = storage;
            _contextManager = contextManager;
            _directoryId = directoryId;
        }

        public void Dispose()
        {            
            _disposed = true;
            _collection.Clear();
            _collection = null;
        }

        public ObservableCollection<NodeInfo> ListAndSubscribe()
        {
            if (_disposed)
                throw new ObjectDisposedException("SearchContext");
            if (_collection == null)
                _collection = new ObservableCollection<NodeInfo>();

            return _collection;
        }
        
        public void Update(string searchQuery)
        {
            if (_disposed)
                throw new ObjectDisposedException("SearchContext");
            if (_collection == null)
                _collection = new ObservableCollection<NodeInfo>();

            List<NodeInfo> resultsList = Search(searchQuery);

            UpdateObservable(resultsList);
        }

        private List<NodeInfo> Search(string searchQuery)
        {
            List<NodeInfo> resultsList = new List<NodeInfo>();

            SearchResults searchResults = _storage.Search(_directoryId, searchQuery);            

            foreach (InternalDirectoryData directory in searchResults.Directories)
            {
                IDirectoryContext context = _contextManager.GetDirectoryContext(directory.Id);

                string detail = DisplayUtilities.FormatDirectoryLabel(directory);

                resultsList.Add(new NodeInfo(context, directory.Id, directory.Name, detail, IMAGE_DIRECTORY));
            }

            foreach (InternalOtpEntryData entry in searchResults.OtpEntries)
            {
                IOtpEntryContext context = _contextManager.GetOtpEntryContext(entry.Id);

                resultsList.Add(new NodeInfo(context, entry.Id, entry.Data.Issuer, entry.Data.Label, IMAGE_OTP_ENTRY));
            }

            return resultsList;
        }

        private void UpdateObservable(List<NodeInfo> resultsList)
        {
            if (resultsList.Count == 0)
            {
                _collection.Clear();
            }
            else if (_collection.Count == 0)
            {
                foreach (NodeInfo x in resultsList)
                    _collection.Add(x);
            }
            else if (!CollectionItemsMatch(_collection, resultsList))
            {               
                HashSet<Guid> existing = new HashSet<Guid>(_collection.Select(x => x.Id));
                HashSet<Guid> updated = new HashSet<Guid>(resultsList.Select(x => x.Id));

                for (int i = 0; i < _collection.Count; i++)
                {                    
                    if (!updated.Contains(_collection[i].Id))
                        _collection.RemoveAt(i--);
                }

                for (int i = 0; i < resultsList.Count; i++)
                {
                    if (!existing.Contains(resultsList[i].Id))                    
                        _collection.Insert(i, resultsList[i]);                    
                }

#if DEBUG
                if (!CollectionItemsMatch(_collection, resultsList))
                    throw new Exception("Failed to update observable");
#endif
            }
        }

        private static bool CollectionItemsMatch(ObservableCollection<NodeInfo> collection, List<NodeInfo> resultsList)
        {
            if (resultsList.Count != collection.Count)
                return false;

            for (int i = 0; i < resultsList.Count; i++)
            {
                if (resultsList[i].Id != collection[i].Id)
                    return false;
            }
            
            return true;
        }

        public void Validate()
        {
            if (_disposed)
                throw new ObjectDisposedException("SearchContext");

            if (_collection != null)
            {
                for (int i = 0; i < _collection.Count; i++)
                {
                    NodeInfo node = _collection[i];
                    if (!node.Context.Exists)
                    {
                        _collection.RemoveAt(i--);
                    }
                    else if (node.Context is IDirectoryContext directoryContext)
                    {                        
                        string detail = directoryContext.GetDetailLabel();
                        if (node.Detail != detail)
                            node.UpdateDetail(detail);
                    }
                }
            }
        }       
    }
}
