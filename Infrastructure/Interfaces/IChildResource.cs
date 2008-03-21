using System;

namespace SharpFile.Infrastructure {
    public interface IChildResource : IResource {      
        DateTime LastWriteTime { get; }
        DateTime LastAccessTime { get; }
        DateTime CreationTime { get; }

        void Copy(string destination, bool overwrite);
        void Move(string destination);
    }
}