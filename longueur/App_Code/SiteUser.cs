using System;
using System.Web;
using System.Web.Security;

/// <summary>
/// Summary description for SiteUser.
/// </summary>
public class SiteUser
{
	private const int formsAuthenticationVersion = 1;
	private const int cookieExpirationTime = 30;

	private int id;
	private string name;
	private string hashedPassword;
	private string email;
	private string guid;
	private bool enableJs;
	private UserType userType;

	public SiteUser()
	{
		this.id = -1;
		this.name = "Anonymous";
		this.hashedPassword = string.Empty;
		this.userType = UserType.NonAuthenticated;
		this.email = string.Empty;
		this.enableJs = false;
		this.guid = string.Empty;
	}

	public SiteUser(int id, string name, string hashedPassword, string email, bool enableJs, UserType userType)
	{
		this.id = id;
		this.name = name;
		this.hashedPassword = hashedPassword;
		this.email = email;
		this.guid = System.Guid.NewGuid().ToString();
		this.enableJs = enableJs;
		this.userType = userType;
	}

	public bool Login()
	{
		return Login(true);
	}

	public bool Login(bool persistent)
	{
		if (userType != UserType.NonAuthenticated)
		{
			string roles = userType.ToString();

			// Make sure our admin users are also users.
			if (userType == UserType.Admin)
			{
				roles = string.Format("{0},{1}", 
					roles,
					UserType.User.ToString());
			}

			// Initialize FormsAuthentication, for what it's worth
			FormsAuthentication.Initialize();

			// Create a new ticket used for authentication
			FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
			   formsAuthenticationVersion,
			   id.ToString(),
			   DateTime.Now,
			   DateTime.Now.AddMinutes(cookieExpirationTime),
			   persistent,
			   roles,
			   FormsAuthentication.FormsCookiePath);

			// Encrypt the cookie using the machine key for secure transport
			string hashedTicket = FormsAuthentication.Encrypt(ticket);

			HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashedTicket);

			// Set the cookie's expiration time to the tickets expiration time
			if (ticket.IsPersistent)cookie.Expires = ticket.Expiration;

			// Add the cookie to the list for outgoing response
			if (HttpContext.Current != null)
			{
				HttpContext.Current.Response.Cookies.Add(cookie);
			}

			return true;
		}

		return false;
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

	public string HashedPassword
	{
		get 
		{
			return hashedPassword;
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