namespace SharpFile {
    public partial class MediaListView : ListView {
        public MediaListView()
            : base() {
        }

        public new void UpdateStatus(string status) {
            ShowMessageBox(status);
        }
    }
}