using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Common.Logger;
using SharpFile.Infrastructure;

namespace SharpFile.UI {
    public partial class CommandLineTextBox : UserControl {
        public delegate void ProcessOutputDelegate(string output);
        public event ProcessOutputDelegate ProcessOutput;

        public delegate void UseCommandLineDelegate(bool useCommandLine);
        public event UseCommandLineDelegate UseCommandLine;

        private string path;

        public CommandLineTextBox() {
            InitializeComponent();

            this.txtFile.AutoCompleteMode = AutoCompleteMode.Append;
            this.txtFile.AutoCompleteSource = AutoCompleteSource.CustomSource;

            this.txtFile.KeyUp += delegate(object sender, KeyEventArgs e) {
                if (e.KeyCode == Keys.Enter) {
                    bool useCommandLine = chkUseCommandLine.Checked;

                    string fileName = path + this.txtFile.Text;
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = fileName;

                    if (useCommandLine) {
                        processStartInfo.UseShellExecute = false;
                        processStartInfo.CreateNoWindow = true;
                        processStartInfo.RedirectStandardOutput = true;
                    }

                    try {
                        using (Process process = new Process()) {
                            process.StartInfo = processStartInfo;
                            process.Start();

                            if (useCommandLine) {
                                process.WaitForExit();

                                string stdOutput = process.StandardOutput.ReadToEnd();

                                if (!string.IsNullOrEmpty(stdOutput)) {
                                    string output = string.Format("{0}> {1}",
                                        fileName,
                                        stdOutput);

                                    OnProcessOutput(output);
                                }
                            }
                        }
                    } catch (Win32Exception ex) {
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, "Starting process {0} failed.",
                            fileName);
                    }
                }
            };

            this.chkUseCommandLine.CheckedChanged += delegate {
                OnUseCommandLine(this.chkUseCommandLine.Checked);
            };
        }

        public void OnProcessOutput(string output) {
            if (ProcessOutput != null) {
                ProcessOutput(output);
            }
        }

        public new void Focus() {
            this.txtFile.Focus();
        }

        public void OnUseCommandLine(bool useCommandLine) {
            if (UseCommandLine != null) {
                UseCommandLine(useCommandLine);
            }
        }

        public void UpdateText(IView view) {
            if (view.Path != null) {
                if (path != view.Path.FullName) {
                    path = view.Path.FullName;
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
        }

        protected override void OnPaint(PaintEventArgs e) {
            string path = this.txtPath.Text;
            Size size = TextRenderer.MeasureText(path, this.Font);

            this.txtPath.Width = (int)size.Width - 5;
            this.txtFile.Width = (this.Width - this.txtPath.Width - this.chkUseCommandLine.Width - 2);

            txtPath.Location = new Point(0, 0);
            txtFile.Location = new Point(txtPath.Width, 0);
            chkUseCommandLine.Location = new Point(txtFile.Location.X + txtFile.Width + 5, 0);

            base.OnPaint(e);
        }
    }
}