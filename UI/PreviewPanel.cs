using System.Drawing;
using System.Windows.Forms;

namespace SharpFile.UI {
    public partial class PreviewPanel : UserControl {
        private PictureBox pictureBox;
        private Label label;

        public PreviewPanel() {
            InitializeComponent();
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