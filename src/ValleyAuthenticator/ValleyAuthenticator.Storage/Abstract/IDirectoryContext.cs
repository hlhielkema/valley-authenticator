using System;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Abstract
{
    /// <summary>
    /// Directory context
    /// </summary>
    public interface IDirectoryContext : INodeContext
    {
        /// <summary>
        /// Gets if the directory is the root directory
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// Gets or sets the name of the directory
        /// </summary>
        string Name { get; set; }

        ISearchContext CreateSearchContext();

        IOtpFormContext CreateAddFormContext();

        ObservableCollection<NodeInfo> ListAndSubscribe();
        
        Guid AddEntry(OtpData data);

        Guid AddDirectory(string name);
        
        string GetDetailLabel();

        void Validate();

        bool TryImport(ExportFormat format, string data, bool multiple);
    }
}
