using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Common;
using Common.Logger;
using SharpFile.Infrastructure.Interfaces;
using SharpFile.Infrastructure.SettingsSection;
using Logger = Common.SettingsSection.Logger;

namespace SharpFile.Infrastructure {
	/// <summary>
	/// Settings singleton.
	/// Number 4 from: http://www.yoda.arachsys.com/csharp/singleton.html.
	/// </summary>
	[Serializable]
	public sealed class Settings : Common.SettingsBase {
		private static Settings instance = new Settings();

		private ImageList imageList = new ImageList();
		private ParentType parentType = ParentType.Dual;
		private int width = 500;
		private int height = 500;
		private bool minimizeToSystray = false;
		private bool directoriesSortedFirst = true;
		private bool showParentDirectory = true;
		private bool showRootDirectory = true;
		private FormWindowState windowState = FormWindowState.Normal;
		private Point location = new Point(0, 0);

		// Sub-settings.
		private DualParent dualParentSettings;
		private Icons iconSettings;
		private Retrievers retrieverSettings;
		private FontInfo fontInfo;
		private ViewInfo viewInfo;
		private PluginPanes pluginPaneSettings;

		// Constructed from settings.
		private List<IParentResourceRetriever> parentResourceRetrievers;
		private Font font;
		private System.Windows.Forms.View view;

		/// <summary>
		/// Explicit static ctor to load settings and to 
		/// tell C# compiler not to mark type as beforefieldinit.
		/// </summary>
		static Settings() {
			Load(instance);
		}

		/// <summary>
		/// Internal instance ctor.
		/// </summary>
		private Settings()
			: base() {
			dualParentSettings = new DualParent();
			iconSettings = new Icons();
			retrieverSettings = new Retrievers();
			fontInfo = new FontInfo();
			viewInfo = new ViewInfo();
			pluginPaneSettings = new PluginPanes();

			ImageList.ColorDepth = ColorDepth.Depth32Bit;
		}

		protected override void SaveSettings() {
			savePluginPaneSettings();
			base.SaveSettings();
		}

		/// <summary>
		/// Saves the dynamic PluginPane setting's serialized output to the config file.
		/// </summary>
		private static void savePluginPaneSettings() {
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(FilePath);

			//Create our own namespaces for the output and empty namespaces.
			XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);

