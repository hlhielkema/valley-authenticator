using System;
using System.Collections.Generic;
using System.Text;
using ValleyAuthenticator.Storage.Models;

namespace ValleyAuthenticator.Storage
{
    public sealed class AuthenticatorStorage
    {
        public static AuthenticatorStorage Instance
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

        private readonly AuthDirectoryData _root;
        private readonly Dictionary<Guid, AuthDirectoryData> _directoryLookup;
        private readonly Dictionary<Guid, AuthEntryData> _entryLookup;

        private AuthenticatorStorage()
        {
            _root = new AuthDirectoryData()
            {
                Id = Guid.NewGuid(),
                Name = "root",
                Parent = null,
                Directories = new List<AuthDirectoryData>(),
                Entries = new List<AuthEntryData>()
            };
            _directoryLookup = new Dictionary<Guid, AuthDirectoryData>();
            _entryLookup = new Dictionary<Guid, AuthEntryData>();
            _directoryLookup.Add(_root.Id, _root);
        }

        public Guid AddDirectory(Guid? directoryId, string name)
        {
            Guid id = Guid.NewGuid();

            AuthDirectoryData target = _root;
            if (directoryId.HasValue)
                target = _directoryLookup[directoryId.Value];

            AuthDirectoryData directory = new AuthDirectoryData()
            {
                Id = id,
                Name = name,
                Parent = target.Id,
                Directories = new List<AuthDirectoryData>(),
                Entries = new List<AuthEntryData>()
            };

            target.Directories.Add(directory);
            _directoryLookup.Add(id, directory);


            return id;
        }

        public Guid AddEntry(Guid? directoryId, OtpData data)
        {
            Guid id = Guid.NewGuid();

            AuthDirectoryData target = _root;
            if (directoryId.HasValue)
                target = _directoryLookup[directoryId.Value];

            AuthEntryData entry = new AuthEntryData()
            {
                Id = id,
                Parent = target.Id,
                Data = data
            };

            target.Entries.Add(entry);
            _entryLookup.Add(id, entry);

            return id;
        }

        public List<AuthNodeInfo> GetForDirectory(Guid? directoryId)
        {
            AuthDirectoryData target = _root;
            if (directoryId.HasValue)
                target = _directoryLookup[directoryId.Value];

            List<AuthNodeInfo> result = new List<AuthNodeInfo>();

            foreach (AuthDirectoryData directory in target.Directories)
                result.Add(new AuthNodeInfo(directory.Id, target.Id, directory.Name, string.Format("{0} entries, {1} directories", directory.Entries.Count, directory.Directories.Count), AuthNodeType.Directory));
            foreach (AuthEntryData entry in target.Entries)
                result.Add(new AuthNodeInfo(entry.Id, target.Id, entry.Data.Issuer, entry.Data.Label, AuthNodeType.Entry));

            return result;
        }

        public AuthEntryInfo GetEntry(Guid entryId)
        {
            AuthEntryData data = _entryLookup[entryId];
            return new AuthEntryInfo(data.Id, data.Data);
        }

        public bool DeleteDirectory(Guid directoryId)
        {
            if (_directoryLookup.TryGetValue(directoryId, out AuthDirectoryData data))
            {
                if (!data.Parent.HasValue)
                    throw new Exception("Root cannot be deleted");
                AuthDirectoryData parentDirectory = _directoryLookup[data.Parent.Value];
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
            if (_entryLookup.TryGetValue(entryId, out AuthEntryData data))
            {
                AuthDirectoryData parentDirectory = _directoryLookup[data.Parent];
                if (!parentDirectory.Entries.Remove(data))
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

        private void IndexDirectory(AuthDirectoryData directory)
        {
            _directoryLookup.Add(directory.Id, directory);
            foreach (AuthEntryData entry in directory.Entries)
                _entryLookup.Add(entry.Id, entry);
            foreach (AuthDirectoryData childDirectory in directory.Directories)
                IndexDirectory(childDirectory);
        }        

        public void AddTestData()
        {
            Guid favoriteId = AddDirectory(null, "Favorites");
            AddDirectory(null, "Work");
            AddDirectory(null, "Private");
            AddDirectory(null, "Side projects");
            AddEntry(null, new OtpData(EntryType.Totp, "admin", "345h7", "Google"));
            AddEntry(null, new OtpData(EntryType.Totp, "admin", "345h7", "Microsoft"));
            AddEntry(null, new OtpData(EntryType.Totp, "admin@gmail.com", "123456789", "Gmail (invalid)"));
            AddEntry(favoriteId, new OtpData(EntryType.Totp, "admin", "345h7", "Ydentic"));
        }
    }
}
