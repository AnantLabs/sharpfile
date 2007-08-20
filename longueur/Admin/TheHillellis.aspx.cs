using System;
using System.Data;
using Domain.Membership;
using Domain.Blog;
using Data;
using System.Web.Security;
using System.Web;
using System.Web.UI;
using Data.Blog;
using System.Collections.Generic;
using System.Text;

public partial class Admin_TheHillellis : Page
{
	private const string _id = "id";
	private IBlog blogDAO = new TheHillellis();

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack) {
			int id = 0;

			if (string.IsNullOrEmpty(Request.QueryString[_id])) {
				rptSlogs.DataSource = EntriesFactory.GetEntries(ThemeType.Minimal, blogDAO);
				rptSlogs.DataBind();
			} else if (int.TryParse(Request.QueryString[_id], out id)) {
				divSlogInfo.Visible = true;
				rptSlogs.Visible = false;
				divNewEntry.Visible = false;

				Entry entry = EntriesFactory.GetEntries(ThemeType.Minimal, blogDAO, id)[0];

				if (entry != null) {
					lblId.Text = entry.Id.ToString();
					txtName.Text = entry.Name;
					txtTitle.Text = entry.Title;
					txtContent.Text = entry.Content;
					lblDateTime.Text = entry.DateTime.ToString();

					StringBuilder tagIdStringBuilder = new StringBuilder();
					entry.Tags.ForEach(delegate(Tag t) {
						tagIdStringBuilder.AppendFormat("{0} ",
							t.Name);
					});

					txtTags.Text = tagIdStringBuilder.ToString().Trim();
				} else {
					lblMessage.Text = "Oh no, looks like something went wacky-tacky.<br />That emtry doesn't seem to exist.";
				}
			}

			rptTags.DataSource = blogDAO.GetTags();
			rptTags.DataBind();
		}
	}

	protected override void OnPreRender(EventArgs e) {
		if (!string.IsNullOrEmpty(lblMessage.Text)) {
			lblMessage.Visible = true;
		}

		base.OnPreRender(e);
	}

	protected override void OnInit(EventArgs e)
	{
		InitializeComponent();
		base.OnInit(e);
	}

	private void InitializeComponent()
	{
		this.btnNewSubmit.Click += new EventHandler(btnNewSubmit_Click);
		this.btnSave.Click += new EventHandler(btnSave_Click);
		this.btnDelete.Click += new EventHandler(btnDelete_Click);
	}

	void btnDelete_Click(object sender, EventArgs e) {
		throw new Exception("The method or operation is not implemented.");
	}

	private bool authenticate() {
		bool authenticated = false;

		if (User.Identity.IsAuthenticated) {
			authenticated = true;
		} else {
			lblMessage.Text = "Looks like you aren't an admin user after all, jerk.";
			lblMessage.Visible = true;
		}

		return authenticated;
	}

	private void redirect() {
		Response.Redirect(Request.Url.PathAndQuery);
	}

	void btnSave_Click(object sender, EventArgs e) {
		if (authenticate()) {
			int id = 0;			

			if (int.TryParse(Request.QueryString[_id], out id)) {
				string name = txtName.Text;
				SiteUser siteUser = new SiteUser(name);

				if (siteUser != null) {
					try {
						string tagIds = getTagIds(txtTags.Text);
						blogDAO.UpdateEntry(id, txtTitle.Text, txtContent.Text, siteUser.Id, tagIds);
						//redirect();
					} catch (Exception ex) {
						lblMessage.Text = "There was an error: " + ex.Message + ex.StackTrace;
					}
				} else {
					lblMessage.Text = "Looks like the user you were try to add as the author doesn't exist. Weirdo.";
				}
			} else {
				lblMessage.Text = "The entry id is incorrect. Please fix it and try harder next time.";
			}
		}
	}

	void btnNewSubmit_Click(object sender, EventArgs e) {
		if (authenticate()) {
			int id = 0;
			Session["ArchiveCount"] = null;

			if (int.TryParse(((FormsIdentity)HttpContext.Current.User.Identity).Name, out id)) {
				// Grab the tag id from it's name in the textbox for tags.
				string tagIds = getTagIds(txtNewTags.Text);

				try {
					blogDAO.InsertEntry(txtNewTitle.Text, txtNewContent.Text, id, DateTime.Now, tagIds);
					redirect();
				} catch (Exception ex) {
					lblMessage.Text = ex.Message + ex.StackTrace;
				}
			} else {
				lblMessage.Text = "Looks like you aren't an admin user after all, jerk.";
			}
		}

		if (!string.IsNullOrEmpty(lblMessage.Text)) {
			lblMessage.Visible = true;
		}
	}

	private string getTagIds(string tagText) {
		List<Tag> tags = blogDAO.GetTags();
		List<string> tagNames = new List<string>(tagText.Trim().Split(' '));
		StringBuilder tagIdStringBuilder = new StringBuilder();

		foreach (string tagName in tagNames) {
			if (tagName != string.Empty) {
				Tag tag = tags.Find(delegate(Tag t) {
					return t.Name == tagName;
				});

				if (tag != null) {
					tagIdStringBuilder.AppendFormat("{0},",
						tag.Id);
				} else {
					// TODO: Insert the tag here and then append the newly inserted tag's id to the sb.
				}
			}
		}

		if (tagIdStringBuilder.Length > 0) {
			tagIdStringBuilder.Remove(tagIdStringBuilder.Length - 1, 1);
		}

		return tagIdStringBuilder.ToString();
	}
}
