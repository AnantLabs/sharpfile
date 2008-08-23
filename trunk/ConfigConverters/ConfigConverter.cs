namespace SharpFile.ConfigConverters {
    /// <summary>
    /// Abstract class that provides the ability to convert config settings from one version to another.
    /// </summary>
    public abstract class ConfigConverter {
        protected string settingsVersion;
        protected string assemblyVersion;
        protected string message = @"
The current configuration file version is {0}. SharpFile's current version is {1}. 
Settings have changed between the two versions, so any custom settings will be lost. 
Press OK to continue loading the program even though settings will be lost.
Press CANCEL to exit SharpFile.";

        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="settingsVersion">Version number stored in the config file.</param>
        /// <param name="assemblyVersion">Version associated with the current assembly.</param>
        public ConfigConverter(string settingsVersion, string assemblyVersion) {
            this.settingsVersion = settingsVersion;
            this.assemblyVersion = assemblyVersion;
        }

        /// <summary>
        /// Convert the config file from one format to another.
        /// </summary>
        public abstract void Convert();

        /// <summary>
        /// Retrieves the message shown to the user in the UI.
        /// </summary>
        /// <returns>The message.</returns>
        public virtual string GetMessage() {
            return string.Format(message,
                settingsVersion,
                assemblyVersion);
        }
    }
}