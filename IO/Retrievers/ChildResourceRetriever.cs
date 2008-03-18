using System;
using SharpFile.Infrastructure;

namespace SharpFile.IO.Retrievers {
    public abstract class ChildResourceRetriever {
        public delegate void GetCompleteDelegate();
        public delegate bool CustomMethodDelegate(IResource resource);
    }
}
