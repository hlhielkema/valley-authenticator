using System;
using System.Collections.Generic;
using System.Text;

namespace ValleyAuthenticator.Storage.Abstract
{
    internal interface IPersistentStorage
    {
        bool TryRead(out string data);

        void Write(string data);
    }
}
