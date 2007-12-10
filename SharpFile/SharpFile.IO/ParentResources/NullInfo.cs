using System;
using SharpFile.IO.Retrievers;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ParentResources {
    public class NullInfo : IResource {
		private const string displayName = "---------";

		#region IParentResource Members
		public string DisplayName {
			get { 
				return displayName; 
			}
		}

        public string Path {
            get {
                throw new Exception("The method or operation is not implemented.");
            }
        }

		public string Name {
			get { 
				throw new Exception("The method or operation is not implemented."); 
			}
		}

		public string FullPath {
			get { 
				throw new Exception("The method or operation is not implemented."); 
			}
		}

		public long Size {
			get { 
				throw new Exception("The method or operation is not implemented."); 
			}
		}

		public IResource Root {
			get { 
				throw new Exception("The method or operation is not implemented."); 
			}
		}

		public void Execute(IView view) {
			throw new Exception("Execute not defined.");
		}

		public IChildResourceRetriever ChildResourceRetriever {
			get {
				throw new Exception("ChildResourceRetriever not defined.");
			}
		}
		#endregion
	}
}