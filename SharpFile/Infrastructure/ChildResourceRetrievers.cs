using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Common;

namespace SharpFile.Infrastructure {
    public class ChildResourceRetrievers : List<IChildResourceRetriever> {
        /// <summary>
        /// Construct an empty list.
        /// </summary>
        public ChildResourceRetrievers()
            : base() {
        }

        /// <summary>
        /// Construct a list with a capacity.
        /// </summary>
        /// <param name="capacity">Capacity.</param>
        public ChildResourceRetrievers(int capacity)
            : base(capacity) {
        }

        /// <summary>
        /// Construct a list from an IEnumerable collection.
        /// </summary>
        /// <param name="collection">Original collection.</param>
        public ChildResourceRetrievers(IEnumerable<IChildResourceRetriever> collection)
            : base(collection) {
        }

        /// <summary>
        /// Filter the list of child resource retrievers for the file system object.
        /// </summary>
        /// <param name="fsi">File system object.</param>
        /// <returns>List of appropriate child resource retrievers.</returns>
        public IEnumerable<IChildResourceRetriever> Filter(IChildResource fsi) {
            foreach (IChildResourceRetriever childResourceRetriever in this) {
                if (childResourceRetriever.OnCustomMethod(fsi)) {
                    yield return childResourceRetriever;
                } else if (childResourceRetriever.OnCustomMethodWithArguments(fsi, childResourceRetriever.CustomMethodArguments)) {
                    yield return childResourceRetriever;
                }
            }
        }

        /// <summary>
        /// Deep-copy clones the list of child resource retrievers.
        /// </summary>
        /// <returns>A clone of the original list of child resource retrievers.</returns>
        public ChildResourceRetrievers Clone() {
            ChildResourceRetrievers childResourceRetrievers = new ChildResourceRetrievers(this.Count);

            // Call clone for each child resource retriever.
            foreach (IChildResourceRetriever childResourceRetriever in this) {
                childResourceRetrievers.Add(childResourceRetriever.Clone());
            }

            return childResourceRetrievers;
        }

        /// <summary>
        /// Default custom method used to determine which child resource retriever is appropriate for the file system object.
        /// </summary>
        /// <param name="fsi">File system object. Not currently used for anything.</param>
        /// <returns>Whether or not the child restriever resource should be used for this file system object. Always returns true.</returns>
        public static bool DefaultCustomMethod(IChildResource fsi) {
            return true;
        }

        /// <summary>
        /// Is used to determine if a file system object has an extension in a list of extensions.
        /// </summary>
        /// <param name="fsi">File system object.</param>
        /// <param name="extensions">List of extensions.</param>
        /// <returns>Whether or not the file system object has an extension that is contained in the list of extensions.</returns>
        public static bool IsFileWithExtension(IChildResource fsi, List<string> extensions) {
            if (extensions != null) {
                string extension = General.GetExtension(fsi.FullName).ToLower();

                if (extensions.Contains(extension)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Used to determine if a file system object is a compressed file.
        /// </summary>
        /// <param name="fsi">File systm object.</param>
        /// <returns>Whether or not the file system object is compressed.</returns>
        public static bool IsCompressedFile(IChildResource fsi) {
            // TODO: Should use the IsFileWithExtension instead.
            if (fsi is FileInfo) {
                if (Common.General.GetExtension(fsi.FullName).ToLower().Equals(".zip")) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Thoroughly checks whether the file is compressed or not.
        /// </summary>
        /// <param name="fsi">File system object.</param>
        /// <returns>Whether or not the file system object is compressed.</returns>
        public static bool IsCompressedFileThorough(IChildResource fsi) {
            if (fsi is FileInfo) {
                try {
                    ZipFile zipFile = new ZipFile(fsi.FullName);
                    return true;
                } catch {
                     //The resource is not a compressed file.
                }
            }

            return false;
        }

        /// <summary>
        /// Used to determine if a file system object is a media file.
        /// </summary>
        /// <param name="fsi">File system object.</param>
        /// <returns>Whether or not the file system object is a media file.</returns>
        public static bool IsMediaFile(IChildResource fsi) {
            if (fsi is FileInfo) {
                if (General.GetExtension(fsi.FullName).ToLower().Equals(".bmp")) {
                    return true;
                }
            }

            return false;
        }
    }
}