namespace SharpFile.IO.Retrievers {
	public interface IChildResourceRetriever {
		void Get(IView view, IResource resource);
        void Cancel();
	}
}