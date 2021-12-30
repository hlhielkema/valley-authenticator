using System;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Impl
{
    internal class EditExistingOtpFormContext : IOtpFormContext
    {
        public string SubmitText => "Update";

        private readonly IOtpEntryContext _entryContext;

        public EditExistingOtpFormContext(IOtpEntryContext entryContext)
        {
            // Set private fields
            _entryContext = entryContext ?? throw new ArgumentNullException(nameof(entryContext));
        }
       
        public OtpData GetDefault()
            => _entryContext.OtpData;

        public void Set(OtpData data)
            => _entryContext.OtpData = data;
    }
}
