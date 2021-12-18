using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ValleyAuthenticator.Storage.Abstract;

namespace ValleyAuthenticator.Storage.Common
{
    public class FileSystemStorage : IPersistentStorage
    {
        private string _path;

        public FileSystemStorage()
        {
            string name = "test2";
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), name);
        }

        public bool TryRead(out string data)
        {
            if (File.Exists(_path))
            {
                data = File.ReadAllText(_path);
                return true;
            }
            else
            {
                data = null;
                return false;
            }
        }

        public void Write(string data)
        {
            File.WriteAllText(_path, data);
        }
    }
}
