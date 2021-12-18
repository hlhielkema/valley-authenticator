using System;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface IDirectoryContext : INodeContext
    {
        bool IsRoot { get; }

        string Name { get; set; }

        ISearchContext CreateSearchContext();

        IOtpFormContext CreateAddFormContext();

        ObservableCollection<NodeInfo> ListAndSubscribe();
        
        Guid AddEntry(OtpData data);

        Guid AddDirectory(string name);
        
        string GetDetailLabel();

        void Validate();        
    }
}
