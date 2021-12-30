using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal;
using ValleyAuthenticator.Storage.Internal.Model;
using ValleyAuthenticator.Storage.Internal.Models;
using ValleyAuthenticator.Storage.Utils;

namespace ValleyAuthenticator.Storage.Impl
{
    internal class ObservableCollectionManager
    {
        public ObservableCollection<NodeInfo> Collection => _collection;

        private readonly ObservableCollection<NodeInfo> _collection;
        private readonly InternalStorageManager _storage;
        private readonly ContextManager _contextManager;

        // Private constants
        private const string IMAGE_OTP_ENTRY = "key.png";
        private const string IMAGE_DIRECTORY = "folder.png";

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

            OtpData data = context.OtpData;

            NodeInfo node = new NodeInfo(context, id, data.Issuer, data.Label, IMAGE_OTP_ENTRY);

            _collection.Add(node);
        }

        public void AddDirectory(Guid id)
        {
            IDirectoryContext context = _contextManager.GetDirectoryContext(id);

            string detail = DisplayUtilities.FormatDirectoryLabel();

            NodeInfo node = new NodeInfo(context, id, context.Name, detail, IMAGE_DIRECTORY);

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
                    string detail = DisplayUtilities.FormatDirectoryLabel(directory);
                    _collection.Add(new NodeInfo(context, directory.Id, directory.Name, detail, IMAGE_DIRECTORY));
                }
            }

            foreach (InternalOtpEntryData entry in target.OtpEntries)
            {
                if (!inCollection.Contains(entry.Id))
                {
                    IOtpEntryContext context = _contextManager.GetOtpEntryContext(entry.Id);
                    _collection.Add(new NodeInfo(context, entry.Id, entry.Data.Issuer, entry.Data.Label, IMAGE_OTP_ENTRY));
                }
            }
        }

        public void ReplaceWithSearchResults(SearchResults searchResults)
        {
            List<NodeInfo> resultsList = new List<NodeInfo>();

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

                string detail = DisplayUtilities.FormatDirectoryLabel(directory);

                initial.Add(new NodeInfo(context, directory.Id, directory.Name, detail, IMAGE_DIRECTORY));
            }

            foreach (InternalOtpEntryData entry in target.OtpEntries)
            {
                IOtpEntryContext context = contextManager.GetOtpEntryContext(entry.Id);

                initial.Add(new NodeInfo(context, entry.Id, entry.Data.Issuer, entry.Data.Label, IMAGE_OTP_ENTRY));
            }

            ObservableCollection<NodeInfo> collection = new ObservableCollection<NodeInfo>(initial);

            return new ObservableCollectionManager(storage, contextManager, collection);
        }
    }
}
