using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Common.Logger;

namespace Common {
	/// <summary>
	/// Settings singleton.
	/// Number 4 from: http://www.yoda.arachsys.com/csharp/singleton.html.
	/// </summary>
	[Serializable]
	public class SettingsBase {
		public const string FilePath = "settings.config";
		private static object lockObject = new object();
		private string version = Assembly.GetEntryAssembly().GetName().Version.ToString();
		private SettingsSection.Logger loggerSettings;
		private LoggerService loggerService;
		private List<XmlElement> unknownConfigurationXmlElements;

		/// <summary>
		/// Explicit static ctor to load settings and to 
		/// tell C# compiler not to mark type as beforefieldinit.
		/// </summary>
		static SettingsBase() {
			lockObject = new object();
		}

		/// <summary>
		/// Internal instance ctor.
		/// </summary>
		protected SettingsBase() {
			loggerSettings = new SettingsSection.Logger();			
			unknownConfigurationXmlElements = new List<XmlElement>();
		}

		/// <summary>
		/// Loads the settings config file into the singleton instance.
		/// </summary>
		public static void Load<T>(T instance) where T : SettingsBase {
			lock (lockObject) {
				try {
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					FileInfo fileInfo = new FileInfo(FilePath);

					// If there is no settings file, create one from some defaults.
					if (fileInfo.Exists && fileInfo.Length > 0) {
						// Set our instance properties from the Xml file.
						retrieveSettings<T>(instance, xmlSerializer);
					}
				} catch (Exception ex) {
					LoggerService loggerService = new LoggerService("log.txt", LogLevelType.Verbose);
					loggerService.Log(LogLevelType.Verbose, ex,
						"There was an error generating the settings.");

					throw;
				}
			}
		}

		/// <summary>
		/// Checks to see whether the current assembly version matches the version specified in the settings.
		/// </summary>
		/// <param name="settingsVersion">Version specified in the settings.</param>
		/// <param name="assemblyVersion">Assembly-specified version.</param>
		/// <returns>Whether the two versions are the same.</returns>
		public static bool CompareSettingsVersion(ref string settingsVersion, ref string assemblyVersion) {
			FileInfo fileInfo = new FileInfo(FilePath);

			// If there is no settings file, create one from some defaults.
			if (fileInfo.Exists && fileInfo.Length > 0) {
				try {
					XmlDocument xml = new XmlDocument();
					xml.Load(fileInfo.FullName);

					assemblyVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
					settingsVersion = xml.SelectSingleNode("/Settings/Version").InnerText;

					if (assemblyVersion != settingsVersion) {
						return false;
					}
				} catch {
					// Ignore any xml exceptions here. They will be caught when the file attempts to deserialize.
				}
			}

			return true;
		}

		/// <summary>
		/// Persists the setttings to the config file.
		/// </summary>
		public static void Save<T>(T instance) where T : SettingsBase {
			lock (lockObject) {
				try {
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

					using (TextWriter tw = new StreamWriter(FilePath)) {
						xmlSerializer.Serialize(tw, instance);
					}

					instance.SaveSettings();
				} catch (Exception ex) {
					instance.loggerService.Log(LogLevelType.ErrorsOnly, ex, "Error when saving settings.");
				}
			}
		}

		/// <summary>
		/// Designed to be overriden for any settings that need to be saved by inheriting classes.
		/// </summary>
		protected virtual void SaveSettings() {
		}

		/// <summary>
		/// Clears the config file.
		/// </summary>
		public static void Clear<T>(T instance) where T : SettingsBase {
			if (File.Exists(FilePath)) {
				File.Delete(FilePath);
				instance = default(T);
			}
		}

		/// <summary>
		/// Deserializes the settings config file to an object and sets the singleton's properties appropriately.
		/// </summary>
		/// <param name="xmlSerializer">XmlSerializer to deserialize.</param>
		private static void retrieveSettings<T>(T instance, XmlSerializer xmlSerializer) where T : SettingsBase {
			// Add any unknown elements to a list.
			// This list will be used later for plugin pane settings.
			xmlSerializer.UnknownElement += delegate(object sender, XmlElementEventArgs e) {
				instance.unknownConfigurationXmlElements.Add(e.Element);
			};

			// Deserialize the settings from the xml serializer.
			using (TextReader tr = new StreamReader(FilePath)) {
				T settings = (T)xmlSerializer.Deserialize(tr);
				Reflection.DuplicateObject<T>(settings, instance);
			}
		}

		/// <summary>
		/// Version of the entry assembly.
		/// </summary>
		public string Version {
			get {
				return version;
			}
			set {
				version = value;
			}
		}

		/// <summary>
		/// Logger info.
		/// </summary>
		[XmlElement("Logger")]
		public SettingsSection.Logger LoggerSettings {
			get {
				return loggerSettings;
			}
			set {
				loggerSettings = value;
			}
		}

		/// <summary>
		/// Derived Logger service.
		/// </summary>
		[XmlIgnore]
		public LoggerService Logger {
			get {
				if (loggerService == null) {
					loggerService = new LoggerService(loggerSettings.File, loggerSettings.LogLevel);
				}

				return loggerService;
			}
		}

		/// <summary>
		/// Unknown elements in the config file that will be stored in case they are needed.
		/// Used to store dynamic plugin settings.
		/// </summary>
		[XmlIgnore]
		public List<XmlElement> UnknownConfigurationXmlElements {
			get {
				return unknownConfigurationXmlElements;
			}
		}
	}
}