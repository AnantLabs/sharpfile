using System;
using Data.DAL;

/// <summary>
/// Summary description for Tag
/// </summary>
public class Tag {
	private int id;
	private string name;
	private string image;

	public Tag() {
	}

	public Tag(int id, string name, string image) {
		this.id = id;
		this.name = name;
		this.image = image;
	}

	[DBField("Id"), DBParameter("@Id")]
	public int Id {
		get { return id; }
		set { id = value; }
	}

	[DBField("Name"), DBParameter("@Name")]
	public string Name {
		get { return name; }
		set { name = value; }
	}

	[DBField("Image"), DBParameter("@Image")]
	public string Image {
		get { return image; }
		set { image = value; }
	}
}