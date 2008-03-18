using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
// From: http://aspzone.com/blogs/john/articles/167.aspx
// TODO: Look at an_phu comment at the bottom.

	/*
	[XmlRoot("Node")]
	public class Node {
		[XmlAttribute("Name")]
		public string Name;

		[XmlAttribute("Data")]
		public string Data;

		public Node() {
			Name = null;
			Data = null;
		}

		public Node(string name, string data) {
			this.Name = name;
			this.Data = data;
		}
	}
	*/

	public class Nodes : DictionaryBase, IXmlSerializable {
		public Nodes() {
		}

		public virtual object this[string key] {
			get {
				return this.Dictionary[key];
			}
			set {
				this.Dictionary[key] = value;
			}
		}

		public virtual void Add(string key, object value) {
			this.Dictionary.Add(key, value);
		}

		public virtual bool Contains(string key) {
			return this.Dictionary.Contains(key);
		}

		public virtual bool ContainsKey(string key) {
			return this.Dictionary.Contains(key);
		}

		public virtual bool ContainsValue(object value) {
			foreach (object item in this.Dictionary.Values) {
				if (item == value)
					return true;
			}
			return false;
		}

		public virtual void Remove(string key) {
			this.Dictionary.Remove(key);
		}

		public virtual ICollection Keys {
			get {
				return this.Dictionary.Keys;
			}
		}

		public virtual ICollection Values {
			get {
				return this.Dictionary.Values;
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter w) {
			XmlSerializer keySer = new XmlSerializer(typeof(string));
			XmlSerializer valueSer = new XmlSerializer(typeof(object));

			foreach (object key in Dictionary.Keys) {
				w.WriteStartElement("item");

				w.WriteStartElement("key");
				keySer.Serialize(w, key);
				w.WriteEndElement();

				w.WriteStartElement("value");
				object value = Dictionary[key];
				valueSer.Serialize(w, value);
				w.WriteEndElement();

				w.WriteEndElement();
			}
		}

		void IXmlSerializable.ReadXml(XmlReader r) {
			XmlSerializer keySer = new XmlSerializer(typeof (string));
			XmlSerializer valueSer = new XmlSerializer(typeof(object));

			r.Read();

			while (r.NodeType != XmlNodeType.EndElement) {
				r.ReadStartElement("item");

				r.ReadStartElement("key");
				object key = keySer.Deserialize(r);
				r.ReadEndElement();

				r.ReadStartElement("value");
				object value = valueSer.Deserialize(r);
				r.ReadEndElement();

				Dictionary.Add(key, value);

				r.ReadEndElement();
				r.MoveToContent();
			}
		}

		XmlSchema IXmlSerializable.GetSchema() {
			return null;
		}
	}
}