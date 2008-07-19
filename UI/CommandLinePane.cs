using System.Windows.Forms;
using SharpFile.Infrastructure;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.UI {
    public partial class CommandLinePane : DockContent, IPluginPane {
        public CommandLinePane() {
            InitializeComponent();
            this.Dock = DockStyle.Bottom;

            this.SizeChanged += delegate {
                this.txtResults.Width = this.Width - 10;
                this.txtCommandLine.Width = this.Width - 10;
                this.txtResults.Height = this.Height - this.txtCommandLine.Height;
            };

            this.txtCommandLine.ProcessOutput += delegate(string output) {
                this.txtResults.Text += output;
            };

            this.txtResults.Visible = false;

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