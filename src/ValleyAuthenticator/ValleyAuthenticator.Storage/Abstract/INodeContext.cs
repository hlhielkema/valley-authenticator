using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface INodeContext
    {        
        /// <summary>
        /// Gets the display name of the type of the node
        /// </summary>
        string TypeDisplayName { get; }

        /// <summary>
        /// Gets if the node still exists
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Delete the node
        /// </summary>
        /// <returns>true=deleted; false=not found</returns>
        bool Delete();

        /// <summary>
        /// Export the node to a given format
        /// </summary>
        /// <param name="format">export format</param>
        /// <returns>export data</returns>
        string Export(ExportFormat format);

        /// <summary>
        /// Get information about the node.
        /// </summary>
        /// <returns>node information</returns>
        NodeInfo GetInfo();
    }
}
