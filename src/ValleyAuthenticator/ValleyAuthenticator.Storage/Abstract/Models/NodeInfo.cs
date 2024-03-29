﻿using System;
using System.ComponentModel;

namespace ValleyAuthenticator.Storage.Abstract.Models
{
    public sealed class NodeInfo : INotifyPropertyChanged
    {        
        public Guid Id { get; private set; }

        public string Name
        {
            get => _name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(value));

                if (_name != value)
                {
                    _name = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public string Detail
        {
            get => _detail;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(value));

                if (_detail != value)
                {
                    _detail = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Detail"));
                }
            }
        }

        public string Image { get; private set; }
       
        public INodeContext Context { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // Private fields
        private string _name;
        private string _detail;

        internal NodeInfo(INodeContext context, Guid id, string name, string detail, string image)
        {
            Context = context;
            Id = id;            
            Image = image;
            _name = name;
            _detail = detail;
        }       

        public void Refresh()
        {
            // TODO
            NodeInfo node = Context.GetInfo();
            Name = node.Name;
            Detail = node.Detail;
        }

        public override bool Equals(object obj)
            => obj is NodeInfo info && Id.Equals(info.Id);

        public override int GetHashCode()
            => Id.GetHashCode();
    }
}
