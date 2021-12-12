namespace ValleyAuthenticator.Storage.Abstract
{
    public interface INodeContext
    {        
        string TypeDisplayName { get; }

        bool Exists { get; }

        bool Delete();        
    }
}
