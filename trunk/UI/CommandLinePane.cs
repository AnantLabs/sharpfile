using System.Windows.Forms;
using SharpFile.Infrastructure;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.UI {
    public partial class CommandLinePane : DockContent, IPluginPane {
        private DockState visibleState = DockState.DockBottomAutoHide;

        public CommandLinePane() {
            InitializeComponent();
            this.AllowEndUserDocking = false;
            this.Dock = DockStyle.Bottom;

            this.SizeChanged += delegate {
                this.txtResults.Width = this.Width - 10;
                this.txtCommandLine.Width = this.Width - 10;
            };
        }

        public void Update(IView view) {
            this.txtCommandLine.UpdateText(view);
            Refresh();
        }

        public void GiveUpFocus() {
            this.DockHandler.GiveUpFocus();
        }

        public DockState VisibleDockState {
            get {
                return visibleState;
            }
            set {
                visibleState = value;
            }
        }
    }
}