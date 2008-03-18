using System;

namespace SharpFile.Infrastructure {
    public interface IChildResource {
        string FullName { get; }
        string Name { get; }
        DateTime LastWriteTime { get; }
        DateTime LastAccessTime { get; }
        DateTime CreationTime { get; }
        long Size { get; }

        //void Execute();
        void Copy(string destination);
    }
}