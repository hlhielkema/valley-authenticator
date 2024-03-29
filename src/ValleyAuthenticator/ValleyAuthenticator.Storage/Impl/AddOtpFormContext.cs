﻿using System;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Impl
{
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
            => OtpData.Default;

        public void Set(OtpData data)
            => _directoryContext.AddEntry(data);
    }
}
