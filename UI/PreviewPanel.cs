using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpFile.Infrastructure;

namespace SharpFile.UI {
    public partial class PreviewPanel : UserControl {
        private PictureBox pictureBox;
        private Label label;

        public PreviewPanel() {
            InitializeComponent();

            this.SizeChanged += delegate {
                this.pictureBox.Height = this.Height - 5;
                this.label.Height = this.Height - 5;
            };
        }

        public void Update(IResource resource) {
            using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
                StringBuilder sb = new StringBuilder();
                Image image = null;
                backgroundWorker.WorkerSupportsCancellation = true;

                sb.AppendFormat("{0}\n", resource.Name);

                backgroundWorker.DoWork += delegate {
                    System.IO.StreamReader sr = null;

                    try {
                        sr = System.IO.File.OpenText(resource.FullName);
                        string line = sr.ReadLine();

                        if (line.Length > 50) {
                            sb.AppendFormat("{0}...",
                                line.Substring(0, 50));
                        } else {
                            sb.Append(line);
                        }
                    } catch {
                        // Ignore any exceptions.
                    } finally {
                        if (sr != null) {
                            sr.Dispose();
                        }
                    }

                    try {
                        image = Image.FromFile(resource.FullName);
                        int width = image.Width;
                        int height = image.Height;

                        if (image.Height > image.Width) {
                            float ratio = (float)image.Width / (float)image.Height;
                            width = this.pictureBox.Width;
                            height = (int)((float)image.Height * (float)ratio);
                        } else if (image.Width > image.Height) {
                            float ratio = (float)image.Height / (float)image.Width;
                            height = this.pictureBox.Height;
                            width = (int)((float)image.Width * (float)ratio);
                        } else if (image.Width == image.Height) {
                            height = this.pictureBox.Height;
                            width = height;
                        }

                        image = image.GetThumbnailImage(width, height, null, IntPtr.Zero);
                    } catch {
                        int index = IconManager.GetImageIndex(resource, Settings.Instance.ImageList);

                        if (index > -1) {
                            image = Settings.Instance.ImageList.Images[index];
                        }
                    }
                };

                backgroundWorker.RunWorkerCompleted += delegate {
                    if (InvokeRequired) {
                        Invoke((MethodInvoker)delegate {
                            UpdateText(sb.ToString());
                            UpdateImage(image);
                        });
                    } else {
                        UpdateText(sb.ToString());
                        UpdateImage(image);
                    }
                };

                backgroundWorker.RunWorkerAsync();
            }            
        }

        public void UpdateText(string text) {
            label.Text = text;
        }

        public void UpdateImage(Image image) {
            this.pictureBox.Visible = true;
            this.pictureBox.Image = image;

            this.pictureBox.Size = image.Size;
            this.label.Location = new Point(this.pictureBox.Location.X + this.pictureBox.Width + 1, 
                this.pictureBox.Location.Y + 2);
        }

        public void ClearImage() {
            this.pictureBox.Visible = false;
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.label = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(4, 4);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(100, 110);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(111, 4);
            this.label.Name = "label";
            this.label.TabIndex = 1;
            // 
            // PreviewPanel
            // 
            this.Dock = DockStyle.Fill;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.label);
            this.Controls.Add(this.pictureBox);
            this.Name = "PreviewPanel";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}