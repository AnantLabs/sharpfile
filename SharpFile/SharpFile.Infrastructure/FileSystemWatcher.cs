using System;
using System.IO;
using System.Collections.Generic;
using System.Timers;
using System.ComponentModel;

namespace SharpFile.Infrastructure {
	public class FileSystemWatcher {
		private System.IO.FileSystemWatcher fileSystemWatcher = new System.IO.FileSystemWatcher();
		private Stack<ChangedDelegate> stack = new Stack<ChangedDelegate>();
		private Timer timer = new Timer();
		private ISynchronizeInvoke synchronizingObject;

		private static object lockObject = new object();

		public delegate void ChangedDelegate(object sender, FileSystemEventArgs e);
		public event ChangedDelegate Changed;

		public FileSystemWatcher(ISynchronizeInvoke synchronizingObject, double interval) {
			this.synchronizingObject = synchronizingObject;

			// Set up the timer.
			timer.SynchronizingObject = synchronizingObject;
			timer.Interval = interval;
			timer.Enabled = true;

			timer.Elapsed += delegate {
				lock (lockObject) {
					if (stack.Count > 0) {
						//synchronizingObject.Invoke(stack.Peek(), null);
						stack.Peek().Invoke(null, null);
						stack.Clear();
					}
				}
			};

			// Set up the watcher.
			fileSystemWatcher.SynchronizingObject = synchronizingObject;
			fileSystemWatcher.IncludeSubdirectories = false;
			fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName |
				NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;

			fileSystemWatcher.Changed += delegate {
				stack.Push(OnChanged);
			};
		}

		public void OnChanged(object sender, FileSystemEventArgs e) {
			if (Changed != null) {
				Changed(sender, e);
			}
		}

		public bool EnableRaisingEvents {
			get {
				return fileSystemWatcher.EnableRaisingEvents;
			}
			set {
				fileSystemWatcher.EnableRaisingEvents = value;
			}
		}

		public string Path {
			get {
				return fileSystemWatcher.Path;
			}
			set {
				fileSystemWatcher.Path = value;
			}
		}

		public string Filter {
			get {
				return fileSystemWatcher.Filter;
			}
			set {
				fileSystemWatcher.Filter = value;
			}
		}

		public double Interval {
			get {
				return timer.Interval;
			}
			set {
				timer.Interval = value;
			}
		}
	}
}
