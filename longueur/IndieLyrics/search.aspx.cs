using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

public partial class search : System.Web.UI.Page
{
	private const string searchQueryString = "id";
	private const string artistNameColumn = "ArtistName";
	private const string albumNameColumn = "AlbumName";
	private const string songNameColumn = "SongName";
	private const string notApplicable = "n/a";
	private const string noResult = "Sorry, there were no results. Please try another letter. Or <a href=\"add_quote.aspx\">add</a> a quote of your own, lazypants.";
	private const string firstResult = "Please choose what you would like to look for and then click on a letter for the beginning letter.";
	private const string searchStringHiddenForm = "SearchStringHiddenForm";

	private List<string> alphaNumerals = new List<string>(new string[] { "#", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" });

	private string lastArtistName = string.Empty;
	private string lastAlbumName = string.Empty;
	private string lastSongName = string.Empty;

	private int albumCount = 0;
	private int songCount = 0;
	private int artistCount = 0;

	private string searchString = string.Empty;
	private SearchType searchType;

    protected void Page_Load(object sender, EventArgs e)
    {
		if (!IsPostBack)
		{
			populateAlphaNumerals();

			ddlSearchType.DataSource = Enum.GetNames(typeof(SearchType));
			ddlSearchType.DataBind();

			clearResults(firstResult);
		}
    }

	private void clearResults(string message)
	{
		rptList.Controls.Clear();
		rptList.Controls.Add(new LiteralControl(message));
	}

	private void populateAlphaNumerals()
	{
		rptAlphaNumerals.DataSource = alphaNumerals;
		rptAlphaNumerals.DataBind();
	}

	protected void PopulateResults(object sender, EventArgs e)
	{
		if (sender.GetType() == typeof(LinkButton))
		{
			this.searchString = ((LinkButton)sender).CommandArgument;
		}
		else
		{
			if (Request.Form[searchStringHiddenForm] != null)
			{
				string tmpSearchString = Request.Form[searchStringHiddenForm];

				if (alphaNumerals.Contains(tmpSearchString))
				{
					this.searchString = tmpSearchString;
				}
			}
		}

		searchType = (SearchType)Enum.Parse(typeof(SearchType), ddlSearchType.SelectedValue);

		if (!string.IsNullOrEmpty(searchString))
		{
			int maxResults = 10;

			if (Common.General.IsInt(txtMaxResults.Text))
			{
				maxResults = int.Parse(txtMaxResults.Text);
			}

			rptList.DataSource = Data.SearchData(searchString, searchType, maxResults);
			rptList.DataBind();

			populateAlphaNumerals();
		}

		if (string.IsNullOrEmpty(searchString)
			|| rptList.Items.Count == 0)
		{
			clearResults(noResult);
		}
	}

	protected bool IsLastAlphaNumeralItem(RepeaterItem item)
	{
        if (item.ItemIndex == alphaNumerals.Count-1)
        {
            return true;
        }

        return false;
    }

	protected string GetDetails(object container)
	{
		string html = string.Empty;

		try
		{
			RepeaterItem item = (RepeaterItem)container;
			DataRowView rowView = (DataRowView)item.DataItem;

			string artistName = notApplicable;
			string albumName = notApplicable;
			string songName = notApplicable;

			if (!Common.General.IsNullOrEmpty(rowView[artistNameColumn]))
			{
				artistName = rowView[artistNameColumn].ToString();
			}

			if (artistName != lastArtistName)
			{
				artistCount++;
				lastArtistName = artistName;

				html = string.Format(@"
{0}
<strong>{1}</strong>
<br />
",
					artistCount > 1 ? "<br />" : "",
					artistName);
			}

			if (!Common.General.IsNullOrEmpty(rowView[albumNameColumn]))
			{
				albumName = rowView[albumNameColumn].ToString();
			}

			if (albumName != lastAlbumName)
			{
				albumCount++;
				lastAlbumName = albumName;

				html = string.Format(@"{0}
{1}
&nbsp;&nbsp;{2}
<br />
",
					html,
					albumCount > 1 ? "<br />" : "",
					albumName);
			}

			if (!Common.General.IsNullOrEmpty(rowView[songNameColumn]))
			{
				songName = rowView[songNameColumn].ToString();
			}

			if (songName != lastSongName)
			{
				lastSongName = songName;

				html = string.Format(@"{0}
&nbsp;&nbsp;&nbsp;&nbsp;{1}<br />
",
					html,
					songName);
			}
		}
		catch
		{
		}

		return html;
	}

	protected string SearchString
	{
		get
		{
			return searchString;
		}
	}
}
