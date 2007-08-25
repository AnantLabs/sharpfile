using System.Web.Configuration;
using Common;

// SettingsSingleton is a Singleton.
public class SettingsSingleton {
	private RuleElementCollection rewrites;

	private SettingsSingleton() {
		loadSettings();
	}

	// Gets the single instance of SingleInstanceClass.
	public static SettingsSingleton Instance {
		get { return Singleton<SettingsSingleton>.Instance; }
	}

	private void loadSettings() {
		RewriteSettings settings = WebConfigurationManager.GetSection("rewriteSettings")
							as RewriteSettings;

		rewrites = settings.Rewrites;
	}

	public RuleElementCollection Rewrites {
		get {
			return rewrites;
		}
	}
}
