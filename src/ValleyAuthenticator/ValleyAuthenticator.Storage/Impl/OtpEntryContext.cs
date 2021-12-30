using System;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal;
using ValleyAuthenticator.Storage.Internal.Model;

namespace ValleyAuthenticator.Storage.Impl
{
    /// <summary>
    /// OTP entry context
    /// </summary>
    internal class OtpEntryContext : IOtpEntryContext
    {
        /// <summary>
        /// Gets the display name of the type of the node
        /// </summary>
        public string TypeDisplayName { get; } = "OTP entry";

        /// <summary>
        /// Gets if the node still exists
        /// </summary>
        public bool Exists => _storage.OtpEntryExists(_entryId);

        /// <summary>
        /// Gets or sets the OTP data of the entry
        /// </summary>
        public OtpData OtpData
        {
            get => new OtpData(_storage.GetOtpEntry(_entryId).Data);
            set => _storage.GetOtpEntry(_entryId).Data = value.AsData();
        }

        // Private fields
        private readonly InternalStorageManager _storage;
        private readonly ContextManager _contextManager;
        private readonly Guid _entryId;

        // Private constants
        private const string IMAGE_OTP_ENTRY = "key.png";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="contextManager"></param>
        /// <param name="entryId"></param>
        public OtpEntryContext(InternalStorageManager storage, ContextManager contextManager, Guid entryId)
        {
            // Input validation
            if (entryId == Guid.Empty)
                throw new ArgumentException(nameof(entryId));

            // Set private fields
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _contextManager = contextManager ?? throw new ArgumentNullException(nameof(contextManager));
            _entryId = entryId;
        }

        public IOtpFormContext CreateEditFormContext()
            => new EditExistingOtpFormContext(this);

        /// <summary>
        /// Delete the OTP entry
        /// </summary>
        /// <returns>true=deleted;false=not found</returns>
        public bool Delete()
        {
            if (_storage.OtpEntryExists(_entryId))
            {
                Guid parent = _storage.GetOtpEntry(_entryId).Parent;

                _storage.DeleteOtpEntry(_entryId);

                _contextManager.GetDirectoryContext(parent).OnItemDeleted(_entryId);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Export the node to a given format
        /// </summary>
        /// <param name="format">export format</param>
        /// <returns>export data</returns>
        public string Export(ExportFormat format)
            => ExportHelper.ExportEntry(_storage.GetOtpEntry(_entryId), format);

        /// <summary>
        /// Get information about the node.
        /// </summary>
        /// <returns>node information</returns>
        public NodeInfo GetInfo()
        {
            InternalOtpEntryData entry = _storage.GetOtpEntry(_entryId);

            return new NodeInfo(this, entry.Id, entry.Data.Issuer, entry.Data.Label, IMAGE_OTP_ENTRY);
        }
    }
}
