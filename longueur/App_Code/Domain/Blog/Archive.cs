using System;

/// <summary>
/// Summary description for Archive
/// </summary>
public class Archive {
	private int id;
	private DateTime startDate;
	private DateTime endDate;
	private string name;

	public Archive(int id, DateTime startDate, DateTime endDate, string name) {
		this.id = id;
		this.startDate = startDate;
		this.endDate = endDate;
		this.name = name;
	}

	public int ID { get { return id; } set { id = value; } }
	public DateTime StartDate { get { return startDate; } set { startDate = value; } }
	public DateTime EndDate { get { return endDate; } set { endDate = value; } }
	public string Name { get { return name; } set { name = value; } }
}
