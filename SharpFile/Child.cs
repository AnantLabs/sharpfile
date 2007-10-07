using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common;
using SharpFile.IO;
using SharpFile.Infrastructure;

using SharpFile.UI;

namespace SharpFile {
	public partial class Child : Form {
		public delegate void OnUpdateStatusDelegate(string status);
		public event OnUpdateStatusDelegate OnUpdateStatus;

		public delegate void OnUpdateProgressDelegate(int value);
		public event OnUpdateProgressDelegate OnUpdateProgress;

		public delegate int OnGetImageIndexDelegate(string path, bool forceUpdate);
		public event OnGetImageIndexDelegate OnGetImageIndex;

		/// <summary>
		/// Child ctor.
		/// </summary>
		public Child() {
			InitializeComponent();
			addTab();

			SelectedFileBrowser.OnGetImageIndex += FileBrowser_OnGetImageIndex;
			SelectedFileBrowser.OnUpdateProgress += SelectedFileBrowser_OnUpdateProgress;
			SelectedFileBrowser.OnUpdateStatus += SelectedFileBrowser_OnUpdateStatus;
		}

		void SelectedFileBrowser_OnUpdateStatus(string status) {
			if (OnUpdateStatus != null) {
				OnUpdateStatus(status);
			}
		}

		void SelectedFileBrowser_OnUpdateProgress(int value) {
			if (OnUpdateProgress != null) {
				OnUpdateProgress(value);
			}
		}

		int FileBrowser_OnGetImageIndex(string path, bool forceUpdate) {
			if (OnGetImageIndex != null) {
				return OnGetImageIndex(path, forceUpdate);
			}

			return -1;
		}

		public void UpdateDriveListing() {
			if (this.tabControl.TabCount > 0) {
				SelectedFileBrowser.UpdateDriveListing();
			}
		}

		private void addTab() {
			TabPage tabPage = new TabPage();
			this.tabControl.Controls.Add(tabPage);
		}

		public FileBrowser SelectedFileBrowser {
			get {
				return this.tabControl.SelectedTab.FileBrowser;
			}
		}

		public SysImageList SysImageList {
			get {
				return ((Parent)this.MdiParent).SysImageList;
			}
		}
	}
}