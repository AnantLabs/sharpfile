using System;

/// <summary>
/// Represents all of the information necessary for a phot from flickr.
/// </summary>
public class Photo
{
	private string title;
	private string mediumUrl;
	private string thumbnailUrl;
	private string description;
	private string id;
	private string largeUrl;

	public Photo(string id, string title, string description, string thumbnailUrl,
		string mediumUrl, string largeUrl)
	{
		this.id = id;
		this.title = title;
		this.description = description;
		this.thumbnailUrl = thumbnailUrl;
		this.mediumUrl = mediumUrl;
		this.largeUrl = largeUrl;
	}

	public string Id {
		get { return id; }
	}

	public string Title {
		get { return title; }
	}

	public string Description {
		get { return description; }
	}

	public string ThumbnailUrl {
		get { return thumbnailUrl; }
	}

	public string MediumUrl {
		get { return mediumUrl; }
	}

	public string LargeUrl {
		get { return largeUrl; }
	}
}
