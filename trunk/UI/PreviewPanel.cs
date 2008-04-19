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

        /// <summary>
        /// Ctor.
        /// </summary>
        public PreviewPanel() {
            InitializeComponent();

            this.SizeChanged += delegate {
                Image image = getImageFromResource();
                updateImage(image);
            };
        }

        /// <summary>
        /// Updates the preview panel from the resource.
        /// </summary>
        /// <param name="resource">Resource.</param>
        public void Update(IResource resource) {
            this.resource = resource;
            StringBuilder sb = new StringBuilder();
            Image image = null;

            formatName(sb);
            getDetailTextFromResource(sb);
            image = getImageFromResource();
            updateText(sb);
            updateImage(image);
        }

        /// <summary>
        /// Updates the text.
        /// </summary>
        /// <param name="text">Text to update.</param>
        private void updateText(StringBuilder text) {
            textBox.Text = text.ToString();
        }

        /// <summary>
        /// Updates the image.
        /// </summary>
        /// <param name="image">Image to update.</param>
        private void updateImage(Image image) {
            if (image != null) {
                this.pictureBox.Visible = true;
                this.pictureBox.Image = image;

                this.pictureBox.Size = new Size(image.Size.Width + 5, image.Size.Height);
            } else {
                this.pictureBox.Visible = false;
            }
        }

        /// <summary>
        /// Formats the name to use.
        /// </summary>
        /// <param name="sb"></param>
        private void formatName(StringBuilder sb) {
            Common.Templater templater = new Common.Templater(resource);
            string result = templater.Generate(Settings.Instance.PreviewPanel.NameFormat);

            sb.AppendFormat("{0}{1}",
                    result,
                    Environment.NewLine);
        }

        /// <summary>
        /// Gets the detail text for the resource.
        /// </summary>
        /// <param name="sb">StringBuilder to append the details to.</param>
        private void getDetailTextFromResource(StringBuilder sb) {
            if (resource != null && resource is FileInfo) {
                FileInfo fileInfo = (FileInfo)resource;

                bool generateDetailText = (Settings.Instance.PreviewPanel.AlwaysShowDetailText ||
                    Settings.Instance.PreviewPanel.DetailTextExtensions.Contains(fileInfo.Extension.ToLower()));

                if (generateDetailText) {
                    System.IO.StreamReader sr = null;

                    try {
                        sr = System.IO.File.OpenText(resource.FullName);

                        for (int i = 0; i < Settings.Instance.PreviewPanel.MaximumLinesOfDetailText; i++) {
                            if (sr.EndOfStream) {
                                break;
                            } else {
                                string line = sr.ReadLine();
                                sb.Append(line);
                                sb.Append(Environment.NewLine);

                                if (i == Settings.Instance.PreviewPanel.MaximumLinesOfDetailText - 1) {
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

        /// <summary>
        /// Get image from resource.
        /// </summary>
        /// <returns></returns>
        private Image getImageFromResource() {
            Image image = null;

            if (resource != null) {
                if (Settings.Instance.PreviewPanel.ThumbnailImages) {
                    try {
                        image = Image.FromFile(resource.FullName);
                        int width = 0;
                        int height = 0;

                        if (image.Width == image.Height) {
                            // If the image is square, just make the height and width equal.
                            height = this.Height;
                            width = height;
                        } else {
                            // Otherwise, calculate a ratio for the width and height.
                            float ratio = 1;

                            if (image.Height > image.Width) {
                                ratio = (float)image.Width / (float)image.Height;
                            } else {
                                ratio = (float)image.Height / (float)image.Width;
                            }

                            height = this.Height;
                            width = (int)((float)image.Width * (float)ratio);
                        }

                        // Only thumbnail the image if the size of the panel is smaller than the size of the image.
                        if (image.Width > width || image.Height > height) {
                            image = image.GetThumbnailImage(width, height, null, IntPtr.Zero);
                        }
                    } catch {
                        // Catch any problems with retrieving the thumbnail.
                    }
                }

                // Try to grab the resource's icon if ther eis no thumbnail for the resource.
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