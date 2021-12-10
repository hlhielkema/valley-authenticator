using System;
using System.Collections.Generic;
using ValleyAuthenticator.Storage.Abstract;

namespace ValleyAuthenticator.Storage.Info
{
    public sealed class NodeInfo
    {        
        public string Name { get; private set; }

        public string Detail { get; private set; }
       
        public INodeContext Context { get; private set; }

        public NodeType Type { get; private set; }

        // Private fields
        private Guid _id;        

        public string Image
        {
            get
            {
                switch (Type)
                {
                    case NodeType.OtpEntry:
                        return "key.png";
                    default:
                        return "folder.png";
                }
            }
        }

        internal NodeInfo(INodeContext context, Guid id, Guid parent, string name, string detail, NodeType type)
        {
            Context = context;
            _id = id;
            Name = name;
            Detail = detail;
            Type = type;
        }

        public override bool Equals(object obj)
            => obj is NodeInfo info && _id.Equals(info._id);

        public override int GetHashCode()
            => _id.GetHashCode();
    }
}
