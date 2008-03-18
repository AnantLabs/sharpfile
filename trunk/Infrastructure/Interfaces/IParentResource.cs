namespace SharpFile.Infrastructure {
    public interface IParentResource : IResource {
        System.IO.DriveType DriveType { get; }
        bool IsReady { get; }
    }
}