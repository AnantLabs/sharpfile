using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.SettingsSection;
using SharpFile.IO.ChildResources;

namespace SharpFile.UI {
    internal class ListViewItemsCreator {
        private static readonly object lockObject = new object();
        private static int runningGetters = 0;

        private IView view;
        private IList<IChildResource> resources;
        private List<ListViewItem> listViewItems;
        private int fileCount = 0;
        private int folderCount = 0;

        public ListViewItemsCreator(IView view, IChildResource resource) {
            this.view = view;
            this.listViewItems = new List<ListViewItem>();

            this.resources = new List<IChildResource>();
            this.resources.Add(resource);
        }

        public ListViewItemsCreator(IView view, IList<IChildResource> resources) {
            this.view = view;
            this.resources = resources;
            this.listViewItems = new List<ListViewItem>();
        }

        public List<ListViewItem> Get(StringBuilder sb) {
            /*
            runningGetters = resources.Count;

            // TODO: Instead of creating a new thread for each, just create two threads and parcel the correct number of resources to each.
            // Create a new listview item with the display name.
            for (int i = 0; i < runningGetters; i++) {
                IChildResource resource = resources[i];
                ThreadPool.QueueUserWorkItem(createListViewItem, resource);
            }

            lock (lockObject) {
                while (runningGetters > 0) {
                    Monitor.Wait(lockObject);
                }
            }
            */

            foreach (IChildResource resource in resources) {
                try {
                    ListViewItem item = addItem(resource);
                    listViewItems.Add(item);
                } catch (Exception ex) {
                    sb.AppendFormat("{0}: {1}",
                         resource.FullName,
                         ex.Message);
                }
            }

            return listViewItems;
        }

        /// <summary>
        /// Adds the item to the view.
        /// </summary>
        /// <param name="resource">Resource to add.</param>
        protected ListViewItem addItem(IResource resource) {
            lock (lockObject) {
                if (!view.ItemDictionary.ContainsKey(resource.FullName)) {
                    ListViewItem item = createListViewItem(resource);
                    view.ItemDictionary.Add(resource.FullName, item);

                    return item;
                } else {
                    return view.ItemDictionary[resource.FullName];
                }
            }
        }

        /// <summary>
        /// Create listview from the filesystem information.
        /// </summary>
        /// <param name="fileSystemInfo">Filesystem information.</param>
        /// <returns>Listview item that references the filesystem object.</returns>
        private ListViewItem createListViewItem(IResource resource) {
            ListViewItem item = new ListViewItem();
            List<ListViewItem.ListViewSubItem> listViewSubItems =
                new List<ListViewItem.ListViewSubItem>(view.ColumnInfos.Count);
            item.Tag = resource;
            item.Name = resource.FullName;

            if (resource is FileInfo) {
                fileCount++;
            } else if (resource is DirectoryInfo &&
                !(resource is ParentDirectoryInfo) &&
                !(resource is RootDirectoryInfo)) {
                folderCount++;
            }

            foreach (ColumnInfo columnInfo in view.ColumnInfos) {
                try {
                    PropertyInfo propertyInfo = null;
                    string propertyName = columnInfo.Property;
                    string text = string.Empty;
                    string tag = string.Empty;

                    // Make sure that the type or it's base type is not supposed to be excluded for this particular column.
                    if (columnInfo.ExcludeForTypes.Find(delegate(FullyQualifiedType f) {
                        Type resourceType = resource.GetType();
                        bool isExcludedType = (f.Type.Equals(resourceType.FullName) ||
                            f.Type.Equals(resourceType.BaseType.FullName));

                        return isExcludedType;
                    }) == null) {
                        // TODO: Use LCG to retrieve properties here instead of GetProperty.
                        // LCG example: TypeUtility<FileInfo>.GetMemberGetPropertyExists<object>(propertyName);
                        // Would shave ~100 ms off the query to retrieve c:\windows\system32\.
                        propertyInfo = resource.GetType().GetProperty(propertyName);

                        // Make sure that the property exists on the resource.
                        if (propertyInfo != null) {
                            text = propertyInfo.GetValue(resource, null).ToString();
                        }
                    }

                    // The original value will be set on the tag for sortability.
                    tag = text;

                    // Invoke the the method delegate if there is one available.
                    if (columnInfo.MethodDelegate != null) {
                        try {
                            text = columnInfo.MethodDelegate.Invoke(text);
                        } catch (Exception ex) {
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "Failed to call the provided method delegate {0} for {1}",
                                    columnInfo.MethodDelegate.Method.Name,
                                    resource.Name);
                        }
                    }

                    if (columnInfo.PrimaryColumn) {
                        item.Text = text;
                        item.SubItems[0].Tag = tag;
                    } else {
                        ListViewItem.ListViewSubItem listViewSubItem =
                            new ListViewItem.ListViewSubItem();
                        listViewSubItem.Text = text;
                        listViewSubItem.Tag = tag;

                        item.SubItems.Add(listViewSubItem);
                    }
                } catch (Exception ex) {
                    Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                        "Column, {0}, with property, {1}, could not be added for {2}.",
                        columnInfo.Text, columnInfo.Property, resource.FullName);
                }
            }

