using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal;
using ValleyAuthenticator.Storage.Internal.Model;
using ValleyAuthenticator.Storage.Internal.Models;

namespace ValleyAuthenticator.Storage.Impl
{
    internal class ObservableCollectionManager
    {
        public ObservableCollection<NodeInfo> Collection => _collection;

        private readonly ObservableCollection<NodeInfo> _collection;
        private readonly InternalStorageManager _storage;
        private readonly ContextManager _contextManager;

        private ObservableCollectionManager(InternalStorageManager storage, ContextManager contextManager, ObservableCollection<NodeInfo> collection)
        {
            // Set private fields
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _contextManager = contextManager ?? throw new ArgumentNullException(nameof(contextManager));            
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }        

        public void AddEntry(Guid id)
        {
            IOtpEntryContext context = _contextManager.GetOtpEntryContext(id);            
            _collection.Add(context.GetInfo());
        }

        public void AddDirectory(Guid id)
        {
            IDirectoryContext context = _contextManager.GetDirectoryContext(id);
            
            NodeInfo node = context.GetInfo();

            // Insert the new directory after the last directory in the collection (before any entries).
            // The directories are always at the top of the list.
            int last = 0;
            while (last < _collection.Count && _collection[last].Context is IDirectoryContext)
                last++;
            _collection.Insert(last, node);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool RemoveById(Guid id)
        {
            for (int i = 0; i < _collection.Count; i++)
            {
                if (_collection[i].Id == id)
                {
                    _collection.RemoveAt(i);
                    return true;
                }
            }
            return true;
        }

        public void Validate()
        {
            for (int i = 0; i < _collection.Count; i++)
            {
                NodeInfo node = _collection[i];
                if (!node.Context.Exists)                
                    _collection.RemoveAt(i--);                
                else                
                    node.Refresh();                
            }
        }

        public void AddMissingNodesForDirectory(Guid directoryId)
        {
            HashSet<Guid> inCollection = new HashSet<Guid>();
            foreach (NodeInfo node in _collection)
                inCollection.Add(node.Id);

            InternalDirectoryData target = _storage.GetDirectory(directoryId);

            foreach (InternalDirectoryData directory in target.Directories)
            {
                if (!inCollection.Contains(directory.Id))
                {
                    IDirectoryContext context = _contextManager.GetDirectoryContext(directory.Id);
                    _collection.Add(context.GetInfo());
                }
            }

            foreach (InternalOtpEntryData entry in target.OtpEntries)
            {
                if (!inCollection.Contains(entry.Id))
                {
                    IOtpEntryContext context = _contextManager.GetOtpEntryContext(entry.Id);
                    _collection.Add(context.GetInfo());
                }
            }
        }

        public void ReplaceWithSearchResults(SearchResults searchResults)
        {
            List<NodeInfo> resultsList = new List<NodeInfo>();

            foreach (InternalDirectoryData directory in searchResults.Directories)
            {
                IDirectoryContext context = _contextManager.GetDirectoryContext(directory.Id);
                resultsList.Add(context.GetInfo());
            }

            foreach (InternalOtpEntryData entry in searchResults.OtpEntries)
            {
                IOtpEntryContext context = _contextManager.GetOtpEntryContext(entry.Id);
                resultsList.Add(context.GetInfo());
            }

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

        public static ObservableCollectionManager CreateEmpty(InternalStorageManager storage, ContextManager contextManager)
        {
            ObservableCollection<NodeInfo> collection = new ObservableCollection<NodeInfo>();

            return new ObservableCollectionManager(storage, contextManager, collection);
        }

        public static ObservableCollectionManager CreateForDirectory(InternalStorageManager storage, ContextManager contextManager, Guid directoryId)
        {
            List<NodeInfo> initial = new List<NodeInfo>();

            InternalDirectoryData target = storage.GetDirectory(directoryId);

            foreach (InternalDirectoryData directory in target.Directories)
            {
                IDirectoryContext context = contextManager.GetDirectoryContext(directory.Id);

                initial.Add(context.GetInfo());
            }

            foreach (InternalOtpEntryData entry in target.OtpEntries)
            {
                IOtpEntryContext context = contextManager.GetOtpEntryContext(entry.Id);

                initial.Add(context.GetInfo());
            }

            ObservableCollection<NodeInfo> collection = new ObservableCollection<NodeInfo>(initial);

            return new ObservableCollectionManager(storage, contextManager, collection);
        }
    }
}
