using System;
using System.Collections.Generic;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Impl;
using ValleyAuthenticator.Storage.Info;
using ValleyAuthenticator.Storage.Internal.Model;

namespace ValleyAuthenticator.Storage
{
    public sealed class AuthenticatorStorage : IAuthenticatorStorage
    {
        public static IAuthenticatorStorage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthenticatorStorage();
                    _instance.AddTestData();
                }
                return _instance;
            }
        }

        private static AuthenticatorStorage _instance;

        private readonly InternalDirectoryData _root;
        private readonly Dictionary<Guid, InternalDirectoryData> _directoryLookup;
        private readonly Dictionary<Guid, InternalOtpEntryData> _entryLookup;

        private AuthenticatorStorage()
        {
            _root = new InternalDirectoryData()
            {
                Id = Guid.NewGuid(),
                Name = "root",
                Parent = null,
                Directories = new List<InternalDirectoryData>(),
                OtpEntries = new List<InternalOtpEntryData>()
            };
            _directoryLookup = new Dictionary<Guid, InternalDirectoryData>();
            _entryLookup = new Dictionary<Guid, InternalOtpEntryData>();
            _directoryLookup.Add(_root.Id, _root);
        }

        public IDirectoryContext GetRootDirectoryContext()
            => new DirectoryContext(this, null);

        public IDirectoryContext GetDirectoryContext(Guid? directoryId)
            => new DirectoryContext(this, directoryId);

        public IOtpEntryContext GetOtpEntryContext(Guid entryId)
            => new OtpEntryContext(this, entryId);

        public Guid AddDirectory(Guid? directoryId, string name)
        {
            Guid id = Guid.NewGuid();

            InternalDirectoryData target = _root;
            if (directoryId.HasValue)
                target = _directoryLookup[directoryId.Value];

            InternalDirectoryData directory = new InternalDirectoryData()
            {
                Id = id,
                Name = name,
                Parent = target.Id,
                Directories = new List<InternalDirectoryData>(),
                OtpEntries = new List<InternalOtpEntryData>()
            };

            target.Directories.Add(directory);
            _directoryLookup.Add(id, directory);


            return id;
        }

        public Guid AddEntry(Guid? directoryId, OtpData data)
        {
            Guid id = Guid.NewGuid();

            InternalDirectoryData target = _root;
            if (directoryId.HasValue)
                target = _directoryLookup[directoryId.Value];

            InternalOtpEntryData entry = new InternalOtpEntryData()
            {
                Id = id,
                Parent = target.Id,
                Data = data.AsData(),
            };

            target.OtpEntries.Add(entry);
            _entryLookup.Add(id, entry);

            return id;
        }

        public List<NodeInfo> GetForDirectory(Guid? directoryId)
        {
            InternalDirectoryData target = _root;
            if (directoryId.HasValue)
                target = _directoryLookup[directoryId.Value];

            List<NodeInfo> result = new List<NodeInfo>();

            foreach (InternalDirectoryData directory in target.Directories)
                result.Add(new NodeInfo(GetDirectoryContext(directory.Id), directory.Id, target.Id, directory.Name, string.Format("{0} entries, {1} directories", directory.OtpEntries.Count, directory.Directories.Count), NodeType.Directory));
            foreach (InternalOtpEntryData entry in target.OtpEntries)
                result.Add(new NodeInfo(GetOtpEntryContext(entry.Id), entry.Id, target.Id, entry.Data.Issuer, entry.Data.Label, NodeType.OtpEntry));

            return result;
        }

        public OtpData GetEntryOtpData(Guid entryId)
        {
            InternalOtpEntryData data = _entryLookup[entryId];
            return new OtpData(data.Data);
        }

        public bool DeleteDirectory(Guid directoryId)
        {
            if (_directoryLookup.TryGetValue(directoryId, out InternalDirectoryData data))
            {
                if (!data.Parent.HasValue)
                    throw new Exception("Root cannot be deleted");
                InternalDirectoryData parentDirectory = _directoryLookup[data.Parent.Value];
                if (!parentDirectory.Directories.Remove(data))
                    throw new Exception("Entry found in directory");
                _directoryLookup.Remove(directoryId);
                RebuildIndex();
                return true;
            }
            return false;
        }

        public bool DeleteEntry(Guid entryId)
        {
            if (_entryLookup.TryGetValue(entryId, out InternalOtpEntryData data))
            {
                InternalDirectoryData parentDirectory = _directoryLookup[data.Parent];
                if (!parentDirectory.OtpEntries.Remove(data))
                    throw new Exception("Entry found in directory");
                _entryLookup.Remove(entryId);
                return true;
            }
            return false;
        }

        private void RebuildIndex()
        {
            _directoryLookup.Clear();
            _entryLookup.Clear();
            IndexDirectory(_root);
        }

        private void IndexDirectory(InternalDirectoryData directory)
        {
            _directoryLookup.Add(directory.Id, directory);
            foreach (InternalOtpEntryData entry in directory.OtpEntries)
                _entryLookup.Add(entry.Id, entry);
            foreach (InternalDirectoryData childDirectory in directory.Directories)
                IndexDirectory(childDirectory);
        }        

        public void AddTestData()
        {
            Guid favoriteId = AddDirectory(null, "Favorites");
            AddDirectory(null, "Work");
            AddDirectory(null, "Private");
            AddDirectory(null, "Side projects");
            AddEntry(null, new OtpData(OtpType.Totp, "admin", "345h7", "Google"));
            AddEntry(null, new OtpData(OtpType.Totp, "admin", "345h7", "Microsoft"));
            AddEntry(null, new OtpData(OtpType.Totp, "admin@gmail.com", "123456789", "Gmail (invalid)"));
            AddEntry(favoriteId, new OtpData(OtpType.Totp, "admin", "345h7", "Ydentic"));
        }
    }
}
