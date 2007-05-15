using System;
using System.Web;
using System.Web.Security;
using System.Data;

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
	private DateTime dateTime;

	private bool isPopulated = false;

	public SiteUser()
	{
		this.id = -1;
		this.name = "Anonymous";
		this.hashedPassword = string.Empty;
		this.userType = UserType.NonAuthenticated;
		this.email = string.Empty;
		this.enableJs = false;
		this.guid = string.Empty;
		this.dateTime = DateTime.MinValue;
	}

	// TODO: This should be whacked.
	// TODO: This needs to be whacked for the DateTime to be populated correctly.
	//public SiteUser(int id, string name, string hashedPassword, string email, bool enableJs, UserType userType) {
	//    this.id = id;
	//    this.name = name;
	//    this.hashedPassword = hashedPassword;
	//    this.email = email;
	//    this.guid = System.Guid.NewGuid().ToString();
	//    this.enableJs = enableJs;
	//    this.userType = userType;
	//}

	// TODO: This should be whacked.
	//public SiteUser(int id, string name, string hashedPassword, string email, bool enableJs, UserType userType, DateTime dateTime) {
	//    this.id = id;
	//    this.name = name;
	//    this.hashedPassword = hashedPassword;
	//    this.email = email;
	//    this.guid = System.Guid.NewGuid().ToString();
	//    this.enableJs = enableJs;
	//    this.userType = userType;
	//    this.dateTime = dateTime;
	//}

	public SiteUser(int id) {
		populateUser(id);
	}

	public SiteUser(string name) {
		populateUser(name);
	}

	private void populateUser(int id) {
		DataTable userData = AdminData.GetUserData(id);
		populateUserFromDataTable(userData);		
	}

	private void populateUser(string name) {
		DataTable userData = AdminData.GetUserData(name);
		populateUserFromDataTable(userData);
	}

	private void populateUserFromDataTable(DataTable userTable) {
		if (userTable.Rows.Count > 0) {
			this.id = int.Parse(userTable.Rows[0]["Id"].ToString());
			this.name = userTable.Rows[0]["Name"].ToString();
			this.hashedPassword = userTable.Rows[0]["Password"].ToString();
			this.email = userTable.Rows[0]["Email"].ToString();
			this.enableJs = false;
			this.userType = (UserType)Enum.Parse(typeof(UserType), userTable.Rows[0]["TypeName"].ToString());

			this.isPopulated = true;
		} else {
			throw new Exception("No user can be found.");
		}
	}

	//public SiteUser(DataTable userTable) {
	//    if (userTable.Rows.Count > 0) {
	//        this.id = int.Parse(result.Rows[0]["Id"].ToString());
	//        this.name = result.Rows[0]["Name"].ToString();
	//        this.hashedPassword = result.Rows[0]["Password"].ToString();
	//        this.email = result.Rows[0]["Email"].ToString();
	//        this.enableJs = false;
	//        this.userType = (UserType)Enum.Parse(typeof(UserType), result.Rows[0]["TypeName"].ToString());
	//    }

	//    return new SiteUser();
	//}

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

	public bool Save()
	{
		// TODO: This should be a nice controller.
		throw new Exception("This method not completed yet.");
	}

	#region Static methods
	public static SiteUser GetAnonymousUser() {
		return Data.GetAnonymousUser();
	}

	public static SiteUser GetCurrentUser()
	{
		if (HttpContext.Current != null) {
			int id = 0;

			if (int.TryParse(HttpContext.Current.User.Identity.Name, out id)) {
				return new SiteUser(id);
			}
		}

		return SiteUser.GetAnonymousUser();
	}

	public static bool IsUserAuthorized() {
		if (HttpContext.Current != null) {

			// TODO: This is pretty lame.
			if (HttpContext.Current.User.IsInRole(UserType.Admin.ToString()) ||
				HttpContext.Current.User.IsInRole(UserType.User.ToString())) {
				return true;
			}
		}

		return false;
	}

	public static bool IsUserAuthorized(int id) {
		SiteUser siteUser = new SiteUser(id);

		// TODO: This is also pretty lame.
		if (siteUser.UserType == UserType.User ||
			siteUser.UserType == UserType.Admin) {
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

	public DateTime DateTime
	{
		get {
			return dateTime;
		}
	}
	#endregion
}