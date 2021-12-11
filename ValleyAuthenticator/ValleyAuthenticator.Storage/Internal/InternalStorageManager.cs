using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal.Model;

namespace ValleyAuthenticator.Storage.Internal
{
    internal class InternalStorageManager
    {
        private readonly IPersistentStorage _persistentStorage;        
        private readonly Dictionary<Guid, InternalDirectoryData> _directoryLookup;
        private readonly Dictionary<Guid, InternalOtpEntryData> _entryLookup;
        private InternalDirectoryData _root;
        private bool _suspendSave;

        public Guid RootId => _root.Id;

        public InternalStorageManager(IPersistentStorage persistentStorage)
        {
            _persistentStorage = persistentStorage;
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
            _suspendSave = false;
        }

        public bool Load()
        {
            if (_persistentStorage.TryRead(out string json))
            {
                _root = JsonConvert.DeserializeObject<InternalDirectoryData>(json);

                RebuildParentStructure(_root);

                RebuildIndex();

                return true;
            }
            return false;
        }

        public void Save()
        {
            if (!_suspendSave)
            {
                string json = JsonConvert.SerializeObject(_root, Formatting.Indented);
                _persistentStorage.Write(json);
            }
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

            Save();

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

            Save();

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

                Save();

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

                Save();

                return true;
            }
            return false;
        }

        private void RebuildParentStructure(InternalDirectoryData target)
        {
            foreach (InternalOtpEntryData otpEntry in target.OtpEntries)
                otpEntry.Parent = target.Id;

            foreach (InternalDirectoryData directory in target.Directories)
            {
                directory.Parent = target.Id;
                RebuildParentStructure(directory);
            }
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
            _suspendSave = true;
            Guid favoriteId = AddDirectory(RootId, "Favorites");
            AddDirectory(RootId, "Work");
            AddDirectory(RootId, "Private");
            AddDirectory(RootId, "Side projects");
            AddEntry(RootId, new OtpData(OtpType.Totp, "admin", "345h7", "Google"));
            AddEntry(RootId, new OtpData(OtpType.Totp, "admin", "345h7", "Microsoft"));
            AddEntry(RootId, new OtpData(OtpType.Totp, "admin@gmail.com", "123456789", "Gmail (invalid)"));
            AddEntry(favoriteId, new OtpData(OtpType.Totp, "admin", "345h7", "Ydentic"));
            _suspendSave = false;
        }
    }
}
