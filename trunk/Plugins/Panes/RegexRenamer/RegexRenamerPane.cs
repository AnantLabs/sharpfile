// RegexRenamerPane - MG - v0.1Alpha

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.Attributes;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.UI {
    [PluginAttribute(
        Author = "MG",
        Description = "Rename several files with regex.",
        Version = "0.1")]
    public partial class RegexRenamerPane : DockContent, IPluginPane {
        private bool isActivated = true;
        private System.ComponentModel.IContainer components;
        private StringBuilder sb = new StringBuilder();
        private Panel panel1;
        private SplitContainer splitContainer1;
        private Label label3;
        private Label label2;
        private Label label1;
        private Button btnRename;
        private TextBox txtReplacePattern;
        private ToolTip toolTip1;
        private Button btnTest;
        private TextBox txtPattern;
        private TextBox txtFolder;
        private SplitContainer splitContainer2;
        private ListBox lbOldFileNames;
        private ListBox lbNewFileNames;
        private ErrorProvider errorProvider;
    
        delegate void TaskWorker(BackgroundWorker bw);
        int m_Errors = 0;

        public RegexRenamerPane()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing) 
        {
            base.Dispose(disposing);
        }

        public void Update(IView view) 
        {
            if (view.Path != null) {
                txtFolder.Text = view.Path.FullName;
                ClearForm();
                SetupBackgroundWorker(new TaskWorker(Trial));
            }
        }

        private void btnTrial_Click_1(object sender, EventArgs e)
        {
            ClearForm();
            SetupBackgroundWorker(new TaskWorker(Trial));
        }

        private void btnRename_Click_1(object sender, EventArgs e)
        {
            ClearForm();
            SetupBackgroundWorker(new TaskWorker(Rename));
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            TaskWorker tw = e.Argument as TaskWorker;
            BackgroundWorker bw = sender as BackgroundWorker;
            if (tw != null)
                tw(bw);
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableForm(true);
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            bool error = false;
            object[] args = e.UserState as object[];
            if (args.Length > 1)
            {
                if ((bool)args[1] == true)
                {
                    error = true;
                    m_Errors++;
                }
            }

            KeyValuePair<string, string> kvp = (KeyValuePair<string, string>)args[0];
            lbOldFileNames.Items.Add(Path.GetFileName(kvp.Key));

            if (!error)
                lbNewFileNames.Items.Add(Path.GetFileName(kvp.Value));
            else
                lbNewFileNames.Items.Add((string)args[2]);
        }

        private void Trial(BackgroundWorker bw)
        {
            Dictionary<string, string> files = GetList();
            int done = 0;
            foreach (KeyValuePair<string, string> kvp in files)
            {
                done++;
                bw.ReportProgress(done * 100 / files.Count, new object[] { kvp });
            }
        }

        private void Rename(BackgroundWorker bw)
        {
            Dictionary<string, string> files = GetList();
            int done = 0;
            foreach (KeyValuePair<string, string> kvp in files)
            {
                string errMsg = null;
                bool error = false;
                try
                {
                    done++;
                    File.Move(kvp.Key, kvp.Value);
                }
                catch (Exception ex)
                {
                    error = true;
                    errMsg = "Error : " + ex.Message + " : " + kvp.Value;
                }
                bw.ReportProgress(done * 100 / files.Count, new object[] { kvp, error, errMsg });
            }
        }

        Dictionary<string, string> GetList()
        {
            Dictionary<string, string> files = new Dictionary<string, string>();
            foreach (string file in Directory.GetFiles(txtFolder.Text))
            {
                if (Regex.IsMatch(file, txtPattern.Text))
                    files.Add(file, Regex.Replace(file, txtPattern.Text, txtReplacePattern.Text));
            }
            return files;
        }

        void SetupBackgroundWorker(TaskWorker tw)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync(new TaskWorker(tw));
        }

        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            StringBuilder msg = new StringBuilder("Exception occured");
            msg.AppendLine(e.Exception.Message);
            MessageBox.Show(
                msg.ToString(),
                "ThreadException",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private void ClearForm()
        {
            lbNewFileNames.Items.Clear();
            lbOldFileNames.Items.Clear();
            m_Errors = 0;
            EnableForm(false);
        }

        private void EnableForm(bool flag)
        {
            btnRename.Enabled = flag;
            btnTest.Enabled = flag;
        }

        private void lbOldFileNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbNewFileNames.SelectedIndex = lbOldFileNames.SelectedIndex;
            lbNewFileNames.TopIndex = lbOldFileNames.TopIndex;
        }

        private void lbNewFileNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbOldFileNames.SelectedIndex = lbNewFileNames.SelectedIndex;
            lbOldFileNames.TopIndex = lbNewFileNames.TopIndex;
        }

        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRename = new System.Windows.Forms.Button();
            this.txtReplacePattern = new System.Windows.Forms.TextBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.txtPattern = new System.Windows.Forms.TextBox();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lbOldFileNames = new System.Windows.Forms.ListBox();
            this.lbNewFileNames = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(893, 565);
            this.panel1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btnRename);
            this.splitContainer1.Panel1.Controls.Add(this.txtReplacePattern);
            this.splitContainer1.Panel1.Controls.Add(this.btnTest);
            this.splitContainer1.Panel1.Controls.Add(this.txtPattern);
            this.splitContainer1.Panel1.Controls.Add(this.txtFolder);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(893, 565);
            this.splitContainer1.SplitterDistance = 125;
            this.splitContainer1.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Replace Regex:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Input Regex:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Folder:";
            // 
            // btnRename
            // 
            this.btnRename.Location = new System.Drawing.Point(177, 90);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(75, 23);
            this.btnRename.TabIndex = 4;
            this.btnRename.Text = "Rename";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click_1);
            // 
            // txtReplacePattern
            // 
            this.txtReplacePattern.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReplacePattern.Location = new System.Drawing.Point(96, 64);
            this.txtReplacePattern.Name = "txtReplacePattern";
            this.txtReplacePattern.Size = new System.Drawing.Size(288, 20);
            this.txtReplacePattern.TabIndex = 2;
            this.toolTip1.SetToolTip(this.txtReplacePattern, "Enter the match Regex such as:\r\nNewName${file}");
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(96, 90);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 23);
            this.btnTest.TabIndex = 3;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTrial_Click_1);
            // 
            // txtPattern
            // 
            this.txtPattern.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPattern.Location = new System.Drawing.Point(96, 38);
            this.txtPattern.Name = "txtPattern";
            this.txtPattern.Size = new System.Drawing.Size(288, 20);
            this.txtPattern.TabIndex = 1;
            this.toolTip1.SetToolTip(this.txtPattern, "Enter the input Regex such as:\r\n(?<file>\\w+[.]jpg)");
            // 
            // txtFolder
            // 
            this.txtFolder.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtFolder.Enabled = false;
            this.txtFolder.Location = new System.Drawing.Point(96, 12);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(288, 20);
            this.txtFolder.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lbOldFileNames);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lbNewFileNames);
            this.splitContainer2.Size = new System.Drawing.Size(893, 436);
            this.splitContainer2.SplitterDistance = 442;
            this.splitContainer2.TabIndex = 0;
            // 
            // lbOldFileNames
            // 
            this.lbOldFileNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbOldFileNames.FormattingEnabled = true;
            this.lbOldFileNames.Location = new System.Drawing.Point(0, 0);
            this.lbOldFileNames.Name = "lbOldFileNames";
            this.lbOldFileNames.Size = new System.Drawing.Size(442, 433);
            this.lbOldFileNames.TabIndex = 0;
            // 
            // lbNewFileNames
            // 
            this.lbNewFileNames.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbNewFileNames.FormattingEnabled = true;
            this.lbNewFileNames.Location = new System.Drawing.Point(0, 0);
            this.lbNewFileNames.Name = "lbNewFileNames";
            this.lbNewFileNames.Size = new System.Drawing.Size(447, 433);
            this.lbNewFileNames.TabIndex = 0;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // RegexRenamerPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(893, 565);
            this.Controls.Add(this.panel1);
            this.Name = "RegexRenamerPane";
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
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
    }
}
