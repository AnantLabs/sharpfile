using System;
using System.Web;

/// <summary>
/// Summary description for SiteUser.
/// </summary>
public class SiteUser
{
	private int id;
	private string name;
	private string password;
	private string email;
	private string guid;
	private bool enableJs;
	private UserType userType;

	public SiteUser()
	{
		this.id = -1;
		this.name = "Anonymous";
		this.password = string.Empty;
		this.userType = UserType.NonAuthenticated;
		this.email = string.Empty;
		this.enableJs = false;
		this.guid = string.Empty;
	}

	public SiteUser(int id, string name, string password, string email, bool enableJs, UserType userType)
	{
		this.id = id;
		this.name = name;
		this.password = password;
		this.email = email;
		this.guid = System.Guid.NewGuid().ToString();
		this.enableJs = enableJs;
		this.userType = userType;
	}

	#region Static methods
	public static SiteUser GetCurrentUser()
	{
		object obj = HttpContext.Current.Session[Constants.CurrentUser];

		if (obj != null
			&& IsObjectAUser(obj))
		{
			return obj as SiteUser;
		}

		return Data.GetAnonymousUser();
	}

	public static bool IsObjectAUser(object obj)
	{
		if (obj != null)
		{
			SiteUser user;

			try
			{
				user = (SiteUser)obj;
			}
			catch
			{
				return false;
			}

			return true;
		}

		return false;
	}

	public static bool IsUserAuthorized(SiteUser user)
	{
		return IsUserAuthorized(user, user.Id);
	}

	public static bool IsUserAuthorized(SiteUser user, int id)
	{
		//there should be a check for cookies here? maybe the content is passed in?

		if (user != null
			&& user.UserType != UserType.NonAuthenticated
			&& (user.Id == id || user.UserType == UserType.Admin))
		{
			return true;
		}

		return false;
	}
	#endregion

	#region Public properties
	public int Id
	{
		get 
		{ 
			return id; 
		}
	}

	public string Name
	{
		get 
		{ 
			return name; 
		}
	}

	public string Password
	{
		get 
		{ 
			return password;
		}
	}

	public string Email
	{
		get 
		{ 
			return email; 
		}
	}

	public UserType UserType
	{
		get 
		{ 
			return userType;
		}
	}

	public string Guid
	{
		get 
		{
			return guid; 
		}
	}

	public bool EnableJs
	{
		get 
		{ 
			return enableJs; 
		}
		set 
		{ 
			enableJs = value; 
		}
	}
	#endregion
}