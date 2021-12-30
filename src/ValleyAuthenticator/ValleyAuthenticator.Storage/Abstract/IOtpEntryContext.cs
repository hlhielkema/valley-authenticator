using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Abstract
{
    /// <summary>
    /// OTP entry context
    /// </summary>
    public interface IOtpEntryContext : INodeContext
    {
        /// <summary>
        /// Gets or sets the OTP data of the entry
        /// </summary>
        OtpData OtpData { get; set; }

        IOtpFormContext CreateEditFormContext();
    }
}
