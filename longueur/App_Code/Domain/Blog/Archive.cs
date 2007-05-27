using System;

/// <summary>
/// Summary description for Archive
/// </summary>
public class Archive {
	private int id;
	private DateTime startDate;
	private DateTime endDate;

	public Archive(int id, DateTime startDate, DateTime endDate) {
		this.id = id;
		this.startDate = startDate;
		this.endDate = endDate;
	}

	public int ID { get { return id; } set { id = value; } }
	public DateTime StartDate { get { return startDate; } set { startDate = value; } }
	public DateTime EndDate { get { return endDate; } set { endDate = value; } }
}
