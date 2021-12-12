using System;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface ISearchContext : IDisposable
    {
        ObservableCollection<NodeInfo> ListAndSubscribe();

        void Validate();

        void Update(string searchQuery);
    }
}
