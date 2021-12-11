﻿using System;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage.Info;

namespace ValleyAuthenticator.Storage.Abstract
{
    public interface IDirectoryContext : INodeContext
    {
        ObservableCollection<NodeInfo> ListAndSubscribe();

        void Unsubscribe(ObservableCollection<NodeInfo> collection);

        Guid AddEntry(OtpData data);

        Guid AddDirectory(string name);
    }
}
