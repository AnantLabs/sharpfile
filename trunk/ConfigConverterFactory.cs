using SharpFile.ConfigConverters;

namespace SharpFile {
    public class ConfigConverterFactory {
        /// <summary>
        /// Retrieves the correct converter based on the setting and assembly versions.
        /// </summary>
        /// <param name="settingsVersion">Version number stored in the config file.</param>
        /// <param name="assemblyVersion">Version associated with the current assembly.</param>
        /// <returns>Correct type of ConfigConverter.</returns>
        public static ConfigConverter GetConfigConverter(string settingsVersion, string assemblyVersion) {
            if (settingsVersion.Equals("0.7.6.0") && assemblyVersion.Equals("0.7.6.572")) {
                return new Convert070061208_to_076572(settingsVersion, assemblyVersion);
            } else {
                return new DefaultConfigConverter(settingsVersion, assemblyVersion);
            }
        }
    }
}