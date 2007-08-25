using System.Configuration;

public class RuleElementCollection : ConfigurationElementCollection {
	#region Constructors
	static RuleElementCollection() {
		properties = new ConfigurationPropertyCollection();
	}
	#endregion

	#region Fields
	private static ConfigurationPropertyCollection properties;
	#endregion

	#region Properties
	protected override ConfigurationPropertyCollection Properties {
		get { return properties; }
	}

	public override ConfigurationElementCollectionType CollectionType {
		get { return ConfigurationElementCollectionType.BasicMap; }
	}

	protected override string ElementName {
		get { return "rule"; }
	}
	#endregion

	#region Indexers
	public RuleElement this[int index] {
		get { return (RuleElement)base.BaseGet(index); }
		set {
			if (base.BaseGet(index) != null) {
				base.BaseRemoveAt(index);
			}

			base.BaseAdd(index, value);
		}
	}

	public new RuleElement this[string url] {
		get { return (RuleElement)base.BaseGet(url); }
	}
	#endregion

	#region Methods
	public void Add(RuleElement rule) {
		base.BaseAdd(rule);
	}

	public void Remove(string url) {
		base.BaseRemove(url);
	}

	public void Remove(RuleElement rule) {
		base.BaseRemove(GetElementKey(rule));
	}

	public void Clear() {
		base.BaseClear();
	}

	public void RemoveAt(int index) {
		base.BaseRemoveAt(index);
	}

	public string GetKey(int index) {
		return (string)base.BaseGetKey(index);
	}
	#endregion

	#region Overrides
	protected override ConfigurationElement CreateNewElement() {
		return new RuleElement();
	}

	protected override object GetElementKey(ConfigurationElement element) {
		return (element as RuleElement).Url;
	}
	#endregion
}