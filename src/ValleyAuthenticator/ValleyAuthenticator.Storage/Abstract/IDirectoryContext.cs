using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface IDirectoryContext : INodeContext
    {
        ISearchContext CreateSearchContext();

        IOtpFormContext CreateAddFormContext();

        ObservableCollection<NodeInfo> ListAndSubscribe();
        
        Guid AddEntry(OtpData data);

        Guid AddDirectory(string name);

        void Rename(string name);

        string GetDetailLabel();

        void Validate();
    }
}
