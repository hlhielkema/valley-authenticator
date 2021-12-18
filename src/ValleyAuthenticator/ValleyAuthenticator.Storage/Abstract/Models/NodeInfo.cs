using System;
using System.ComponentModel;

namespace ValleyAuthenticator.Storage.Abstract.Models
{
    public sealed class NodeInfo : INotifyPropertyChanged
    {        
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Image { get; private set; }

        public string Detail { get; private set; }
       
        public INodeContext Context { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        internal NodeInfo(INodeContext context, Guid id, string name, string detail, string image)
        {
            Context = context;
            Id = id;
            Name = name;
            Image = image;
            Detail = detail;
        }

        internal void UpdateDetail(string value)
        {
            if (Detail != value)
            {
                Detail = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Detail"));
            }
        }        
     
        public override bool Equals(object obj)
            => obj is NodeInfo info && Id.Equals(info.Id);

        public override int GetHashCode()
            => Id.GetHashCode();
    }
}
