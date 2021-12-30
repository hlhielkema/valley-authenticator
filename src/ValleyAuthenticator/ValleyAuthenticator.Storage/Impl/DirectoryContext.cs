using System;
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
                ValidateParent();
            }
        }
        
        // Private fields
        private readonly InternalStorageManager _storage;
        private readonly ContextManager _contextManager;
        private ObservableCollectionManager _collectionManager;
        private Guid _directoryId;        
        private string _name;
   
        public DirectoryContext(InternalStorageManager storage, ContextManager contextManager, Guid directoryId)
        {
            if (directoryId == Guid.Empty)
                throw new ArgumentException(nameof(directoryId));

            // Set private fields
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _contextManager = contextManager ?? throw new ArgumentNullException(nameof(contextManager));
            _directoryId = directoryId;

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
            if (_collectionManager == null)
                _collectionManager = ObservableCollectionManager.CreateForDirectory(_storage, _contextManager, _directoryId);

            return _collectionManager.Collection;
        }

        public Guid AddEntry(OtpData data)
        {
            Guid id = _storage.AddEntry(_directoryId, data);
            _collectionManager?.AddEntry(id);
            return id;
        }
   
        public Guid AddDirectory(string name)
        {
            Guid id = _storage.AddDirectory(_directoryId, name);
            _collectionManager?.AddDirectory(id);       
            return id;
        }        

        public bool Delete()
        {
            if (_storage.DirectoryExists(_directoryId))
            {
                Guid? parent = _storage.GetDirectory(_directoryId).Parent;
                if (!parent.HasValue)
                    throw new Exception("It's not possible to delete the root directory");

                _storage.DeleteDirectory(_directoryId);

                if (_collectionManager != null)
                {
                    _collectionManager.Clear();
                    _collectionManager = null;
                }

                _contextManager.GetDirectoryContext(parent.Value).OnItemDeleted(_directoryId);

                return true;
            }
            return false;
        }

        public void OnItemDeleted(Guid id)
            => _collectionManager?.RemoveById(id);

        public string GetDetailLabel()
        {
            InternalDirectoryData directory = _storage.GetDirectory(_directoryId);
            return DisplayUtilities.FormatDirectoryLabel(directory);
        }

        public void Validate()
            => _collectionManager?.Validate();

        /// <summary>
        /// Validate the parent directory
        /// </summary>
        private void ValidateParent()
        {
            Guid? parent = _storage.GetDirectory(_directoryId).Parent;
            if (parent.HasValue)
                _contextManager.GetDirectoryContext(parent.Value).Validate();
        }

        public string Export(ExportFormat format)
            => ExportHelper.ExportDirectory(_storage.GetDirectory(_directoryId), format);

        public bool TryImport(ExportFormat format, string data, bool multiple)
        {            
            if (ExportHelper.TryImport(_storage, _directoryId, format, data, multiple))
            {
                _collectionManager?.AddMissingNodesForDirectory(_directoryId);
                return true;
            }
            return false;
        }            
    }
}
