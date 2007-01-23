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
using System.Text.RegularExpressions;
using Common;

public partial class quote : System.Web.UI.Page
{
    private const string _saveBtn = "saveBtn";
    private const string _rateQuote = "rateQuote";
    private const string _Rating = "Rating";
    private const string _CommentError = "CommentError";
    private const string _beginningButNoClosing = "Your comment has a beginning tag, but not a closing tag.";
    private const string _closingButNoBeginning = "Your comment has a closing tag, but not a beginning tag.";
    private const string _editedCommentText = "editedCommentText";

    protected void Page_Load(object sender, EventArgs e)
    {
		SiteUser siteUser = SiteUser.GetCurrentUser();

        if (Request[Constants.QID] != null && General.IsInt(Request[Constants.QID]))
        {
            Session[Constants.QuoteID] = int.Parse(Request[Constants.QID]);
            Response.Clear();
            Response.Redirect(Request.Path, true);
        }
        else if (Request[Constants.QID] == null && Session[Constants.QuoteID] == null)
        {
            AddComment.Controls.Clear();
            AddComment.Controls.Add(new LiteralControl("<br>no quote id = no details"));
        }
        else if (!IsPostBack && Session[Constants.QuoteID] != null)
        {
			CommentUser.Text = siteUser.Name;
			GetDetails();
            GetComments();

            submitBtn.Attributes[Constants.onclick] = "SubmitCommentTextArea();";
            ErrorMessage.Visible = false;
        }
        else if (IsPostBack && Session[Constants.QuoteID] != null)
        {
            submitBtn.Attributes[Constants.onclick] = "SubmitCommentTextArea();";
            ErrorMessage.Visible = false;
        }
    }

    private void GetComments()
    {
        if (Session[Constants.QuoteID] != null)
        {
            DataTable comments = Data.GetQuoteComments((int)Session[Constants.QuoteID]);

            if (comments.Rows.Count > 0)
            {
                quoteComments.DataSource = comments;
                quoteComments.DataBind();

                foreach (RepeaterItem item in quoteComments.Items)
                {
                    if (item.FindControl(Constants.commentId) != null && item.FindControl(_saveBtn) != null)
                    {
                        ((ImageButton)item.FindControl(_saveBtn)).Attributes[Constants.onclick] = "RefreshCommentTextArea(" + ((HtmlInputHidden)item.FindControl(Constants.commentId)).Value + ");";
                        ((ImageButton)item.FindControl("deleteBtn")).Attributes[Constants.onclick] = "if (confirm('Are you sure you want to delete this comment?')) { DeleteCommentTextArea(" + ((HtmlInputHidden)item.FindControl(Constants.commentId)).Value + "); return true; } return false;";
                    }
                }
            }
            else
            {
                quoteComments.Controls.Clear();
            }
        }
    }

    private void GetDetails()
    {
		SiteUser siteUser = SiteUser.GetCurrentUser();
		bool isAuthorized = !(siteUser.UserType == UserType.NonAuthenticated);
        int userId = siteUser.Id;
        int quoteId = -1;

        if (!General.IsInt(Session[Constants.QuoteID]))
        {
            return;
        }
        else
        {
            quoteId = Convert.ToInt32(Session[Constants.QuoteID]);
        }

        DataTable details = Data.GetQuoteDetails(userId, quoteId);

        if (details.Rows.Count == 1)
        {
            quoteDetails.DataSource = details;
            quoteDetails.DataBind();

            RepeaterItem item = quoteDetails.Items[0];
            DataRow row = details.Rows[0];

			if (isAuthorized)
			{
				if (item.FindControl(_rateQuote) != null)
				{
					((DropDownList)item.FindControl(_rateQuote)).SelectedValue = row[_Rating] == Convert.DBNull ? string.Empty : row[_Rating].ToString();
				}
			}
        }
    }

	private void InsertComment()
	{
		SiteUser siteUser = SiteUser.GetCurrentUser();
		string commentText = CommentText.Text;

		if (parseComment(commentText))
		{
			int userId = siteUser.Id;
			Data.CommentInsert((int)Session[Constants.QuoteID], userId, commentText);
			GetComments();

			CommentText.Text = string.Empty;
			ErrorMessage.Text = string.Empty;
			ErrorMessage.Visible = false;
			Session[_CommentError] = string.Empty;
		}
		else
		{
			//show the error
			ErrorMessage.Text = Session[_CommentError].ToString();
			ErrorMessage.Visible = true;
		}
	}

	private void UpdateComment(int commentId, string commentText)
	{
		SiteUser siteUser = SiteUser.GetCurrentUser();

		if (parseComment(commentText))
		{
			int userId = siteUser.Id;
			Data.CommentUpdate(commentId, userId, commentText);
			GetComments();
		}
		else
		{
			//show the error
		}
	}

