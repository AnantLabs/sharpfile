using System.Configuration;

	/// <summary>
	/// An example configuration section class.
	/// Example code from: http://www.codeproject.com/dotnet/mysteriesofconfiguration.asp.
	/// </summary>
public class RewriteSettings : ConfigurationSection {
	#region Constructors
	/// <summary>
	/// Predefines the valid properties and prepares
	/// the property collection.
	/// </summary>
	static RewriteSettings() {
		rewrites = new ConfigurationProperty(
			"rewrites",
			typeof(RuleElementCollection),
			null,
			ConfigurationPropertyOptions.IsDefaultCollection
			);

		properties = new ConfigurationPropertyCollection();
		properties.Add(rewrites);
	}
	#endregion

	#region Static Fields
	private static ConfigurationPropertyCollection properties;
	private static ConfigurationProperty rewrites;
	#endregion

	#region Properties
	/// <summary>
	/// Gets the Connection element.
	/// </summary>
	public RuleElementCollection Rewrites {
		get { return (RuleElementCollection)base[rewrites]; }
		set { base[rewrites] = value; }
	}

	/// <summary>
	/// Override the Properties collection and return our custom one.
	/// </summary>
	protected override ConfigurationPropertyCollection Properties {
		get { return properties; }
	}
	#endregion
}