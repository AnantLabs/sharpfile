using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using SharpFile.IO.ChildResources;

namespace SharpFile.UI {
    public partial class PreviewPanel : UserControl {
        private IResource resource;
        private PictureBox pictureBox;
        private TextBox textBox;

        public PreviewPanel() {
            InitializeComponent();

            this.SizeChanged += delegate {
                Image image = getImageFromResource();
                updateImage(image);
            };
        }

        public void Update(IResource resource) {
            this.resource = resource;

            StringBuilder sb = new StringBuilder();
            Image image = null;

            formatName(sb);
            getDetailTextFromResource(sb);
            image = getImageFromResource();

            updateText(sb.ToString());
            updateImage(image);
        }

        private void updateText(string text) {
            textBox.Text = text;
        }

        private void updateImage(Image image) {
            if (image != null) {
                this.pictureBox.Visible = true;
                this.pictureBox.Image = image;

                this.pictureBox.Size = new Size(image.Size.Width + 5, image.Size.Height);
            } else {
                this.pictureBox.Visible = false;
            }
        }

        private void formatName(StringBuilder sb) {
            Common.Templater templater = new Common.Templater(resource);
            string result = templater.Generate(Settings.Instance.PreviewPane.NameFormat);

            sb.AppendFormat("{0}{1}",
                    result,
                    Environment.NewLine);
        }

        private void getDetailTextFromResource(StringBuilder sb) {
            if (resource != null && resource is FileInfo) {
                FileInfo fileInfo = (FileInfo)resource;

                bool generateDetailText = (Settings.Instance.PreviewPane.AlwaysShowDetailText ||
                    Settings.Instance.PreviewPane.DetailTextExtensions.Contains(fileInfo.Extension.ToLower()));

                if (generateDetailText) {
                    System.IO.StreamReader sr = null;

                    try {
                        sr = System.IO.File.OpenText(resource.FullName);

                        for (int i = 0; i < Settings.Instance.PreviewPane.MaximumLinesOfDetailText; i++) {
                            if (sr.EndOfStream) {
                                break;
                            } else {
                                string line = sr.ReadLine();
                                sb.Append(line);
                                sb.Append(Environment.NewLine);

                                if (i == Settings.Instance.PreviewPane.MaximumLinesOfDetailText - 1) {
                                    sb.Append("...");
                                }
                            }
                        }
                    } catch {
                        // Ignore any exceptions.
                    } finally {
                        if (sr != null) {
                            sr.Dispose();
                        }
                    }
                }
            }
        }

        private Image getImageFromResource() {
            Image image = null;

            if (resource != null) {
                if (Settings.Instance.PreviewPane.ThumbnailImages) {
                    try {
                        image = Image.FromFile(resource.FullName);
                        int width = 0;
                        int height = 0;

                        if (image.Width == image.Height) {
                            height = this.Height;
                            width = height;
                        } else {
                            float ratio = 1;

                            if (image.Height > image.Width) {
                                ratio = (float)image.Width / (float)image.Height;
                            } else {
                                ratio = (float)image.Height / (float)image.Width;
                            }

                            height = this.Height;
                            width = (int)((float)image.Width * (float)ratio);
                        }

                        if (image.Width > width || image.Height > height) {
                            image = image.GetThumbnailImage(width, height, null, IntPtr.Zero);
                        }
                    } catch {
                        // Catch any problems with retrieving the thumbnail.
                    }
                }

                if (image == null) {
                    int index = IconManager.GetImageIndex(resource, Settings.Instance.ImageList);

                    if (index > -1) {
                        image = Settings.Instance.ImageList.Images[index];
                    }
                }
            }

            return image;
        }

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.textBox = new System.Windows.Forms.TextBox();

            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(100, 110);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.Dock = DockStyle.Left;
            // 
            // label
            // 
            this.textBox.AutoSize = true;
            this.textBox.Location = new System.Drawing.Point(111, 0);
            this.textBox.Name = "label";
            this.textBox.TabIndex = 1;
            this.textBox.WordWrap = true;
            this.textBox.ReadOnly = true;
            this.textBox.Multiline = true;
            this.textBox.BorderStyle = BorderStyle.None;
            this.textBox.ScrollBars = ScrollBars.Both;
            this.textBox.Dock = DockStyle.Fill;
            this.textBox.BackColor = Color.White;
            // 
            // PreviewPanel
            // 
            this.Dock = DockStyle.Fill;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.pictureBox);
            this.Name = "PreviewPanel";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}