using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharpFile.Infrastructure;

namespace SharpFile {
    public partial class MediaListView : IView {
        private ListView listView = new ListView();
        private PictureBox pictureBox = new PictureBox();
        private SplitContainer splitContainer = new SplitContainer();

        public MediaListView() {
            initializeComponents();
        }

        private void initializeComponents() {
            this.splitContainer.SplitterDistance = 10;
            this.splitContainer.Orientation = Orientation.Horizontal;
            this.splitContainer.Panel1.Controls.Add(listView);
            this.splitContainer.Panel2.Controls.Add(pictureBox);
        }

        public void OnUpdateStatus(string status) {
            ShowMessageBox(status);
        }

        /// <summary>
        /// Parses the file/directory information and inserts the file info into the listview.
        /// </summary>
        public void InsertItem(IChildResource resource) {
            listView.InsertItem(resource);

            pictureBox.ImageLocation = resource.FullPath;
        }

        #region IView Members

        public void AddItemRange(IEnumerable<IChildResource> childResources) {
            listView.AddItemRange(childResources);
        }

        public void AddItemRange(IEnumerable<System.IO.FileSystemInfo> resources) {
            listView.AddItemRange(resources);
        }

        public void RemoveItem(string path) {
            listView.RemoveItem(path);
        }

        public void Clear() {
            listView.Clear();
        }

        public void BeginUpdate() {
            listView.BeginUpdate();
        }

        public void EndUpdate() {
            listView.EndUpdate();
        }

        public string Path {
            get { return listView.Path; }
        }

        public string Filter {
            get { return listView.Filter; }
        }

        public FileSystemWatcher FileSystemWatcher {
            get { return listView.FileSystemWatcher; }
        }

        public Control Control {
            get { return splitContainer; }
        }

        public void ShowMessageBox(string text) {
            listView.ShowMessageBox(text);
        }

        public IViewComparer Comparer {
            get {
                return listView.Comparer;
            }
            set {
                listView.Comparer = value;
            }
        }

        public IEnumerable<ColumnInfo> ColumnInfos {
            get {
                return listView.ColumnInfos;
            }
            set {
                listView.ColumnInfos = value;
            }
        }

        public string Name {
            get {
                return listView.Name;
            }
            set {
                listView.Name = value;
            }
        }

        public bool Enabled {
            get {
                return listView.Enabled;
            }
            set {
                listView.Enabled = value;
            }
        }

        public void OnUpdatePath(string path) {
            listView.OnUpdatePath(path);
        }

        public void OnUpdateProgress(int progress) {
            listView.OnUpdateProgress(progress);
        }

        public int OnGetImageIndex(IResource resource) {
            return listView.OnGetImageIndex(resource);
        }

        public event SharpFile.Infrastructure.View.GetImageIndexDelegate GetImageIndex;
        public event SharpFile.Infrastructure.View.UpdateProgressDelegate UpdateProgress;
        public event SharpFile.Infrastructure.View.UpdateStatusDelegate UpdateStatus;
        public event SharpFile.Infrastructure.View.UpdatePathDelegate UpdatePath;

        #endregion
    }
}