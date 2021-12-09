using System;
using System.Collections.Generic;

namespace ValleyAuthenticator.Storage.Models
{
    public sealed class AuthNodeInfo
    {
        public Guid Id { get; private set; }

        public Guid Parent { get; private set; }

        public string Name { get; private set; }

        public string Detail { get; private set; }

        public AuthNodeType Type { get; private set; }

        public string Image
        {
            get
            {
                switch (Type)
                {
                    case AuthNodeType.Entry:
                        return "key.png";
                    default:
                        return "folder.png";
                }
            }
        }

        public AuthNodeInfo(Guid id, Guid parent, string name, string detail, AuthNodeType type)
        {
            Id = id;
            Parent = parent;
            Name = name;
            Detail = detail;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return obj is AuthNodeInfo info &&
                   Id.Equals(info.Id) &&
                   Parent.Equals(info.Parent) &&
                   Name == info.Name &&
                   Detail == info.Detail &&
                   Type == info.Type;
        }

        public override int GetHashCode()
        {
            int hashCode = -1515344868;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Parent.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Detail);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }
    }
}
