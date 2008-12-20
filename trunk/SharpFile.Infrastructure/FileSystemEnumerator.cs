/*=============================================================================
    FileSystemEnumerator.cs: Lazy enumerator for finding files in subdirectories.

    Copyright (c) 2006 Carl Daniel. Distributed under the Boost
    Software License, Version 1.0. (See accompanying file
    LICENSE.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
=============================================================================*/

// ---------------------------------------------------------------------------
// FileSystemEnumerator implementation
// http://www.codeproject.com/KB/files/FileSystemEnumerator.aspx
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SharpFile.Infrastructure.Interfaces;
using Common.Win32;

namespace SharpFile.Infrastructure {
    /// <summary>
    /// File system enumerator.  This class provides an easy to use, efficient mechanism for searching a list of
    /// directories for files matching a list of file specifications.  The search is done incrementally as matches
    /// are consumed, so the overhead before processing the first match is always kept to a minimum.
    /// </summary>
    public sealed class FileSystemEnumerator : IDisposable {
        //public delegate bool StringTextSearcher(string term);
        //public delegate bool RegexTextSearcher(Regex term);

        /// <summary>
        /// Information that's kept in our stack for simulated recursion
        /// </summary>
        private struct SearchInfo {
            /// <summary>
            /// Find handle returned by FindFirstFile
            /// </summary>
            public SafeFindHandle Handle;

            /// <summary>
            /// Path that was searched to yield the find handle.
            /// </summary>
            public string Path;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="h">Find handle returned by FindFirstFile.</param>
            /// <param name="p">Path corresponding to find handle.</param>
            public SearchInfo(SafeFindHandle h, string p) {
                Handle = h;
                Path = p;
            }
        }

        /// <summary>
        /// Stack of open scopes.  This is a member (instead of a local variable)
        /// to allow Dispose to close any open find handles if the object is disposed
        /// before the enumeration is completed.
        /// </summary>
        private Stack<SearchInfo> m_scopes;

        /// <summary>
        /// Array of paths to be searched.
        /// </summary>
        private string[] m_paths;

        /// <summary>
        /// Array of regular expressions that will detect matching files.
        /// </summary>
        private List<Regex> m_fileSpecs;

        /// <summary>
        /// If true, sub-directories are searched.
        /// </summary>
        private bool m_includeSubDirs;

        #region IDisposable implementation
        /// <summary>
        /// IDisposable.Dispose
        /// </summary>
        public void Dispose() {
            while (m_scopes.Count > 0) {
                SearchInfo si = m_scopes.Pop();
                si.Handle.Close();
            }
        }
        #endregion

        public FileSystemEnumerator(string pathsToSearch)
            : this(pathsToSearch, "*", null, false) {
        }

        public FileSystemEnumerator(string pathsToSearch, string fileTypesToMatch)
            : this(pathsToSearch, fileTypesToMatch, null, false) {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pathsToSearch">Semicolon- or comma-delimitted list of paths to search.</param>
        /// <param name="fileTypesToMatch">Semicolon- or comma-delimitted list of wildcard filespecs to match.</param>
        /// <param name="includeSubDirs">If true, subdirectories are searched.</param>
        public FileSystemEnumerator(string pathsToSearch, string fileTypesToMatch, bool includeSubDirs)
            : this(pathsToSearch, fileTypesToMatch, null, includeSubDirs) {
        }

        public FileSystemEnumerator(string pathsToSearch, string fileTypesToMatch, Delegate textSearchMethod, bool includeSubDirs) {
            m_scopes = new Stack<SearchInfo>();

            // check for nulls
            if (null == pathsToSearch)
                throw new ArgumentNullException("pathsToSearch");
            if (null == fileTypesToMatch)
                throw new ArgumentNullException("fileTypesToMatch");

            // make sure spec doesn't contain invalid characters
            if (fileTypesToMatch.IndexOfAny(new char[] { ':', '<', '>', '/', '\\' }) >= 0)
                throw new ArgumentException("invalid characters in wildcard pattern", "fileTypesToMatch");

            m_includeSubDirs = includeSubDirs;
            m_paths = pathsToSearch.Split(new char[] { ';', ',' });

            string[] specs = fileTypesToMatch.Split(new char[] { ';', ',' });
            m_fileSpecs = new List<Regex>(specs.Length);

            foreach (string spec in specs) {
                // trim whitespace off file spec and convert Win32 wildcards to regular expressions
                string pattern = spec
                  .Trim()
                  .Replace(".", @"\.")
                  .Replace("*", @".*")
                  .Replace("?", @".?");

                m_fileSpecs.Add(
                  new Regex("^" + pattern + "$", RegexOptions.IgnoreCase)
                  );
            }
        }

        public List<IChildResource> Matches() {
            List<IChildResource> childResources = new List<IChildResource>();
            Stack<string> pathsToSearch = new Stack<string>(m_paths);
            WIN32_FIND_DATA findData = new WIN32_FIND_DATA();
            string path, fileName, fullName;

            while (0 != pathsToSearch.Count) {
                path = pathsToSearch.Pop().Trim();

                using (SafeFindHandle handle = Kernel32.FindFirstFile(Path.Combine(path, "*"), findData)) {
                    if (!handle.IsInvalid) {
                        do {
                            fileName = findData.Name;
                            if (string.IsNullOrEmpty(fileName)) {
                                continue;
                            }

							if (string.Equals(fileName, ".", StringComparison.Ordinal)) {
								continue;
							}

                            if (string.Equals(fileName, "..", StringComparison.Ordinal)) {
                                continue;
                            }

                            fullName = Path.Combine(path, fileName);

                            // TODO: Setting to show Hidden/System files.
                            if ((FileAttributes.Directory & findData.Attributes) == FileAttributes.Directory) {
                                if (m_includeSubDirs) {
                                    pathsToSearch.Push(Path.Combine(path, fileName));
                                }

                                childResources.Add(new SharpFile.Infrastructure.IO.ChildResources.DirectoryInfo(fullName, findData));
                            } else {
                                foreach (Regex fileSpec in m_fileSpecs) {
                                    if (fileSpec.IsMatch(fileName)) {
                                        childResources.Add(new SharpFile.Infrastructure.IO.ChildResources.FileInfo(fullName, findData));
                                        break;
                                    }
                                }
                            }

                        } while (Kernel32.FindNextFile(handle, findData));
                    }
                }
            }

            return childResources;
        }
    }
}