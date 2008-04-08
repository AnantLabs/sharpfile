namespace SharpFile.Infrastructure {
    /// <summary>
    /// Specifies what "mode" the view should be.
    /// </summary>
    public enum ParentType {
        /// <summary>
        /// Every view is its own child window.
        /// </summary>
        //Mdi,
        /// <summary>
        /// Two views that fill up the parent window. The default.
        /// </summary>
        Dual
    }
}