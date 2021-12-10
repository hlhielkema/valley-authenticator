using System;
using System.Collections.Generic;
using System.Text;
using ValleyAuthenticator.Storage.Info;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface IDirectoryContext : INodeContext
    {
        List<NodeInfo> GetChilds();

        Guid AddEntry(OtpData data);

        Guid AddDirectory(string name);        
    }
}
