using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.Infrastructure {
    public interface IPluginPane {
        void Update(IView view);
        void Show(DockPanel dockPanel);
        void Show(DockPanel dockPanel, DockState dockState);
        void Show(DockPane dockPane, IDockContent content);

        string Name { get; set; }
        string TabText { get; set; }
        bool AllowEndUserDocking { get; set; }
        double AutoHidePortion { get; set; }
        bool IsActivated { get; set; }
        DockState VisibleState { get; set; }        
        DockAreas DockAreas { get; set; }
        DockState DockState { get; set; }        
        DockContentHandler DockHandler { get; }
        DockStyle Dock { get; set; }
        IPluginPaneSettings Settings { get; set; }
    }
}