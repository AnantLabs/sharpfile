using System;
using Data.DAL;

/// <summary>
/// Summary description for Link
/// </summary>
public class Link {
	private int id;
	private string href;
	private string description;
	private string title;

	public Link() {
	}

	public Link(int id, string href, string title, string description) {
		this.id = id;
		this.href = href;
		this.title = title;
		this.description = description;
	}

	[DBField("Id")]
	public int Id {
		get { return id; }
		set { id = value; }
	}

	[DBField("Href"), DBParameter("@Href")]
	public string Href {
		get { return href; }
		set { href = value; }
	}

	[DBField("Title"), DBParameter("@Title")]
	public string Title {
		get { return title; }
		set { title = value; }
	}

	[DBField("Description"), DBParameter("@Description")]
	public string Description {
		get { return description; }
		set { description = value; }
	}
}
