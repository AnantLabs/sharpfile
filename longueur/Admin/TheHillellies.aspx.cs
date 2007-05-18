using System;
using System.Data;
using Membership;
using Data;
using System.Web.Security;
using System.Web;

public partial class Admin_TheHillellies : System.Web.UI.Page
{
	private const string _id = "id";

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack) {
			int id = 0;

			if (string.IsNullOrEmpty(Request.QueryString[_id])) {
				rptSlogs.DataSource = Slog.GetSlogs();
				rptSlogs.DataBind();
			} else if (int.TryParse(Request.QueryString[_id], out id)) {
				divSlogInfo.Visible = true;
				rptSlogs.Visible = false;
				divNewEntry.Visible = false;

				DataTable slogTable = Slog.GetSlog(id);

				if (slogTable.Rows.Count > 0) {
					lblId.Text = slogTable.Rows[0]["Id"].ToString();
					txtName.Text = slogTable.Rows[0]["Name"].ToString();
					txtTitle.Text = slogTable.Rows[0]["Title"].ToString();
					txtContent.Text = slogTable.Rows[0]["Content"].ToString();
					lblDateTime.Text = slogTable.Rows[0]["DateTime"].ToString();
				} else {
					lblMessage.Text = "Oh no, looks like something went wacky-tacky.<br />That emtry doesn't seem to exist.";
				}
			}
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
						//SlogData.UpdateSlog(lblId.Text);
					} catch (Exception ex) {
						lblMessage.Text = "There was an error: " + ex.Message + ex.StackTrace;
					}

					redirect();
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

			if (int.TryParse(((FormsIdentity)HttpContext.Current.User.Identity).Name, out id)) {
				Slog.InsertSlog(txtNewTitle.Text, txtNewContent.Text, id);
				redirect();
			} else {
				lblMessage.Text = "Looks like you aren't an admin user after all, jerk.";
			}
		}

		if (!string.IsNullOrEmpty(lblMessage.Text)) {
			lblMessage.Visible = true;
		}
	}
}
