using System.Windows.Forms;
using System.Drawing;
using SharpFile.Infrastructure;

namespace SharpFile.UI {
    public partial class CommandLinePanel : UserControl, IPanel {
        public CommandLinePanel() {
            InitializeComponent();
            this.Dock = DockStyle.Top;

            this.txtCommand.Visible = false;
            this.txtPath.Visible = false;

            this.SizeChanged += delegate {
                //this.txtCommand.Location = new Point(this.txtPath.Width + 4, this.txtPath.Location.X);
                //this.txtCommand.Width = this.Width - this.txtPath.Width - 8;
                this.txtResults.Width = this.Width - 10;

                this.txtCommandLine.Width = this.Width - 10;
            };
        }
        
        //http://bytes.com/forum/thread266167.html

        public void Update(string path) {
            //this.txtPath.Text = path;

            this.txtCommandLine.Text = path;
            Refresh();
        }

        public void Update(IView view) {
            this.txtCommandLine.Text = view.Path;
            Refresh();
        }

        //protected override void OnPaint(PaintEventArgs e) {
        //    Graphics graphics = e.Graphics;
        //    SizeF size = graphics.MeasureString(this.txtPath.Text, this.Font);
        //    this.txtPath.Width = (int)size.Width;

        //    this.txtCommand.Location = new Point(this.txtPath.Width + 4, this.txtPath.Location.X);
        //    //this.txtCommand.Width = (this.Width);// - this.txtPath.Width) + 80;

        //    base.OnPaint(e);
        //}
    }
}