using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal;
using ValleyAuthenticator.Storage.Internal.Model;
using ValleyAuthenticator.Storage.Utils;

namespace ValleyAuthenticator.Storage.Impl
{
    /// <summary>
    /// Directory context
    /// </summary>
    internal class DirectoryContext : IDirectoryContext
    {
        /// <summary>
        /// Gets the display name of the type of the node
        /// </summary>
        public string TypeDisplayName { get; } = "Directory";

        /// <summary>
        /// Gets if the node still exists
        /// </summary>
        public bool Exists => _storage.DirectoryExists(_directoryId);

        /// <summary>
        /// Gets if the directory is the root directory
        /// </summary>
        public bool IsRoot { get; private set; }

        /// <summary>
        /// Gets or sets the name of the directory
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(value));

                _storage.RenameDirectory(_directoryId, value);
                _name = value;                
            }
        }

        // Private fields
        private readonly InternalStorageManager _storage;
        private readonly ContextManager _contextManager;
        private Guid _directoryId;
        private ObservableCollection<NodeInfo> _collection;
        private string _name;

        // Private constants
        private const string IMAGE_OTP_ENTRY = "key.png";
        private const string IMAGE_DIRECTORY = "folder.png";
        
        public DirectoryContext(InternalStorageManager storage, ContextManager contextManager, Guid directoryId)
        {
            // Input validation
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));
            if (contextManager == null)
                throw new ArgumentNullException(nameof(contextManager));
            if (directoryId == Guid.Empty)
                throw new ArgumentException(nameof(directoryId));

            // Set private fields
            _storage = storage;
            _contextManager = contextManager;
            _directoryId = directoryId;
            _collection = null;

            // Query additional information about the directory from the storage
            InternalDirectoryData data = _storage.GetDirectory(directoryId);
            _name = data.Name;
            IsRoot = !_storage.GetDirectory(directoryId).Parent.HasValue;
        }

        public ISearchContext CreateSearchContext()
            => _contextManager.CreateSearchContext(_directoryId);

        public IOtpFormContext CreateAddFormContext()
            => new AddOtpFormContext(this);

        public ObservableCollection<NodeInfo> ListAndSubscribe()
        {
            if (_collection == null)
            {
                List<NodeInfo> initial = new List<NodeInfo>();

                InternalDirectoryData target = _storage.GetDirectory(_directoryId);

                foreach (InternalDirectoryData directory in target.Directories)
                {
                    IDirectoryContext context = _contextManager.GetDirectoryContext(directory.Id);

                    string detail = DisplayUtilities.FormatDirectoryLabel(directory);

                    initial.Add(new NodeInfo(context, directory.Id, directory.Name, detail, IMAGE_DIRECTORY));
                }

                foreach (InternalOtpEntryData entry in target.OtpEntries)
                {
                    IOtpEntryContext context = _contextManager.GetOtpEntryContext(entry.Id);

                    initial.Add(new NodeInfo(context, entry.Id, entry.Data.Issuer, entry.Data.Label, IMAGE_OTP_ENTRY));
                }

                _collection = new ObservableCollection<NodeInfo>(initial);
            }

            return _collection;
        }

        public Guid AddEntry(OtpData data)
        {
            Guid id = _storage.AddEntry(_directoryId, data);

            IOtpEntryContext context = _contextManager.GetOtpEntryContext(id);

            NodeInfo node = new NodeInfo(context, id, data.Issuer, data.Label, IMAGE_OTP_ENTRY);

            _collection.Add(node);

            return id;
        }
   
        public Guid AddDirectory(string name)
        {
            Guid id = _storage.AddDirectory(_directoryId, name);

            IDirectoryContext context = _contextManager.GetDirectoryContext(id);

            string detail = DisplayUtilities.FormatDirectoryLabel();

            NodeInfo node = new NodeInfo(context, id, name, detail, IMAGE_DIRECTORY);

            // Insert the new directory after the last directory in the collection (before any entries).
            // The directories are always at the top of the list.
            int last = 0;
            while (last < _collection.Count && _collection[last].Context is IDirectoryContext)
                last++;
            _collection.Insert(last, node);            

            return id;
        }        

        public bool Delete()
        {      
            Guid? parent = _storage.GetDirectory(_directoryId).Parent;
            if (!parent.HasValue)
                throw new Exception("It's not possible to delete the root directory");

            bool deleted = _storage.DeleteDirectory(_directoryId);

            if (deleted)
            {
                if (_collection != null)
                {
                    _collection.Clear();
                    _collection = null;
                }

                _contextManager.GetDirectoryContext(parent.Value).OnItemDeleted(_directoryId);
            }           

            return deleted;
        }

        public void OnItemDeleted(Guid id)
        {
            for (int i = 0; i < _collection.Count; i++)
            {
                if (_collection[i].Id == id)
                {
                    _collection.RemoveAt(i);
                    break;
                }
            }
        }

        public string GetDetailLabel()
        {
            InternalDirectoryData directory = _storage.GetDirectory(_directoryId);
            return DisplayUtilities.FormatDirectoryLabel(directory);
        }

        public void Validate()
        {
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

        private void AddMissingItems()
        {
            HashSet<Guid> inCollection = new HashSet<Guid>();
            foreach (NodeInfo node in _collection)
                inCollection.Add(node.Id);

            InternalDirectoryData target = _storage.GetDirectory(_directoryId);

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

        public string Export(ExportFormat format)
            => ExportHelper.ExportDirectory(_storage.GetDirectory(_directoryId), format);

        public bool TryImport(ExportFormat format, string data, bool multiple)
        {            
            if (ExportHelper.TryImport(_storage, _directoryId, format, data, multiple))
            {
                AddMissingItems();
                return true;
            }
            return false;
        }            
    }
}
