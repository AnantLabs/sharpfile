using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.Infrastructure {
    public interface IPluginPane {
        void Update(IView view);
        void Show(DockPanel dockPanel, DockState dockState);
        void Show(DockPane dockPane, IDockContent content);
        void GiveUpFocus();
        string Name { get; set; }
        DockState VisibleDockState { get; set; }
        DockState VisibleState { get; set; }
        double AutoHidePortion { get; set; }
        string TabText { get; set; }
    }
}