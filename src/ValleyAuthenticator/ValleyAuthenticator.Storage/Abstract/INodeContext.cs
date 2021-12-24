using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface INodeContext
    {        
        string TypeDisplayName { get; }

        bool Exists { get; }

        bool Delete();

        string Export(ExportFormat format);
    }
}