    private bool parseComment(string commentText)
    {
        Session[_CommentError] = string.Empty;
        Regex tagRegex = new Regex(@"<.*?>", RegexOptions.IgnoreCase);
        Regex allowedTagRegex = new Regex(@"<\s*br\s*>|<\s*br\s*/\s*>|<\s*em\s*>|<\s*/\s*em\s*>|<\s*strong\s*>|<\s*/\s*strong\s*>", RegexOptions.IgnoreCase);

        bool beginEm = false;
        bool endEm = false;
        bool beginStrong = false;
        bool endStrong = false;

        foreach (Match m in tagRegex.Matches(commentText))
        {
            string tagMatch = m.Groups[0].Value.ToLower();

            if (!allowedTagRegex.IsMatch(tagMatch))
            {
                //this won't work because the index is for the original text and will overrun after the removal of the first tag
                //commentText = commentText.Remove(m.Index, m.Length);

                Session[_CommentError] = "There was a HTML tag in your comment that doesn't belong.";
            }
            /*
            if ((!m.Groups[0].Value.ToLower().Equals("<br />") || !m.Groups[0].Value.ToLower().Equals("<br>") || !m.Groups[0].Value.ToLower().Equals("<br/>")) && !m.Groups[0].Value.ToLower().Equals("<em>") && !m.Groups[0].Value.ToLower().Equals("</em>") && !m.Groups[0].Value.ToLower().Equals("<strong>") && !m.Groups[0].Value.ToLower().Equals("</strong>"))
            {
                commentText = commentText.Remove(m.Index, m.Length);
            }
            */
            // I wish there was a better way to do this. With regex, perhaps?
            else if (tagMatch.Equals("<em>"))
            {
                beginEm = true;
            }
            else if (tagMatch.Equals("</em>"))
            {
                endEm = true;
            }
            else if (tagMatch.Equals("<strong>"))
            {
                beginStrong = true;
            }
            else if (tagMatch.Equals("</strong>"))
            {
                endStrong = true;
            }
        }

        if (beginEm && !endEm)
        {
            Session[_CommentError] = _beginningButNoClosing;
        }
        else if (!beginEm && endEm)
        {
            Session[_CommentError] = _closingButNoBeginning;
        }
        else if (beginStrong && !endStrong)
        {
            Session[_CommentError] = _beginningButNoClosing;
        }
        else if (!beginStrong && endStrong)
        {
            Session[_CommentError] = _closingButNoBeginning;
        }

        if (Session[_CommentError].ToString() == string.Empty)
        {
            return true;
        }
         
        return false;
    }

    protected void submitBtn_Event(object sender, EventArgs e)
    {
        InsertComment();
        GetComments();
    }

    protected void rateQuote_Event(object sender, EventArgs e)
    {
		SiteUser siteUser = SiteUser.GetCurrentUser();

        if (Session[Constants.QuoteID] == null
            || !General.IsInt(Session[Constants.QuoteID]))
        {
            return;
        }

        int quoteId = Convert.ToInt32(Session[Constants.QuoteID]);
        int userId = -1;
        int rating = -1;

        if (!SiteUser.IsUserAuthorized(siteUser))
        {
            return;
        }
        else 
        {
            userId = siteUser.Id;
        }

        if (quoteDetails.Items[0].FindControl(_rateQuote) == null)
        {
            return;
        }
        else
        {
            string selectedRatingValue = ((DropDownList)quoteDetails.Items[0].FindControl(_rateQuote)).SelectedValue;

            if (selectedRatingValue == string.Empty
                && userId > -1)
            {
                Data.RatingDelete(quoteId, userId);
                GetDetails();
            }
            else if (General.IsInt(selectedRatingValue))
            {
                rating = int.Parse(selectedRatingValue);

                if (userId > -1 && rating > -1)
                {
                    Data.RatingUpsert(quoteId, userId, rating);
                    GetDetails();
                }
            }
        }
    }

    private bool repeaterValidation(object commandArgument)
    {
		SiteUser siteUser = SiteUser.GetCurrentUser();

        if (SiteUser.IsUserAuthorized(siteUser))
        {
            if (General.IsInt(commandArgument))
            {
                return true;
            }
        }

        return false;
    }

    protected void repeater_Event(object sender, RepeaterCommandEventArgs e)
    {
        if (!repeaterValidation(e.CommandArgument))
        {
            return;
        }

        int commentId = Convert.ToInt32(e.CommandArgument);
		SiteUser siteUser = SiteUser.GetCurrentUser();

        if (e.CommandName.Equals("delete"))
        {
            Data.CommentDelete(commentId, siteUser.Id);
            GetComments();
        }
        else if (e.CommandName.Equals("save"))
        {
            foreach (RepeaterItem item in quoteComments.Items)
            {
                if (((HtmlInputHidden)item.FindControl(Constants.commentId)).Value != null
                    && ((HtmlInputHidden)item.FindControl(Constants.commentId)).Value.Equals(commentId.ToString())
                    && ((HtmlTextArea)item.FindControl(_editedCommentText)) != null)
                {
                    string commentText = ((HtmlTextArea)item.FindControl(_editedCommentText)).Value;
					Data.CommentUpdate(commentId, siteUser.Id, commentText);
                    GetComments();
                    break;
                }
            }
        }
    }
}
