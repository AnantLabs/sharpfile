using System;
using SharpFile.Infrastructure;

namespace SharpFile.ConfigConverters {
    public class Convert070061208_to_076572 : ConfigConverter {
        /// <summary>
        /// Default ctor.
        /// </summary>
        /// <param name="settingsVersion">Version number stored in the config file.</param>
        /// <param name="assemblyVersion">Version associated with the current assembly.</param>
        public Convert070061208_to_076572(string settingsVersion, string assemblyVersion) 
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