using System;
using ValleyAuthenticator.Storage.Abstract;

namespace ValleyAuthenticator.Storage
{
    public interface IAuthenticatorStorage
    {
        IDirectoryContext GetRootDirectoryContext();     
    }
}
