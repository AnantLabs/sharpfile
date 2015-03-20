# Introduction #
In the course of working on SharpFile, I noticed some peculiar behavior in regards to retrieving a large number of files or folders using the standard GetFiles() or GetDirectories() from a DirectoryInfo or DriveInfo instance.

# Details #
The standard way to retrieve file information in the following:

```
DriveInfo driveInfo = new DriveInfo(@"c:\");
DirectoryInfo[] directoryInfos = driveInfo.GetDirectories();
FileInfo[] fileInfos = driveInfo.GetFiles();

foreach (DirectoryInfo directoryInfo in directoryInfos) {
  Console.WriteLine(directoryInfo.Name);
  Console.WriteLine(directoryInfo.LastWriteTime);
  Console.WriteLine(directoryInfo.LastAccessTime);
}

foreach (FileInfo fileInfo in fileInfos) {
  Console.WriteLine(fileInfo.Name);
  Console.WriteLine(fileInfo.LastWriteTime);
  Console.WriteLine(fileInfo.LastAccessTime);
}
```

However, while profiling my code I would notice large slowdowns when accessing a network drive wireless with ~200 folders. However, when I used Windows Explorer to access the drive everything was much faster.

After writing a small prototype benchmark, I discovered that it wasn't the retrieval that was so slow, but it was only when accessing certain properties on the DirectoryInfo or FileInfo objects that were returned, such as the LastWriteTime, CreationTime, LastAccessTime. If the code only returned the Name property, it would be fine.

After reflecting over the mscorlib's System.IO namespace I discovered that some properties are essentially lazy-loaded, including all of the DateTime properties. I am sure there was a valid reason for the design of the FileSystemInfo class, however, because of the implementation, the code that used the Framework's base class was 6-10 times slower than code that used the FindFirstFile Win32 API (from http://www.codeproject.com/KB/files/FileSystemEnumerator.aspx) and my own custom FileSystemInfo implementation.