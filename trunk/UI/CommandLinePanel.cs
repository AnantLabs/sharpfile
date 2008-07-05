using System.Windows.Forms;
using SharpFile.Infrastructure;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.UI {
    public partial class CommandLinePanel : DockContent, IPluginPanel {
        public CommandLinePanel() {
            InitializeComponent();
            this.TabText = "Command Line";
            //this.DockAreas = DockAreas.DockTop | DockAreas.DockTop | DockAreas.Document;
            this.AllowEndUserDocking = false;

            this.SizeChanged += delegate {
                this.txtResults.Width = this.Width - 10;
                this.txtCommandLine.Width = this.Width - 10;
            };
        }

        public void Update(IView view) {
            this.txtCommandLine.UpdateText(view);
            Refresh();
        }
    }
}