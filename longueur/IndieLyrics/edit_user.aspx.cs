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
using Common;

public partial class edit_user : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		if (!IsPostBack) {
			if (Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated) {
				SiteUser currentUser = (SiteUser)Session[Constants.CurrentUser];

				UserName.Text = currentUser.Name;
				Email.Text = currentUser.Email;
			} else {
				Response.Clear();
				Response.Redirect("login.aspx");
			}
		} else {
			if (Request.Form["MultiTaskType"] != null && Request.Form["MultiTaskType"] == "save") {
				if (Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated) {
					SiteUser currentUser = (SiteUser)Session[Constants.CurrentUser];

					if ((!string.IsNullOrEmpty(NewPassword.Text) && ConfirmNewPassword.Text == string.Empty) || (NewPassword.Text == string.Empty && !string.IsNullOrEmpty(ConfirmNewPassword.Text))) {
						Message.Text = "Both of the new password boxes must be filled in.";
					} else if (!string.IsNullOrEmpty(NewPassword.Text) && !string.IsNullOrEmpty(ConfirmNewPassword.Text)) {
						if (NewPassword.Text.Equals(ConfirmNewPassword.Text)) {
							if (currentUser.HashedPassword.Equals(Security.Encrypt(CurrentPassword.Text))) {
								try {
									SiteUser newUser = IndieLyricsData.UpdateUser(currentUser.Id, UserName.Text, Email.Text, CurrentPassword.Text, NewPassword.Text);
									Session[Constants.CurrentUser] = newUser;
									Message.Text = "The new information has been saved.";
								} catch (Exception ex) {
									string error = ex.Message + ex.StackTrace;
									Message.Text = "The user could not be edited<!--" + error + "-->";
								}
							} else {
								Message.Text = "The password entered is incorrect. Please correct the password to save your information.";
							}
						} else {
							Message.Text = "The two new passwords must be the same. Please try again.";
						}
					} else if (NewPassword.Text == string.Empty && ConfirmNewPassword.Text == string.Empty) {
						if (currentUser.HashedPassword.Equals(Security.Encrypt(CurrentPassword.Text))) {
							try {
								SiteUser newUser = IndieLyricsData.UpdateUser(currentUser.Id, UserName.Text, Email.Text, CurrentPassword.Text);
								Session[Constants.CurrentUser] = newUser;
								Message.Text = "The new information has been saved.";
							} catch (Exception ex) {
								string error = ex.Message + ex.StackTrace;
								Message.Text = "The user could not be edited<!--" + error + "-->";
							}
						} else {
							Message.Text = "The password entered is incorrect. Please correct the password to save your information.";
						}
					}

					NewPassword.Text = string.Empty;
					ConfirmNewPassword.Text = string.Empty;
				}
			} else if (Request.Form["MultiTaskType"] != null && Request.Form["MultiTaskType"] == "delete") {
				if (Session[Constants.CurrentUser] != null && ((SiteUser)Session[Constants.CurrentUser]).UserType != UserType.NonAuthenticated) {
					IndieLyricsData.DeleteUser(((SiteUser)Session[Constants.CurrentUser]).Id);
					Session[Constants.CurrentUser] = IndieLyricsData.GetAnonymousUser();

					Response.Clear();
					Response.Redirect("default.aspx", true);
				}
			} else if (Request.Form["MultiTaskType"] != null && Request.Form["MultiTaskType"] == "cancel") {
				Response.Clear();
				Response.Redirect("default.aspx", true);
			}
		}
    }
}