            setImageIndex(resource, item);

            return item;
        }

        /// <summary>
        /// Create listview from the filesystem information.
        /// MULTI-THREADED VERSION.
        /// </summary>
        /// <param name="fileSystemInfo">Filesystem information.</param>
        /// <returns>Listview item that references the filesystem object.</returns>
        private void createListViewItem(object state) {
            IResource resource = (IResource)state;
            ListViewItem item = new ListViewItem();
            List<ListViewItem.ListViewSubItem> listViewSubItems =
                new List<ListViewItem.ListViewSubItem>(view.ColumnInfos.Count);
            item.Tag = resource;
            item.Name = resource.FullName;

            if (resource is FileInfo) {
                fileCount++;
            } else if (resource is DirectoryInfo &&
                !(resource is ParentDirectoryInfo) &&
                !(resource is RootDirectoryInfo)) {
                folderCount++;
            }

            foreach (ColumnInfo columnInfo in view.ColumnInfos) {
                try {
                    PropertyInfo propertyInfo = null;
                    string propertyName = columnInfo.Property;
                    string text = string.Empty;
                    string tag = string.Empty;

                    // Make sure that the type or it's base type is not supposed to be excluded for this particular column.
                    if (columnInfo.ExcludeForTypes.Find(delegate(FullyQualifiedType f) {
                        Type resourceType = resource.GetType();
                        bool isExcludedType = (f.Type.Equals(resourceType.FullName) ||
                            f.Type.Equals(resourceType.BaseType.FullName));

                        return isExcludedType;
                    }) == null) {
                        // TODO: Use LCG to retrieve properties here instead of GetProperty.
                        // LCG example: TypeUtility<FileInfo>.GetMemberGetPropertyExists<object>(propertyName);
                        // Would shave ~100 ms off the query to retrieve c:\windows\system32\.
                        propertyInfo = resource.GetType().GetProperty(propertyName);

                        // Make sure that the property exists on the resource.
                        if (propertyInfo != null) {
                            text = propertyInfo.GetValue(resource, null).ToString();
                        }
                    }

                    // The original value will be set on the tag for sortability.
                    tag = text;

                    // Invoke the the method delegate if there is one available.
                    if (columnInfo.MethodDelegate != null) {
                        try {
                            text = columnInfo.MethodDelegate.Invoke(text);
                        } catch (Exception ex) {
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "Failed to call the provided method delegate {0} for {1}",
                                    columnInfo.MethodDelegate.Method.Name,
                                    resource.Name);
                        }
                    }

                    if (columnInfo.PrimaryColumn) {
                        item.Text = text;
                        item.SubItems[0].Tag = tag;
                    } else {
                        ListViewItem.ListViewSubItem listViewSubItem =
                            new ListViewItem.ListViewSubItem();
                        listViewSubItem.Text = text;
                        listViewSubItem.Tag = tag;

                        item.SubItems.Add(listViewSubItem);
                    }
                } catch (Exception ex) {
                    Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                        "Column, {0}, with property, {1}, could not be added for {2}.",
                        columnInfo.Text, columnInfo.Property, resource.FullName);
                }
            }

            setImageIndex(resource, item);

            lock (lockObject) {
                view.ItemDictionary.Add(resource.FullName, item);
                listViewItems.Add(item);

                runningGetters--;
                Monitor.Pulse(lockObject);
            }
        }

        private void setImageIndex(IResource resource, ListViewItem item) {
            int imageIndex = view.OnGetImageIndex(resource);

            if (imageIndex > -1) {
                item.ImageIndex = imageIndex;
            }
        }

        public int FileCount {
            get {
                return fileCount;
            }
        }

        public int FolderCount {
            get {
                return folderCount;
            }
        }
    }
}