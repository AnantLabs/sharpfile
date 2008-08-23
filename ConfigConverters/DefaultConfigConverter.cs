using System;
using SharpFile.Infrastructure;

namespace SharpFile.ConfigConverters {
    public class DefaultConfigConverter : ConfigConverter {
        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="settingsVersion">Version number stored in the config file.</param>
        /// <param name="assemblyVersion">Version associated with the current assembly.</param>
        public DefaultConfigConverter(string settingsVersion, string assemblyVersion) 
            : base(settingsVersion, assemblyVersion) {
        }

        /// <summary>
        /// Convert the config file from one format to another.
        /// </summary>
        public override void Convert() {
            Settings.Clear();
        }
    }
}