using System;
using System.Collections.Generic;
using ValleyAuthenticator.Storage.Info;
using ValleyAuthenticator.Storage.Internal.Model;

namespace ValleyAuthenticator.Storage.Internal
{
    internal class InternalStorageManager
    {
        private readonly InternalDirectoryData _root;
        private readonly Dictionary<Guid, InternalDirectoryData> _directoryLookup;
        private readonly Dictionary<Guid, InternalOtpEntryData> _entryLookup;

        public Guid RootId => _root.Id;

        public InternalStorageManager()
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

        public InternalOtpEntryData GetOtpEntry(Guid entryId)
        {
            return _entryLookup[entryId];
        }

        public InternalDirectoryData GetDirectory(Guid directoryId)
        {
            return _directoryLookup[directoryId];
        }

        public Guid AddDirectory(Guid directoryId, string name)
        {
            Guid id = Guid.NewGuid();

            InternalDirectoryData target = _directoryLookup[directoryId];

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

        public Guid AddEntry(Guid directoryId, OtpData data)
        {
            Guid id = Guid.NewGuid();

            InternalDirectoryData target = _directoryLookup[directoryId];            

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

        public bool DeleteDirectory(Guid directoryId)
        {
            if (_directoryLookup.TryGetValue(directoryId, out InternalDirectoryData data))
            {
                if (!data.Parent.HasValue)
                    throw new Exception("Root cannot be deleted");

                InternalDirectoryData parentDirectory = _directoryLookup[data.Parent.Value];
                
                if (!parentDirectory.Directories.Remove(data))
                    throw new Exception("Directory not found in directory");
                
                _directoryLookup.Remove(directoryId);
                
                RebuildIndex(); // todo
                
                return true;
            }
            return false;
        }

        public bool DeleteOtpEntry(Guid entryId)
        {
            if (_entryLookup.TryGetValue(entryId, out InternalOtpEntryData data))
            {
                InternalDirectoryData parentDirectory = _directoryLookup[data.Parent];
                if (!parentDirectory.OtpEntries.Remove(data))
                    throw new Exception("Entry not found in directory");
                
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
            Guid favoriteId = AddDirectory(RootId, "Favorites");
            AddDirectory(RootId, "Work");
            AddDirectory(RootId, "Private");
            AddDirectory(RootId, "Side projects");
            AddEntry(RootId, new OtpData(OtpType.Totp, "admin", "345h7", "Google"));
            AddEntry(RootId, new OtpData(OtpType.Totp, "admin", "345h7", "Microsoft"));
            AddEntry(RootId, new OtpData(OtpType.Totp, "admin@gmail.com", "123456789", "Gmail (invalid)"));
            AddEntry(favoriteId, new OtpData(OtpType.Totp, "admin", "345h7", "Ydentic"));
        }
    }
}
