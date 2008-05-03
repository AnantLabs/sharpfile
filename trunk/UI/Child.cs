using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Common;
using Common.Logger;
using SharpFile.Infrastructure;
using View = SharpFile.Infrastructure.View;

namespace SharpFile.UI {
    public class Child : UserControl {
        public event View.GetImageIndexDelegate GetImageIndex;
        public event View.UpdatePathDelegate UpdatePath;

        private TabControl tabControl;

        public Child(string name) {
            this.Name = name;
            this.tabControl = new TabControl();
            this.SuspendLayout();

            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.IsVisible = true;

            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.ResumeLayout(false);
        }

        #region Events.
        private int OnGetImageIndex(IResource fsi) {
            if (GetImageIndex != null) {
                return GetImageIndex(fsi);
            }

            return -1;
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e) {
            this.Text = SelectedPath;
            OnUpdatePath(SelectedPath);
        }

        private void OnUpdatePath(string path) {
            if (UpdatePath != null) {
                UpdatePath(path);
            }
        }

        #endregion

        public void AddTab() {
            // TODO: Make this more generic for cross-platform compatibility.
            AddTab(@"C:\", true);
        }

        public void AddTab(string path, bool selectNewTab) {
            FileBrowser fileBrowser = new FileBrowser(this.Name);
            fileBrowser.GetImageIndex += OnGetImageIndex;
            fileBrowser.UpdatePath += OnUpdatePath;

            fileBrowser.Path = path;

            this.TabControl.Controls.Add(fileBrowser);
            this.TabControl.Selected += tabControl_Selected;

            if (selectNewTab) {
                this.TabControl.SelectedTab = fileBrowser;
            }
        }

        public TabControl TabControl {
            get {
                return tabControl;
            }
        }

        public List<string> Paths {
            get {
                List<string> paths = new List<string>();

                foreach (Control control in this.TabControl.Controls) {
                    if (control is FileBrowser) {
                        string path = ((FileBrowser)control).Path;

                        paths.Add(path);
                    }
                }

                return paths;
            }
            set {
                if (value.Count == 0) {
                    Settings.Instance.Logger.Log(LogLevelType.Verbose,
                            @"Paths are null or empty for {0}; assume C:\ is valid.", Name);

                    AddTab();
                } else {
                    foreach (string path in value) {
                        AddTab(path, false);
                    }
                }
            }
        }

        public string SelectedPath {
            get {
                return Forms.GetPropertyInChild<string>(this.TabControl.SelectedTab, "Path");
            }
        }
    }
}