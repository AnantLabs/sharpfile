﻿using System.Windows.Forms;
using SharpFile.Infrastructure;

namespace SharpFile.UI {
    public partial class CommandLinePanel : UserControl, IPluginPanel {
        public CommandLinePanel() {
            InitializeComponent();

            this.SizeChanged += delegate {
                this.txtResults.Width = this.Width - 10;
                this.txtCommandLine.Width = this.Width - 10;
            };
        }

        public void Update(IView view) {
            this.txtCommandLine.UpdateText(view);
            Refresh();
        }
    }
}