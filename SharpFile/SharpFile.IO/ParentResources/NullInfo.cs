using System;
using SharpFile.IO.Retrievers;

namespace SharpFile.IO.ParentResources {
	public class NullInfo : IParentResource {
		private const string displayName = "---------";

		#region IParentResource Members
		public string DisplayName {
			get { 
				return displayName; 
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

		public DriveInfo Root {
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