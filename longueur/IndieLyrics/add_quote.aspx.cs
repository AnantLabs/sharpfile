using System;
using System.Web;
using Common;
using Membership;
using Data;

public partial class add : System.Web.UI.Page {
	protected void Page_Load(object sender, EventArgs e) {
		if (!IsPostBack) {
			if (Request.UrlReferrer != null && !Request.UrlReferrer.AbsolutePath.EndsWith("login.aspx")) {
				Session["ReferralUrl"] = Request.UrlReferrer != null ? Request.UrlReferrer.PathAndQuery : "default.aspx";
			}

			Artist.Focus();
			Genre.DataSource = IndieLyrics.GetGenreList();
			Genre.DataTextField = "GenreName";
			Genre.DataValueField = "GenreID";
			Genre.DataBind();

			Page.ClientScript.RegisterClientScriptBlock(typeof(string), "fancyIdScripts", @"
function getArtistTextBoxId() {
	return '" + Artist.ClientID + @"';
}

function getAlbumTextBoxId() {
	return '" + Album.ClientID + @"';
}

function getSongTextBoxId() {
	return '" + Song.ClientID + @"';
}
", true);
		}

		if (Session["AddError"] != null) {
			//AddError.Text = Session["AddError"].ToString();
			//AddError.Visible = true;
			Session["AddError"] = null;
		} else {
			//AddError.Visible = false;
		}

		if (Session["ErrorArtist"] != null 
			&& Session["ErrorLyricist"] != null 
			&& Session["ErrorSong"] != null
			&& Session["ErrorAlbum"] != null
			&& Session["ErrorQuote"] != null) {

			Artist.Text = Session["ErrorArtist"].ToString();
			Lyricist.Text = Session["ErrorLyricist"].ToString();
			Song.Text = Session["ErrorSong"].ToString();
			Album.Text = Session["ErrorAlbum"].ToString();
			Quote.Text = Session["ErrorQuote"].ToString();

			Session["ErrorArtist"] = null;
			Session["ErrorLyricist"] = null;
			Session["ErrorSong"] = null;
			Session["ErrorAlbum"] = null;
			Session["ErrorQuote"] = null;
		}

		if (IsPostBack && Request.Form["MultiTaskType"] != null && Request.Form["MultiTaskType"] == "submit") {
			if (Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated) {
				if (Artist.Text != null && Artist.Text.Trim() == string.Empty) {
					Session["AddError"] += "Please enter an artist<br />";
				}

				if (Quote.Text != null && Quote.Text.Trim() == string.Empty) {
					//should also check that the quote only has 'allowed' html
					//also convert any newline to <br />'s
					//maybe there should be a preview button?
					Session["AddError"] += "Please enter a quote<br />";
				}

				if (Session["AddError"] == null) {
					int userId = ((SiteUser)Session[Constants.CurrentUser]).Id;
					int quoteId = IndieLyrics.QuoteInsert(Artist.Text, Lyricist.Text, Song.Text, Album.Text, Genre.SelectedItem.Text, Quote.Text, userId);

					if (quoteId > -1) {
						Response.Clear();
						Response.Redirect("quote.aspx?QID=" + quoteId, true);
					} else {
						//do what?
						//error message here
					}
				} else {
					Session["ErrorArtist"] = Artist.Text;
					Session["ErrorLyricist"] = Lyricist.Text;
					Session["ErrorSong"] = Song.Text;
					Session["ErrorAlbum"] = Album.Text;
					Session["ErrorQuote"] = Quote.Text;

					Response.Clear();
					Response.Redirect("add_quote.aspx", true);
				}
			} else {
				Session["AddError"] = "Users need to be logged in to add a lyric.";

				Response.Clear();
				Response.Redirect("add_quote.aspx", true);
			}
		}
	}
}
