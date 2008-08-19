using System;

namespace SharpFile.Infrastructure.Interfaces {
    public interface IChildResource : IResource {
        IResource Parent { get; }
        DateTime LastWriteTime { get; }
        DateTime LastAccessTime { get; }
        DateTime CreationTime { get; }

        void Copy(string destination, bool overwrite);
        void Move(string destination);
    }
}