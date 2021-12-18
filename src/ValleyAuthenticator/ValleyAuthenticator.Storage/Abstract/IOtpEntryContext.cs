using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface IOtpEntryContext : INodeContext
    {
        OtpData GetOtpData();

        void SetOtpData(OtpData data);

        IOtpFormContext CreateEditFormContext();
    }
}
