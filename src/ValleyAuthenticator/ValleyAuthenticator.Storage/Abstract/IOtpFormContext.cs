using System;
using System.Collections.Generic;
using System.Text;
using ValleyAuthenticator.Storage.Abstract.Models;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface IOtpFormContext
    {
        string SubmitText { get; }

        OtpData GetDefault();

        void Set(OtpData data);
    }
}
