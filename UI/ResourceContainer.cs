using System.Collections.Generic;
using SharpFile.Infrastructure;

namespace SharpFile.UI {
    public class ResourceContainer : IEnumerable<IResource> {
        private IList<IResource> list;
        private long size;

        public ResourceContainer() {
            list = new List<IResource>();
            size = 0;
        }

        public bool Contains(IResource resource) {
            return list.Contains(resource);
        }

        public void Add(IResource resource) {
            list.Add(resource);
            size = 0;
        }

        public void Remove(IResource resource) {
            list.Remove(resource);
            size = 0;
        }

        public void Clear() {
            list.Clear();
            size = 0;
        }

        public void ReevaluateSizes() {
            size = 0;
        }

        /// <summary>
        /// Total size of all resources in the container.
        /// Lazy-loaded because it can be labor-intensive for directories.
        /// </summary>
        public long TotalSize {
            get {
                if (size == 0) {
                    foreach (IResource resource in list) {
                        size += resource.Size;
                    }
                }

                return size;
            }
        }

        public string HumanReadableTotalSize {
            get {
                return Common.General.GetHumanReadableSize(TotalSize.ToString());
            }
        }

        #region IEnumerable<IResource> Members
        public IEnumerator<IResource> GetEnumerator() {
            return list.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return list.GetEnumerator();
        }
        #endregion
    }
}