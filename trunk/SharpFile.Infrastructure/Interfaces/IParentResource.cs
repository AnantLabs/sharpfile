namespace SharpFile.Infrastructure.Interfaces {
    public interface IParentResource : IResource {
        System.IO.DriveType DriveType { get; }
        bool IsReady { get; }
        long FreeSpace { get; }
    }
}