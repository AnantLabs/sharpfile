using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.Attributes;
using SharpFile.Infrastructure.IO.ChildResources;
using SharpFile.Infrastructure.SettingsSection;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.UI {
    [PluginAttribute(
        Author = "Adam Hill",
        Description = "Previews the currently selected resource. Can be configured to thumbnail images and peek inside of text files.",
        Version = "1.0")]
    public partial class PreviewPane : DockContent, IPluginPane {
        private bool isActivated = true;
        private IResource resource;
        private PictureBox pictureBox;
        private TextBox textBox;
        private Image image = null;
        private StringBuilder sb = new StringBuilder();
        private IPluginPaneSettings pluginPaneSettings;
        private PreviewPaneSettings settings = new PreviewPaneSettings();

        /// <summary>
        /// Ctor.
        /// </summary>
        public PreviewPane() {
            InitializeComponent();

            this.SizeChanged += delegate {
                if (resource != null) {
                    getImageFromResource();
                    updateImage();
                }
            };
        }

        protected override void Dispose(bool disposing) {
            if (image != null) {
                image.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the preview panel from the resource.
        /// </summary>
        /// <param name="view">View.</param>
        public void Update(IView view) {
            if (view.SelectedResource != null) {
                this.resource = view.SelectedResource;
                sb = new StringBuilder();
                image = null;

                // Grab the appropriate text and update it.
                formatName();
                getDetailTextFromResource();
                updateText();

                // Grab the appropriate image and update it.
                getImageFromResource();
                updateImage();
            }
        }

        /// <summary>
        /// Updates the text.
        /// </summary>
        /// <param name="text">Text to update.</param>
        private void updateText() {
            textBox.Text = sb.ToString();
        }

        /// <summary>
        /// Updates the image.
        /// </summary>
        /// <param name="image">Image to update.</param>
        private void updateImage() {
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
        private void formatName() {
            Templater templater = new Templater(resource);
            string result = templater.Generate(settings.NameFormatTemplate.Template);

            if (settings.NameFormatTemplate.MethodDelegate != null) {
                result = settings.NameFormatTemplate.MethodDelegate(result);
            }

            sb.AppendFormat("{0}{1}",
                    result,
                    Environment.NewLine);
        }

        /// <summary>
        /// Gets the detail text for the resource.
        /// </summary>
        /// <param name="sb">StringBuilder to append the details to.</param>
        private void getDetailTextFromResource() {
            if (resource != null && resource is FileInfo) {
                FileInfo fileInfo = (FileInfo)resource;

                bool generateDetailText = (settings.ShowDetailTextForAllExtensions ||
                    settings.DetailTextExtensions.Contains(fileInfo.Extension.ToLower()));

                if (generateDetailText) {
                    if (settings.ShowAllDetailText) {
                        try {
                            sb.Append(System.IO.File.ReadAllText(resource.FullName));
                        } catch {
                            // Ignore any exceptions.s
                        }
                    } else if (settings.MaximumLinesOfDetailText > 0) {
                        System.IO.StreamReader sr = null;

                        try {
                            sr = System.IO.File.OpenText(resource.FullName);

                            for (int i = 0; i < settings.MaximumLinesOfDetailText; i++) {
                                if (sr.EndOfStream) {
                                    break;
                                } else {
                                    string line = sr.ReadLine();
                                    sb.Append(line);
                                    sb.Append(Environment.NewLine);

                                    if (i == settings.MaximumLinesOfDetailText - 1) {
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
        }

        /// <summary>
        /// Get image from resource.
        /// </summary>
        /// <returns></returns>
        private void getImageFromResource() {
            if (resource != null) {
                if (settings.ThumbnailImages) {
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
                    } catch (System.IO.FileNotFoundException ex) {
                        // Ignore any problems with retrieving the thumbnail.
                    } catch (OutOfMemoryException ex) {
                        // Ignore any problems with retrieving the thumbnail.
                    }
                }

                // Try to grab the resource's icon if there is no thumbnail for the resource.
                if (image == null) {
                    int index = IconManager.GetImageIndex(resource, false, SharpFile.Infrastructure.Settings.Instance.ImageList);

                    if (index > -1) {
                        image = SharpFile.Infrastructure.Settings.Instance.ImageList.Images[index];
                    }
                }
            }
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

        public new bool IsActivated {
            get {
                return isActivated;
            }
            set {
                isActivated = value;

                if (isActivated) {
                    this.DockHandler.Activate();
                }
            }
        }

        public IPluginPaneSettings Settings {
            get {
                return pluginPaneSettings;
            }
            set {
                pluginPaneSettings = value;
                settings = (PreviewPaneSettings)pluginPaneSettings;
            }
        }
    }
}