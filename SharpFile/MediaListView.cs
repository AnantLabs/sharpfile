namespace SharpFile {
    public partial class MediaListView : ListView {
        public MediaListView()
            : base() {
        }

        public new void OnUpdateStatus(string status) {
            ShowMessageBox(status);
        }
    }
}