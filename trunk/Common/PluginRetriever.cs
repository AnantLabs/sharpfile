using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common.Logger;

namespace Common {
	public class PluginRetriever {
		private LoggerService loggerService;

		public PluginRetriever() {
		}

		public PluginRetriever(LoggerService loggerService) {
			this.loggerService = loggerService;
		}

		/// <summary>
		/// Gets plugin files from the default "plugins" path.
		/// </summary>
		/// <returns></returns>
		public List<string> GetPluginFiles() {
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			string assemblyPath = string.Format("{0}plugins{1}",
				AppDomain.CurrentDomain.BaseDirectory,
				System.IO.Path.DirectorySeparatorChar);

			return GetPluginFiles(assemblyPath);
		}

		/// <summary>
		/// Get plugin files from a specific plugin path.
		/// </summary>
		/// <param name="pluginPath"></param>
		/// <returns></returns>
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
		// or require the user to go to a settings section and hit a button to refresh the loaded assemblies...

		/// <summary>
		/// Loads the plugin dll from disk and returns a list of assemblies.
		/// </summary>
		/// <returns></returns>
		public List<Assembly> GetPluginAssemblies() {
			List<Assembly> assemblies = new List<Assembly>();
			List<string> files = GetPluginFiles();

			// TODO: Only load plugin files and not all dll's.
			foreach (string file in files) {
				try {
					Assembly assembly = Assembly.LoadFile(file);
					assemblies.Add(assembly);
				} catch (Exception ex) {
					if (loggerService != null) {
						loggerService.Log(LogLevelType.ErrorsOnly, ex,
							"Assembly, {0}, could not be loaded.",
							file);
					}
				}
			}

			return assemblies;
		}
	}
}