﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal;
using ValleyAuthenticator.Storage.Internal.Model;

namespace ValleyAuthenticator.Storage.Impl
{
    internal class DirectoryContext : IDirectoryContext
    {
        public string TypeDisplayName { get; } = "Directory";

        // Private fields
        private InternalStorageManager _storage;
        private ContextManager _contextManager;
        private Guid _directoryId;
        private List<ObservableCollection<NodeInfo>> _subscribed;

        // Private constants
        private string IMAGE_OTP_ENTRY = "key.png";
        private string IMAGE_DIRECTORY = "folder.png";

        public DirectoryContext(InternalStorageManager storage, ContextManager contextManager, Guid directoryId)
        {            
            _storage = storage;
            _contextManager = contextManager;
            _directoryId = directoryId;
            _subscribed = new List<ObservableCollection<NodeInfo>>();
        }
      
        public ObservableCollection<NodeInfo> ListAndSubscribe()
        {            
            List<NodeInfo> initial = new List<NodeInfo>();

            InternalDirectoryData target = _storage.GetDirectory(_directoryId);

            foreach (InternalDirectoryData directory in target.Directories)
            {
                IDirectoryContext context = _contextManager.GetDirectoryContext(directory.Id);

                string detail = FormatDirectoryLabel(directory);

                initial.Add(new NodeInfo(context, directory.Id, directory.Name, detail, IMAGE_DIRECTORY));
            }

            foreach (InternalOtpEntryData entry in target.OtpEntries)
            {
                IOtpEntryContext context = _contextManager.GetOtpEntryContext(entry.Id);

                initial.Add(new NodeInfo(context, entry.Id, entry.Data.Issuer, entry.Data.Label, IMAGE_OTP_ENTRY));
            }

            ObservableCollection<NodeInfo> collection = new ObservableCollection<NodeInfo>(initial);

            _subscribed.Add(collection);

            return collection;
        }

        public void Unsubscribe(ObservableCollection<NodeInfo> collection)
        {
            _subscribed.Remove(collection);
        }

        public Guid AddEntry(OtpData data)
        {
            Guid id = _storage.AddEntry(_directoryId, data);

            IOtpEntryContext context = _contextManager.GetOtpEntryContext(id);

            NodeInfo node = new NodeInfo(context, id, data.Issuer, data.Label, IMAGE_OTP_ENTRY);

            foreach (ObservableCollection<NodeInfo> collection in _subscribed)
                collection.Add(node);

            UpdateLabelInParent();

            return id;
        }

        public Guid AddDirectory(string name)
        {
            Guid id = _storage.AddDirectory(_directoryId, name);

            IDirectoryContext context = _contextManager.GetDirectoryContext(id);

            string detail = FormatDirectoryLabel();

            NodeInfo node = new NodeInfo(context, id, name, detail, IMAGE_DIRECTORY);

            foreach (ObservableCollection<NodeInfo> collection in _subscribed)
            {
                // Insert the new directory after the last directory in the collection (before any entries).
                // The directories are always at the top of the list.

                int last = 0;
                while (last < collection.Count && collection[last].Context is IDirectoryContext)
                    last++;
                collection.Insert(last, node);
            }
            
            UpdateLabelInParent();

            return id;
        }

        private void UpdateLabelInParent()
        {
            Guid? parent = _storage.GetDirectory(_directoryId).Parent;
            if (parent != null)
            {
                DirectoryContext directoryContext = _contextManager.GetDirectoryContext(parent.Value);
                directoryContext.UpdateDirectoryLabel(_directoryId);
            }
        }

        private void UpdateDirectoryLabel(Guid directoryId)
        {
            InternalDirectoryData directory = _storage.GetDirectory(directoryId);

            string detail = FormatDirectoryLabel(directory);

            foreach (ObservableCollection<NodeInfo> collection in _subscribed)
            {
                foreach (NodeInfo node in collection)
                {
                    if (node.Id == directoryId)
                    {
                        node.UpdateDetail(detail);
                        break;
                    }
                }
            }
        }

        private string FormatDirectoryLabel(InternalDirectoryData directory = null)
        {
            if (directory != null)
            {
                int entries = directory.OtpEntries.Count;
                int directories = directory.Directories.Count;

                if (entries > 0)
                {
                    string entriesLabel = (entries == 1) ? "1 entry" :
                                                           string.Format("{0} entries", entries);

                    if (directories > 0)
                    {
                        return (directories == 1) ? string.Format("{0}, 1 directory", entriesLabel) :
                                                    string.Format("{0}, {1} directories", entriesLabel, directories);
                    }

                    return entriesLabel;
                }
                else if (directories > 0)
                {
                    return (directories == 1) ? "1 directory" :
                                                string.Format("{0} directories", directories);
                }
            }

            return "empty";
        }

        public bool Delete()
        {      
            Guid? parent = _storage.GetDirectory(_directoryId).Parent;
            if (!parent.HasValue)
                throw new Exception("It's not possible to delete the root directory");

            bool deleted = _storage.DeleteDirectory(_directoryId);

            if (deleted)
            {
                foreach (ObservableCollection<NodeInfo> collection in _subscribed)
                    collection.Clear();
                _subscribed.Clear();

                _contextManager.GetDirectoryContext(parent.Value).OnItemDeleted(_directoryId);
            }           

            return deleted;
        }

        public void OnItemDeleted(Guid id)
        {
            foreach (ObservableCollection<NodeInfo> collection in _subscribed)
            {                
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i].Id == id)
                    {
                        collection.RemoveAt(i);
                        break;
                    }                    
                }
            }

            UpdateLabelInParent();
        }
    }
}
