using System.Configuration;
using System.Text.RegularExpressions;
using Common;

public class RuleElement : ConfigurationElement {
	#region Constructors
	/// <summary>
	/// Predefines the valid properties and prepares
	/// the property collection.
	/// </summary>
	static RuleElement() {
		url = new ConfigurationProperty(
			"url",
			typeof(Regex),
			null,
			regexTypeConverter,
			null,
			ConfigurationPropertyOptions.IsRequired
			);

		rewrite = new ConfigurationProperty(
			"rewrite",
			typeof(string),
			null,
			ConfigurationPropertyOptions.IsRequired
			);

		properties = new ConfigurationPropertyCollection();
		properties.Add(url);
		properties.Add(rewrite);
	}
	#endregion

	#region Static Fields
	private static ConfigurationPropertyCollection properties;
	private static ConfigurationProperty url;
	private static ConfigurationProperty rewrite;

	private static RegexTypeConverter regexTypeConverter = new RegexTypeConverter();
	#endregion

	#region Properties
	/// <summary>
	/// Gets the Url setting.
	/// </summary>
	public Regex Url {
		get { return (Regex)base[url]; }
	}

	/// <summary>
	/// Gets the Rewrite setting.
	/// </summary>
	public string Rewrite {
		get { return (string)base[rewrite]; }
	}

	/// <summary>
	/// Override the Properties collection and return our custom one.
	/// </summary>
	protected override ConfigurationPropertyCollection Properties {
		get { return properties; }
	}
	#endregion
}