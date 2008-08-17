// ScreenPane - MG - v0.1Alpha

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using WeifenLuo.WinFormsUI.Docking;
using SharpFile.Infrastructure.Attributes;

namespace SharpFile.UI {
    [PluginAttribute(
        Author = "MG",
        Description = "Control screen brightness and opacity.",
        Version = "0.1")]
    public partial class ScreenPane : DockContent, IPluginPane {
        private bool isActivated = true;
        private IResource resource;

        private Image image = null;
        private TrackBar birghtnessTrackBar;
        private Button resetButton;
        private Label label3;
        private Label label2;
        private TextBox brightnessTextBox;
        private Label label1;
        private Label label4;
        private TrackBar trackBar1;
        private Button button1;
        private Label label5;
        private Label label6;
        private TextBox textBox1;
        private StringBuilder sb = new StringBuilder();
        
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetDeviceGammaRamp(IntPtr hDc,
        [MarshalAs(UnmanagedType.LPArray)] ushort[,] lpRamp);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        IntPtr screenDC;
        IntPtr hDC;

        public ScreenPane()
        {
            InitializeComponent();
            screenDC = hDC;
        }

        protected override void Dispose(bool disposing) {
            if (image != null) {
                image.Dispose();
            }

            base.Dispose(disposing);
        }

        public bool setBrightness(int b)
        {
            IntPtr gammaDC;
            if (screenDC == IntPtr.Zero)
            {
                gammaDC = GetDC(IntPtr.Zero);
            }
            else gammaDC = screenDC;

            if (gammaDC == IntPtr.Zero) return false;

            ushort[,] gammaArray = new ushort[3, 256];

            for (int i = 0; i < 256; i++)
            {
                int arrayValue = i * (b + 128);

                if (arrayValue > 65535)
                    arrayValue = 65535;

                gammaArray[0, i] =
                    gammaArray[1, i] =
                    gammaArray[2, i] = (ushort)arrayValue;
            }

            return SetDeviceGammaRamp(gammaDC, gammaArray);
        }

        public void Update(IView view) {
            this.resource = view.SelectedResource;
            sb = new StringBuilder();
            image = null;
        }
        private void birghtnessTrackBar_Scroll(object sender, EventArgs e)
        {
            this.brightnessTextBox.Text = birghtnessTrackBar.Value.ToString();
            setBrightness(birghtnessTrackBar.Value);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            this.brightnessTextBox.Text = "128";
            birghtnessTrackBar.Value = 128;
            setBrightness(128);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.textBox1.Text = trackBar1.Value.ToString();
            this.ParentForm.Opacity = trackBar1.Value / 100f;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "100";
            trackBar1.Value = 100;
            this.ParentForm.Opacity = 1;
        }

        private void InitializeComponent() {
            this.birghtnessTrackBar = new System.Windows.Forms.TrackBar();
            this.resetButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.brightnessTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.button1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.birghtnessTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // birghtnessTrackBar
            // 
            this.birghtnessTrackBar.Location = new System.Drawing.Point(26, 16);
            this.birghtnessTrackBar.Maximum = 255;
            this.birghtnessTrackBar.Name = "birghtnessTrackBar";
            this.birghtnessTrackBar.Size = new System.Drawing.Size(200, 42);
            this.birghtnessTrackBar.TabIndex = 6;
            this.birghtnessTrackBar.TickFrequency = 20;
            this.birghtnessTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.birghtnessTrackBar.Value = 128;
            this.birghtnessTrackBar.Scroll += new System.EventHandler(this.birghtnessTrackBar_Scroll);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(151, 87);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 11;
            this.resetButton.Text = "Reset";
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(23, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 23);
            this.label3.TabIndex = 10;
            this.label3.Text = "Dark";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(197, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 23);
            this.label2.TabIndex = 9;
            this.label2.Text = "Light";
            // 
            // brightnessTextBox
            // 
            this.brightnessTextBox.BackColor = System.Drawing.Color.White;
            this.brightnessTextBox.Location = new System.Drawing.Point(110, 61);
            this.brightnessTextBox.Name = "brightnessTextBox";
            this.brightnessTextBox.ReadOnly = true;
            this.brightnessTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.brightnessTextBox.Size = new System.Drawing.Size(30, 20);
            this.brightnessTextBox.TabIndex = 7;
            this.brightnessTextBox.Text = "128";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Brightness:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(256, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Opacity:";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(270, 16);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(200, 42);
            this.trackBar1.TabIndex = 13;
            this.trackBar1.TickFrequency = 10;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Value = 100;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(395, 87);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Reset";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(433, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 23);
            this.label5.TabIndex = 16;
            this.label5.Text = "Opaque";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(256, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 23);
            this.label6.TabIndex = 15;
            this.label6.Text = "Transparent";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(354, 61);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.textBox1.Size = new System.Drawing.Size(30, 20);
            this.textBox1.TabIndex = 14;
            this.textBox1.Text = "100";
            // 
            // ScreenPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(974, 832);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.birghtnessTrackBar);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.brightnessTextBox);
            this.Name = "ScreenPane";
            ((System.ComponentModel.ISupportInitialize)(this.birghtnessTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
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
                return null;
            }
            set {
                throw new NotImplementedException("Not implemented");
            }
        }
    }
}
