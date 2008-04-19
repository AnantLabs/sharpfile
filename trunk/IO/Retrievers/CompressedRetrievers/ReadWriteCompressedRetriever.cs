using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Common.Logger;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.IO.Retrievers.CompressedRetrievers {
    public class ReadWriteCompressedRetriever : ChildResourceRetriever {
        /// <summary>
        /// Override the default Execute for ChildResourceRetrievers for files. Use the default for everything else.
        /// </summary>
        /// <param name="view">View to output the results to.</param>
        /// <param name="resource">Resource to grab output from.</param>
        public override void Execute(IView view, IResource resource) {
            if (resource is CompressedFileInfo) {
                string zipFilePath = resource.FullName.Substring(0, resource.FullName.IndexOf(".zip") + 4);
                IResource zipFileResource = FileSystemInfoFactory.GetFileSystemInfo(zipFilePath);

                string tmpDirectory = string.Format(@"{0}{1}tmp{1}{2}\",
                    System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    FileSystemInfo.DirectorySeparator,
                    zipFileResource.Name);

                FastZip fastZip = new FastZip();
                fastZip.ExtractZip(zipFilePath, tmpDirectory, resource.Name);

                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.ErrorDialog = true;
                processStartInfo.UseShellExecute = true;
                processStartInfo.FileName = tmpDirectory + resource.Name;

                try {
                    Process.Start(processStartInfo);
                } catch (System.ComponentModel.Win32Exception ex) {
                    Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, "File, {0}, cannot be opened.",
                        resource.FullName);
                }

                return;
            } else {
                base.Execute(view, resource);
            }
        }

        protected override IList<IChildResource> getResources(IResource resource, string filter) {
            List<IChildResource> childResources = new List<IChildResource>();

            if (Settings.Instance.ShowRootDirectory) {
                childResources.Add(new RootDirectoryInfo(resource.Root.Name));
            }

            if (Settings.Instance.ShowParentDirectory) {
                if (!Settings.Instance.ShowRootDirectory ||
                    (Settings.Instance.ShowRootDirectory &&
                    !resource.Path.Equals(resource.Root.Name))) {
                    childResources.Add(new ParentDirectoryInfo(resource.Path));
                }
            }

            using (ZipFile zipFile = new ZipFile(resource.FullName)) {
                foreach (ZipEntry zipEntry in zipFile) {
                    string zipEntryName = zipEntry.Name.Replace("/", FileSystemInfo.DirectorySeparator);

                    if (zipEntryName.EndsWith(FileSystemInfo.DirectorySeparator)) {
                        zipEntryName = zipEntryName.Remove(zipEntryName.Length - 1, 1);
                    }

                    string[] directoryLevels = zipEntryName.Split(System.IO.Path.DirectorySeparatorChar);

                    if (string.IsNullOrEmpty(filter) || filter.Equals("*.*") || zipEntryName.Contains(filter)) {
                        // Only show the current directories filesystem objects.
                        if (directoryLevels.Length == 1) {
                            string fullName = string.Format(@"{0}{1}{2}",
                                    resource.FullName,
                                    FileSystemInfo.DirectorySeparator,
                                    zipEntryName);

                            if (zipEntry.IsFile) {
                                childResources.Add(new CompressedFileInfo(fullName, zipEntryName, zipEntry.Size,
                                    zipEntry.CompressedSize, zipEntry.DateTime));
                            } else if (zipEntry.IsDirectory) {
                                childResources.Add(new CompressedDirectoryInfo(fullName, zipEntryName,
                                        zipEntry.DateTime));
                            }
                        } else if (directoryLevels.Length > 1) {
                            // Derive the folder if a file is nested deep.
                            string directoryName = directoryLevels[0];

                            if (string.IsNullOrEmpty(filter) || directoryName.Contains(filter)) {
                                string fullName = string.Format(@"{0}{1}{2}",
                                       resource.FullName,
                                       FileSystemInfo.DirectorySeparator,
                                       directoryName);

                                if (childResources.Find(delegate(IChildResource c) {
                                    return c.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase);
                                }) == null) {
                                    childResources.Add(new CompressedDirectoryInfo(fullName, directoryName,
                                            DateTime.MinValue));
                                }
                            }
                        }
                    }
                }
            }

            return childResources;
        }
    }
}