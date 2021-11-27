using System;

namespace ValleyAuthenticator.Storage.Models
{
    public sealed class AuthNodeInfo
    {
        public Guid Id { get; private set; }

        public Guid Parent { get; private set; }

        public string Name { get; private set; }

        public string Detail => Type.ToString();

        public AuthNodeType Type { get; private set; }

        public AuthNodeInfo(Guid id, Guid parent, string name, AuthNodeType type)
        {
            Id = id;
            Parent = parent;
            Name = name;
            Type = type;
        }
    }
}
