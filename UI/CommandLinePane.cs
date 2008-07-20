using System.Windows.Forms;
using SharpFile.Infrastructure;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.UI {
    public partial class CommandLinePane : DockContent, IPluginPane {
        public CommandLinePane() {
            InitializeComponent();            

            this.SizeChanged += delegate {
                this.txtResults.Width = this.Width - 8;
                this.txtCommandLine.Width = this.Width - 10;
                this.txtResults.Height = this.Height - 24;
            };

            this.txtCommandLine.ProcessOutput += delegate(string output) {
                this.txtResults.Text = string.Format("{0}{1}",
                    output,
                    this.txtResults.Text);
            };

            this.txtResults.Visible = false;

            MenuItem copyMenuItem = new MenuItem("Copy", delegate {
                Clipboard.SetText(this.txtResults.SelectedText);
            });

            MenuItem clearMenuItem = new MenuItem("Clear", delegate {
                this.txtResults.Clear();
            });

            MenuItem selectAllMenuItem = new MenuItem("Select All", delegate {
                this.txtResults.SelectionStart = 0;
                this.txtResults.SelectionLength = this.txtResults.Text.Length;
            });

            this.txtResults.ContextMenu = new ContextMenu();

            this.txtResults.ContextMenu.Popup += delegate {
                this.txtResults.ContextMenu.MenuItems.Clear();

                if (string.IsNullOrEmpty(this.txtResults.SelectedText)) {
                    this.txtResults.ContextMenu.MenuItems.AddRange(new MenuItem[] {
                        clearMenuItem,
                        selectAllMenuItem
                    });
                } else {
                    this.txtResults.ContextMenu.MenuItems.AddRange(new MenuItem[] { 
                        copyMenuItem, 
                        clearMenuItem,
                        selectAllMenuItem
                    });
                }
            };

            this.txtCommandLine.UseCommandLine += delegate(bool useCommandLine) {
                if (useCommandLine) {
                    this.txtResults.Visible = true;
                } else {
                    this.txtResults.Visible = false;
                }

                this.txtCommandLine.Focus();
            };
        }

        public void Update(IView view) {
            this.txtCommandLine.UpdateText(view);
            Refresh();
        }
    }
}