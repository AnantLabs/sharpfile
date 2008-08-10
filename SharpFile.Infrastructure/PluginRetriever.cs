using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common.Logger;

namespace SharpFile.Infrastructure {
    public class PluginRetriever {
        public List<string> GetPluginFiles() {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            string assemblyPath = string.Format("{0}plugins{1}",
                AppDomain.CurrentDomain.BaseDirectory,
                Path.DirectorySeparatorChar);
            return GetPluginFiles(assemblyPath);
        }

        public List<string> GetPluginFiles(string pluginPath) {
            List<string> files = new List<string>();

            if (Directory.Exists(pluginPath)) {
                foreach (string file in Directory.GetFiles(pluginPath, "*.dll", SearchOption.AllDirectories)) {
                    files.Add(file);
                }
            }

            return files;
        }

        // TODO: Store the assemblies in this class and make this a part of the Settings Singleton.
        // Have a filewatcher watch the plugins directory so that any new dll dropped in the directory would automatically be loaded?
        //or require the user to go to a settings section and hit a button to refresh the loaded assemblies...

        public List<Assembly> GetPluginAssemblies() {
            List<Assembly> assemblies = new List<Assembly>();
            List<string> files = GetPluginFiles();

            // TODO: Only load plugin files and not all dll's.
            foreach (string file in files) {
                try {
                    Assembly assembly = Assembly.LoadFile(file);
                    assemblies.Add(assembly);
                } catch (Exception ex) {
                    Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                        "Assembly, {0}, could not be loaded.",
                        file);
                }
            }

            return assemblies;
        }
    }
}