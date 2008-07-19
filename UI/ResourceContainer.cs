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
            size += resource.Size;
        }

        public void Remove(IResource resource) {
            list.Remove(resource);
            size -= resource.Size;
        }

        public void RemoveAt(int index) {
            list.RemoveAt(index);
        }

        public long TotalSize {
            get {
                return size;
            }
        }

        public string HumanReadableTotalSize {
            get {
                return Common.General.GetHumanReadableSize(size.ToString());
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