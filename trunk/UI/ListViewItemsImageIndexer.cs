using System.Windows.Forms;
using SharpFile.Infrastructure.Interfaces;

namespace SharpFile.UI {
    public class ListViewItemsImageIndexer {
        private IView view;

        public ListViewItemsImageIndexer(IView view) {
            this.view = view;
        }

        public void Update(bool useFileAttributes) {
            foreach (ListViewItem item in view.ItemDictionary.Values) {
                IResource resource = (IResource)item.Tag;
                int imageIndex = view.OnGetImageIndex(resource, useFileAttributes);

                if (imageIndex > -1) {
                    view.Invoke((MethodInvoker)delegate {
                        item.ImageIndex = imageIndex;
                    });
                }
            }
        }
    }
}