using System;
using System.Collections.Generic;
using System.Text;
using ValleyAuthenticator.Storage.Info;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface IOtpEntryContext : INodeContext
    {
        OtpData GetOtpData();
    }
}
