using System;
using Data.DAL;

/// <summary>
/// Summary description for Tag
/// </summary>
public class Tag {
	private int id;
	private string name;
	private int count;

	public Tag() {
	}

	public Tag(int id, string name, int count) {
		this.id = id;
		this.name = name;
		this.count = count;
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
	
	public int Count {
		get { return count; }
		set { count = value; }
	}
}