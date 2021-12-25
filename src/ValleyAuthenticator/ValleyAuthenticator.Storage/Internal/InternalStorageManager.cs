using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Internal.Model;
using ValleyAuthenticator.Storage.Internal.Models;

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

        public bool OtpEntryExists(Guid entryId)
            => _entryLookup.ContainsKey(entryId);

        public bool DirectoryExists(Guid directoryId)
            => _directoryLookup.ContainsKey(directoryId);

        public SearchResults Search(Guid directoryId, string searchQuery)
        {
            InternalDirectoryData directory = _directoryLookup[directoryId];
            SearchResults results = new SearchResults();
            Search(directory, searchQuery.ToLower(), results);
            return results;
        }

        private void Search(InternalDirectoryData searchDirectory, string searchQuery, SearchResults results)
        {           
            foreach (InternalOtpEntryData entry in searchDirectory.OtpEntries)
            {
                if (entry.Data.Issuer.ToLower().Contains(searchQuery) ||
                    entry.Data.Label.ToLower().Contains(searchQuery))
                {
                    results.OtpEntries.Add(entry);
                }
            }

            foreach (InternalDirectoryData directory in searchDirectory.Directories)
            {
                if (directory.Name.ToLower().Contains(searchQuery))
                    results.Directories.Add(directory);

                Search(directory, searchQuery, results);
            }
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

        public Guid AddOrGetDirectory(Guid directoryId, string name)
        {
            InternalDirectoryData target = _directoryLookup[directoryId];
            InternalDirectoryData match = target.Directories.FirstOrDefault(x => x.Name == name);
            if (match == null)
                return AddDirectory(directoryId, name);
            else
                return match.Id;
        }

        public Guid AddOrGetDirectory(Guid directoryId, string[] names, int offset = 0)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            int d = names.Length - offset;
            if (d < 1)
                throw new ArgumentException(nameof(names));

            if (d == 1)
                return AddOrGetDirectory(directoryId, names[offset]);
            else // d > 1
            {
                Guid childId = AddOrGetDirectory(directoryId, names[offset]);
                return AddOrGetDirectory(childId, names, offset + 1);
            }
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

        public bool RenameDirectory(Guid directoryId, string name)
        {
            if (_directoryLookup.TryGetValue(directoryId, out InternalDirectoryData data))
            {                
                data.Name = name;

                Save();

                return true;
            }

            return false;
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
