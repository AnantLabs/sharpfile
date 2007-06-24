using System;
using Data.DAL;

/// <summary>
/// Summary description for Link
/// </summary>
public class Link {
	private int id;
	private string href;
	private string description;

	public Link() {
	}

	public Link(int id, string href, string description) {
		this.id = id;
		this.href = href;
		this.description = description;
	}

	[DBField("Id"), DBParameter("@Id")]
	public int Id {
		get { return id; }
		set { id = value; }
	}

	[DBField("Href"), DBParameter("@Href")]
	public string Href {
		get { return href; }
		set { href = value; }
	}

	[DBField("Description"), DBParameter("@Description")]
	public string Description {
		get { return description; }
		set { description = value; }
	}
}