			foreach (IPluginPane pluginPane in Instance.PluginPanes.Instances) {
				if (pluginPane.Settings != null) {
					try {
						Type type = pluginPane.Settings.GetType();
						XmlSerializer pluginPaneSerializer = new XmlSerializer(type);

						using (StringWriter sw = new StringWriter()) {
							pluginPaneSerializer.Serialize(sw, pluginPane.Settings, namespaces);

							string serializedOutput = sw.ToString();
							XmlDocument serializedXmlDocument = new XmlDocument();
							serializedXmlDocument.LoadXml(serializedOutput);

							// Get the position that the settings xml should be inserted into.
							string xPathQuery = string.Format("Settings/PluginPanes/Panes/Pane/SettingsType[@Type='{0}']",
								type.FullName);
							XmlNode settingsTypeXmlNode = xmlDocument.SelectSingleNode(xPathQuery);

							if (settingsTypeXmlNode != null && serializedXmlDocument.ChildNodes.Count > 0) {
								// Skip the root node and go to straight to the good stuff 
								// and then import it into the current xml document.
								XmlNode serializedXmlNode = serializedXmlDocument.ChildNodes[1];
								XmlNode importedXmlNode = xmlDocument.ImportNode(serializedXmlNode, true);

								// The parent node is required for the InsertAfter method.
								XmlNode parentNodeToInsertAfter = settingsTypeXmlNode.ParentNode;
								parentNodeToInsertAfter.InsertAfter(importedXmlNode, settingsTypeXmlNode);

								// Save the xml file back out to disk.
								xmlDocument.Save(FilePath);
							}
						}
					} catch (Exception ex) {
						Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, "Error when saving plugin settings for {0} and settings type {1}.",
							pluginPane.Name,
							pluginPane.Settings.GetType().FullName);
					}
				}
			}
		}

		/// <summary>
		/// Singleton instance of the settings.
		/// </summary>
		public static Settings Instance {
			get {
				return instance;
			}
		}

		/// <summary>
		/// Parent type.
		/// </summary>
		[XmlIgnore]
		public ParentType ParentType {
			get {
				return parentType;
			}
			set {
				parentType = value;
			}
		}

		/// <summary>
		/// Width of the form.
		/// </summary>
		public int Width {
			get {
				return width;
			}
			set {
				width = value;
			}
		}

		/// <summary>
		/// Height of the form.
		/// </summary>
		public int Height {
			get {
				return height;
			}
			set {
				height = value;
			}
		}

		/// <summary>
		/// Whether or not to minimize the application to the systray.
		/// </summary>
		public bool MinimizeToSystray {
			get {
				return minimizeToSystray;
			}
			set {
				minimizeToSystray = value;
			}
		}

		/// <summary>
		/// Whether or not the directories are sorted first.
		/// </summary>
		public bool DirectoriesSortedFirst {
			get {
				return directoriesSortedFirst;
			}
			set {
				directoriesSortedFirst = value;
			}
		}

		/// <summary>
		/// Whether or not to show the parent directory.
		/// </summary>
		public bool ShowParentDirectory {
			get {
				return showParentDirectory;
			}
			set {
				showParentDirectory = value;
			}
		}

		/// <summary>
		/// Whether or not to show the root directory.
		/// </summary>
		public bool ShowRootDirectory {
			get {
				return showRootDirectory;
			}
			set {
				showRootDirectory = value;
			}
		}

		/// <summary>
		/// The form window state.
		/// </summary>
		public FormWindowState WindowState {
			get {
				return windowState;
			}
			set {
				windowState = value;
			}
		}

		/// <summary>
		/// The location of the form.
		/// </summary>
		public Point Location {
			get {
				return location;
			}
			set {
				location = value;
			}
		}

		/// <summary>
		/// View information to derive a System.Windows.Forms.View object.
		/// </summary>
		public ViewInfo ViewInfo {
			get {
				return viewInfo;
			}
			set {
				viewInfo = value;
			}
		}

		/// <summary>
		/// Font information to derive a System.Drawing.Font object.
		/// </summary>
		public FontInfo FontInfo {
			get {
				return fontInfo;
			}
			set {
				fontInfo = value;
			}
		}

		/// <summary>
		/// Resource retriever infos.
		/// </summary>
		[XmlElement("Retrievers")]
		public Retrievers RetrieverSettings {
			get {
				if (retrieverSettings.ParentResourceRetrievers.Count == 0) {
					retrieverSettings.ParentResourceRetrievers = Retrievers.GenerateDefaultParentResourceRetrievers();
				}

				if (retrieverSettings.ChildResourceRetrievers.Count == 0) {
					retrieverSettings.ChildResourceRetrievers = Retrievers.GenerateDefaultChildResourceRetrievers();
				}

				if (retrieverSettings.Views.Count == 0) {
					retrieverSettings.Views = Retrievers.GenerateDefaultViews();
				}

				return retrieverSettings;
			}
			set {
				retrieverSettings = value;
			}
		}

		/// <summary>
		/// Dual parent settings.
		/// </summary>
		[XmlElement("DualParent")]
		public DualParent DualParent {
			get {
				if (dualParentSettings.Tools.Count == 0) {
					dualParentSettings.Tools = DualParent.GenerateDefaultTools();
				}

				return dualParentSettings;
			}
			set {
				dualParentSettings = value;
			}
		}

		/// <summary>
		/// Plugin panes settings.
		/// </summary>
		public PluginPanes PluginPanes {
			get {
				if (pluginPaneSettings.Panes.Count == 0) {
					pluginPaneSettings.Panes = PluginPanes.GenerateDefaultPluginPanes();
				}

				return pluginPaneSettings;
			}
			set {
				pluginPaneSettings = value;
			}
		}

		/// <summary>
		/// Icon settings.
		/// </summary>
		[XmlElement("Icons")]
		public Icons IconSettings {
			get {
				if (iconSettings.RetrieveIconExtensions.Count == 0) {
					iconSettings.RetrieveIconExtensions = Icons.GenerateDefaultRetrieveIconExtensions();
				}

				if (iconSettings.IntensiveSearchDriveTypeEnums.Count == 0) {
					iconSettings.IntensiveSearchDriveTypeEnums = Icons.GenerateDefaultIntensiveSearchDriveTypeEnums();
				}

				return iconSettings;
			}
			set {
				iconSettings = value;
			}
		}

		[XmlIgnore]
		public Font Font {
			get {
				if (font == null) {
					font = new Font(fontInfo.FamilyName, fontInfo.Size, fontInfo.Style);
				}

				return font;
			}
		}

		[XmlIgnore]
		public System.Windows.Forms.View View {
			get {
				if (!view.ToString().Equals(viewInfo.Type)) {
					if (Enum.IsDefined(typeof(System.Windows.Forms.View), viewInfo.Type)) {
						view = (System.Windows.Forms.View)Enum.Parse(typeof(System.Windows.Forms.View), viewInfo.Type);
					} else {
						Logger.Log(LogLevelType.ErrorsOnly, "{0} is not a valid view type. Setting view type to default of Details.",
							viewInfo.Type);
						view = System.Windows.Forms.View.Details;
						viewInfo.Type = view.ToString();
					}
				}

				return view;
			}
		}

		/// <summary>
		/// Derived parent resources.
		/// </summary>
		[XmlIgnore]
		public List<IParentResource> ParentResources {
			get {
				List<IParentResource> resources = new List<IParentResource>();

				foreach (IParentResourceRetriever retriever in ParentResourceRetrievers) {
					resources.AddRange(retriever.Get());
				}

				return resources;
			}
		}

		/// <summary>
		/// Derived parent resource retrievers.
		/// </summary>
		[XmlIgnore]
		public List<IParentResourceRetriever> ParentResourceRetrievers {
			get {
				if (parentResourceRetrievers == null || parentResourceRetrievers.Count == 0) {
					parentResourceRetrievers = new List<IParentResourceRetriever>(retrieverSettings.ParentResourceRetrievers.Count);

					foreach (ParentResourceRetriever parentResourceRetrieverSetting in retrieverSettings.ParentResourceRetrievers) {
						try {
							IParentResourceRetriever parentResourceRetriever = Reflection.InstantiateObject<IParentResourceRetriever>(
								parentResourceRetrieverSetting.FullyQualifiedType.Assembly,
								parentResourceRetrieverSetting.FullyQualifiedType.Type);

							foreach (string childResourceRetrieverName in parentResourceRetrieverSetting.ChildResourceRetrievers) {
								SettingsSection.ChildResourceRetriever childResourceRetrieverInfo = retrieverSettings.ChildResourceRetrievers.Find(delegate(SettingsSection.ChildResourceRetriever c) {
									return c.Name == childResourceRetrieverName;
								});

								if (childResourceRetrieverInfo != null) {
									try {
										IChildResourceRetriever childResourceRetriever = Reflection.InstantiateObject<IChildResourceRetriever>(
											childResourceRetrieverInfo.FullyQualifiedType.Assembly,
											childResourceRetrieverInfo.FullyQualifiedType.Type);

										childResourceRetriever.Name = childResourceRetrieverName;
										childResourceRetriever.ColumnInfos = childResourceRetrieverInfo.ColumnInfos;

										if (childResourceRetrieverInfo.FilterMethod is Interfaces.ChildResourceRetriever.FilterMethodWithArgumentsDelegate) {
											childResourceRetriever.FilterMethodWithArguments += (Interfaces.ChildResourceRetriever.FilterMethodWithArgumentsDelegate)childResourceRetrieverInfo.FilterMethod;
											childResourceRetriever.FilterMethodArguments = childResourceRetrieverInfo.FilterMethodArguments;
										} else if (childResourceRetrieverInfo.FilterMethod is Interfaces.ChildResourceRetriever.FilterMethodDelegate) {
											childResourceRetriever.FilterMethod += (Interfaces.ChildResourceRetriever.FilterMethodDelegate)childResourceRetrieverInfo.FilterMethod;
										}

										SettingsSection.View viewSetting = retrieverSettings.Views.Find(delegate(SettingsSection.View v) {
											return v.Name == childResourceRetrieverInfo.View;
										});

										if (viewSetting != null) {
											try {
												IView view = Reflection.InstantiateObject<IView>(
													viewSetting.FullyQualifiedType.Assembly,
													viewSetting.FullyQualifiedType.Type);

												if (viewSetting.Comparer != null) {
													view.Comparer = viewSetting.Comparer;
												}

												childResourceRetriever.View = view;
											} catch (TypeLoadException ex) {
												Logger.Log(LogLevelType.ErrorsOnly,
													"{0} is not derived from IView.",
													viewSetting.Name);
											}
										} else {
											Logger.Log(LogLevelType.ErrorsOnly,
												"{0} could not be found.",
												childResourceRetrieverInfo.View);
										}

										if (parentResourceRetriever.ChildResourceRetrievers == null) {
											parentResourceRetriever.ChildResourceRetrievers = new ChildResourceRetrievers();
										}

										parentResourceRetriever.ChildResourceRetrievers.Add(childResourceRetriever);
									} catch (MissingMethodException ex) {
										Logger.Log(LogLevelType.ErrorsOnly, ex,
											"ChildResourceRetriever, {0}, could not be instantiated.",
											childResourceRetrieverName);
									} catch (TypeLoadException ex) {
										Logger.Log(LogLevelType.ErrorsOnly, ex,
											"ChildResourceRetriever, {0}, could not be instantiated.",
											childResourceRetrieverName);
									}
								} else {
									Logger.Log(LogLevelType.ErrorsOnly,
										"ChildResourceRetriever, {0}, could not be found.",
										childResourceRetrieverName);
								}
							}

							parentResourceRetrievers.Add(parentResourceRetriever);
						} catch (MissingMethodException ex) {
							Logger.Log(LogLevelType.ErrorsOnly, ex,
								"ResourceRetriever, {0}, could not be instantiated (is it an abstract class?).",
								parentResourceRetrieverSetting.Name);
						} catch (TypeLoadException ex) {
							Logger.Log(LogLevelType.ErrorsOnly, ex,
								"ResourceRetriever, {0}, could not be instantiated.",
								parentResourceRetrieverSetting.Name);
						}
					}

					Save(instance);
				}

				return parentResourceRetrievers;
			}
		}

		/// <summary>
		/// Shared Imagelist.
		/// </summary>
		[XmlIgnore]
		public ImageList ImageList {
			get {
				return imageList;
			}
		}
	}
}