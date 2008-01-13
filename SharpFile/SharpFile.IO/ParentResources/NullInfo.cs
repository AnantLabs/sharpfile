using System;
using System.Collections.Generic;
using SharpFile.IO.Retrievers;
using SharpFile.Infrastructure;

namespace SharpFile.IO.ParentResources {
    public class NullInfo : IResource {
        public event ChildResourceRetriever.GetCompleteDelegate GetComplete;

		private const string displayName = "---------";

        public void OnGetComplete() {
            if (GetComplete != null) {
                GetComplete();
            }
        }

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

        public ChildResourceRetrievers ChildResourceRetrievers {
			get {
				throw new Exception("ChildResourceRetriever not defined.");
			}
		}
		#endregion
	}
}