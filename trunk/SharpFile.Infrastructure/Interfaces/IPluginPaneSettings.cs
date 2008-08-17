namespace SharpFile.Infrastructure {
    public interface IPluginPaneSettings {
        /// <summary>
        /// Checks specific settings to see if they need default values for any reason and spplies them.
        /// This has to be done after the method is deserialized and not in the actual getter of the 
        /// property because of the way serialization works.
        /// </summary>
        void GenerateDefaultSettings();
    }
}