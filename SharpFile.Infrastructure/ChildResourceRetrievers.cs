using System.Collections.Generic;
using Common;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure.Interfaces;
using SharpFile.Infrastructure.IO.ChildResources;

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
        public IEnumerable<IChildResourceRetriever> Filter(IResource fsi) {
            foreach (IChildResourceRetriever childResourceRetriever in this) {
                if (childResourceRetriever.OnFilterMethod(fsi)) {
                    yield return childResourceRetriever;
                } else if (childResourceRetriever.OnFilterMethodWithArguments(fsi, childResourceRetriever.FilterMethodArguments)) {
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
        /// Custom method which always returns true.
        /// </summary>
        /// <param name="fsi">File system object. Not currently used for anything.</param>
        /// <returns>Whether or not the child restriever resource should be used for this file system object. Always returns true.</returns>
        public static bool TrueFilterMethod(IResource fsi) {
            return true;
        }

		/// <summary>
		/// Custom method that always returns false.
		/// </summary>
		/// <param name="fsi">File system object. Not currently used for anything.</param>
		/// <returns>Whether or not the child restriever resource should be used for this file system object. Always returns false.</returns>
		public static bool FalseFilterMethod(IResource fsi) {
			return false;
		}

        /// <summary>
        /// Is used to determine if a file system object has an extension in a list of extensions.
        /// </summary>
        /// <param name="fsi">File system object.</param>
        /// <param name="extensions">List of extensions.</param>
        /// <returns>Whether or not the file system object has an extension that is contained in the list of extensions.</returns>
        public static bool IsFileWithExtension(IResource fsi, List<string> extensions) {
            if (extensions != null) {
                string extension = General.GetExtension(fsi.FullName).ToLower();

                if (extensions.Contains(extension)) {
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
        public static bool IsCompressedFileThorough(IResource fsi) {
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
    }
}