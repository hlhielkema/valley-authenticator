using ValleyAuthenticator.Storage.Info;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface IOtpEntryContext : INodeContext
    {
        OtpData GetOtpData();
    }
}
