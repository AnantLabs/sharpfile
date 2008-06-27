using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Common.Logger;
using SharpFile.Infrastructure;

namespace SharpFile.UI {
    public partial class CommandLineTextBox : UserControl {
        private string path;

        public CommandLineTextBox() {
            InitializeComponent();

            this.txtFile.AutoCompleteMode = AutoCompleteMode.Append;
            this.txtFile.AutoCompleteSource = AutoCompleteSource.CustomSource;

            this.txtFile.KeyUp += delegate(object sender, KeyEventArgs e) {
                if (e.KeyCode == Keys.Enter) {
                    string fileName = path + this.txtFile.Text;
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = fileName;

                    try {
                        Process.Start(processStartInfo);
                    } catch (Win32Exception ex) {
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, "Starting process {0} failed.",
                            fileName);
                    }
                }
            };
        }

        public void UpdateText(IView view) {
            if (path != view.Path) {
                path = view.Path;
                this.txtPath.Text = path + ">";
                this.txtFile.Text = string.Empty;
                this.txtFile.AutoCompleteCustomSource.Clear();

                // Load the auto-complete source for the path.
                foreach (string key in view.ItemDictionary.Keys) {
                    string source = key.Replace(path, string.Empty);
                    this.txtFile.AutoCompleteCustomSource.Add(source);
                }

                Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            //http://bytes.com/forum/thread266167.html
            Graphics graphics = e.Graphics;
            SizeF size = graphics.MeasureString(this.txtPath.Text, this.Font);

            this.txtPath.Width = (int)size.Width;
            this.txtFile.Width = (this.Width - this.txtPath.Width);

            txtPath.Location = new Point(0, 0);
            txtFile.Location = new Point(txtPath.Width, 0);

            base.OnPaint(e);
        }
    }
}