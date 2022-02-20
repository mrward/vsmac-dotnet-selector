
using System;
using System.Diagnostics;
using MonoDevelop.Core;

namespace Microsoft.VisualStudioMac.DotNetSelector
{
    class DotNetLocation : IComparable<DotNetLocation>, IComparable, IEquatable<DotNetLocation>
    {
        public DotNetLocation(FilePath rootDirectory)
            : this(rootDirectory.FileName, rootDirectory)
        {
        }

        public DotNetLocation(string name, FilePath rootDirectory)
        {
            Name = name;
            RootDirectory = rootDirectory;
        }

        public FilePath RootDirectory { get; }
        public string Name { get; }

        public int CompareTo(DotNetLocation? other)
        {
            if (other == null)
            {
                return -1;
            }
            return RootDirectory.CompareTo(other.RootDirectory);
        }

        public int CompareTo(object? obj)
        {
            return CompareTo(obj as DotNetLocation);
        }

        public bool Equals(DotNetLocation? other)
        {
            if (other == null)
            {
                return false;
            }

            return RootDirectory == other.RootDirectory;
        }

        public override int GetHashCode()
        {
            return RootDirectory.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} {RootDirectory}";
        }
    }
}

