using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ValleyAuthenticator.Storage.Abstract;

namespace ValleyAuthenticator.Storage.Info
{
    public sealed class NodeInfo : INotifyPropertyChanged
    {        
        public Guid Id { get; private set; }

        public string Name { get; private set; }
        
        public string Detail
        {
            internal set
            {
                if (_detail != value)
                {
                    _detail = value;
                    OnPropertyChanged("Detail");
                }
            }
            get
            {
                return _detail;
            }
        }

        public INodeContext Context { get; private set; }

        public NodeType Type { get; private set; }
        
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

        private string _detail;

        internal NodeInfo(INodeContext context, Guid id, string name, string detail, NodeType type)
        {
            _detail = detail;

            Context = context;
            Id = id;
            Name = name;            
            Type = type;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object obj)
            => obj is NodeInfo info && Id.Equals(info.Id);

        public override int GetHashCode()
            => Id.GetHashCode();
    }
}
