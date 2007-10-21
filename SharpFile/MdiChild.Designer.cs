namespace SharpFile
{
    partial class MdiChild
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.child = new SharpFile.Child();
			this.SuspendLayout();
			// 
			// child
			// 
			this.child.Dock = System.Windows.Forms.DockStyle.Fill;
			this.child.Location = new System.Drawing.Point(0, 0);
			this.child.Name = "child";
			this.child.Size = new System.Drawing.Size(497, 379);
			this.child.TabIndex = 0;
			// 
			// MdiChild
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(497, 379);
			this.Controls.Add(this.child);
			this.Name = "MdiChild";
			this.Text = "Child";
			this.ResumeLayout(false);

        }

        #endregion

		private Child child;



	}
}