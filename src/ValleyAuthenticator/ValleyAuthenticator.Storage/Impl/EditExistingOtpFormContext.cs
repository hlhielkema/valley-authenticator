using System;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Impl
{
    internal class EditExistingOtpFormContext : IOtpFormContext
    {
        public string SubmitText => "Update";

        private IOtpEntryContext _entryContext;

        public EditExistingOtpFormContext(IOtpEntryContext entryContext)
        {
            // Input validation
            if (entryContext == null)
                throw new ArgumentNullException(nameof(entryContext));

            _entryContext = entryContext;
        }
       
        public OtpData GetDefault()
            => _entryContext.GetOtpData();

        public void Set(OtpData data)
        {
            _entryContext.SetOtpData(data);
        }
    }

    internal class AddOtpFormContext : IOtpFormContext
    {
        public string SubmitText => "Create";

        private IDirectoryContext _directoryContext;

        public AddOtpFormContext(IDirectoryContext directoryContext)
        {
            // Input validation
            if (directoryContext == null)
                throw new ArgumentNullException(nameof(directoryContext));

            _directoryContext = directoryContext;
        }

        public OtpData GetDefault()
        {
            return OtpData.Default;
        }

        public void Set(OtpData data)
        {
            _directoryContext.AddEntry(data);
        }
    }
}
