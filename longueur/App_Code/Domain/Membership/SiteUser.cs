using System;
using System.Web;
using System.Web.Security;
using System.Data;
using Data;
using Common;

namespace Domain.Membership {
	/// <summary>
	/// Summary description for SiteUser.
	/// </summary>
	public class SiteUser {
		private const string anonymousName = "Anonymous";
		private const int formsAuthenticationVersion = 1;
		private const int cookieExpirationTime = 60 * 24;

		private int id;
		private string name;
		private string hashedPassword;
		private string email;
		private bool enableJs;
		private UserType userType;
		private DateTime dateTime;

		private bool isPopulated = false;

		public SiteUser() {
			this.id = -1;
			this.name = anonymousName;
			this.hashedPassword = string.Empty;
			this.userType = UserType.NonAuthenticated;
			this.email = string.Empty;
			this.enableJs = false;
			this.dateTime = DateTime.MinValue;
		}

		public SiteUser(string name, string email, string plainTextPassword) {
			int id = SiteUser.createUser(name, email, plainTextPassword, UserType.User);
			populateUser(id);
		}

		public SiteUser(string name, string email, string plainTextPassword, UserType userType) {
			int id = SiteUser.createUser(name, email, plainTextPassword, userType);
			populateUser(id);
		}

		public SiteUser(int id) {
			populateUser(id);
		}

		public SiteUser(string name) {
			populateUser(name);
		}

		public bool Login(string plainTextPassword) {
			return Login(plainTextPassword, true);
		}

		public bool Login(string plainTextPassword, bool persistent) {
			string encryptedPassword = Security.Encrypt(plainTextPassword);

			if (this.hashedPassword == encryptedPassword) {
				if (userType != UserType.NonAuthenticated) {
					string roles = userType.ToString();

					// Make sure our admin users are also users.
					if (userType == UserType.Admin) {
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

					// Set the cookie's expiration time to the tickets expiration time.
					if (ticket.IsPersistent) {
						cookie.Expires = ticket.Expiration;
					}

					// Add the cookie to the list for outgoing response
					if (HttpContext.Current != null) {
						HttpContext.Current.Response.Cookies.Add(cookie);
					}

					return true;
				}
			}

			return false;
		}

		public void Logout() {
			try {
				if (HttpContext.Current != null) {
					if (HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName] != null) {
						HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddYears(-30);
					}
				}
			} catch (Exception ex) {
				Data.Admin.InsertErrorLog(ex);
			}

			HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.PathAndQuery, true);
		}

		public void Update(string name, string email, string plainTextPassword) {
			Data.Membership.UpdateUser(this.id, name, email, plainTextPassword, this.userType);
		}

		public void Update(string name, string email, string plainTextPassword, UserType userType) {
			Data.Membership.UpdateUser(this.id, name, email, plainTextPassword, userType);
		}

		#region private instance methods
		private void populateUser(int id) {
			DataTable userData = Data.Membership.GetUserData(id);
			populateUserFromDataTable(userData);
		}

		private void populateUser(string name) {
			DataTable userData = Data.Membership.GetUserData(name);
			populateUserFromDataTable(userData);
		}

		private void populateUserFromDataTable(DataTable userTable) {
			if (userTable.Rows.Count > 0) {
				DataRow row = userTable.Rows[0];

				this.id = int.Parse(row["Id"].ToString());
				this.name = row["Name"].ToString();
				this.hashedPassword = row["Password"].ToString();
				this.email = row["Email"].ToString();
				this.enableJs = false;
				this.userType = (UserType)Enum.Parse(typeof(UserType), row["TypeName"].ToString());

				this.isPopulated = true;
			} else {
				throw new Exception("No user can be found.");
			}
		}
		#endregion

		#region Static methods
		public static bool IsCurrentUserAuthorized() {
			// TODO: This should be whacked for the 2nd way at some point.
			SiteUser siteUser = GetCurrentUser();

			if ((int)siteUser.UserType > (int)UserType.Admin) {
				return true;
			}

			if (HttpContext.Current != null) {
				if (HttpContext.Current.User.IsInRole(UserType.Admin.ToString()) ||
					HttpContext.Current.User.IsInRole(UserType.User.ToString())) {
					return true;
				}
			}

			return false;
		}

		public static bool Exists(string name) {
			return Data.Membership.UserExists(name);
		}

		public static SiteUser GetAnonymousUser() {
			int id = 0;
			DataTable result = Data.Membership.GetAnonymousUser();

			if (result.Rows.Count == 1) {
				if (int.TryParse(result.Rows[0]["Id"].ToString(), out id)) {
					//user = new SiteUser(int.Parse(result.Rows[0]["Id"].ToString()), result.Rows[0]["Name"].ToString(), string.Empty, result.Rows[0]["Email"].ToString(), false, ((UserType)Enum.Parse(typeof(UserType), result.Rows[0]["TypeName"].ToString())));
					return new SiteUser(id);
				}
			}

			return new SiteUser();
		}

		public static SiteUser GetCurrentUser() {
			if (HttpContext.Current != null) {
				int id = 0;

				if (int.TryParse(HttpContext.Current.User.Identity.Name, out id)) {
					return new SiteUser(id);
				}
			}

			return SiteUser.GetAnonymousUser();
		}

		public static void Update(int id, string name, string email, string plainTextPassword) {
			SiteUser siteUser = new SiteUser(id);

			Update(id, name, email, plainTextPassword, siteUser.UserType);
		}

		public static void Update(int id, string name, string email, string plainTextPassword, UserType userType) {
			SiteUser siteUser = GetCurrentUser();

			if (IsCurrentUserAuthorized()) {
				Data.Membership.UpdateUser(id, name, email, plainTextPassword, userType);
			} else {
				throw new Exception("You don't look like an admin!");
			}
		}

		public static void Create(string name, string email, string plainTextPassword, UserType userType) {
			if (IsCurrentUserAuthorized()) {
				createUser(name, email, plainTextPassword, userType);
			} else {
				throw new Exception("You don't look like an admin!");
			}
		}

		public static void Delete(int id) {
			if (IsCurrentUserAuthorized() ||
				GetCurrentUser().Id == id) {
				Data.Membership.DeleteUser(id);
			}
		}

		private static int createUser(string name, string email, string plainTextPassword, UserType userType) {
			if (!SiteUser.Exists(name)) {
				return Data.Membership.CreateUser(name, email, plainTextPassword, userType);
			} else {
				throw new ArgumentException("The user, " + name + ", already exists.");
			}
		}
		#endregion

		#region Public properties
		public int Id {
			get {
				return id;
			}
		}

		public string Name {
			get {
				return name;
			}
		}

		public string HashedPassword {
			get {
				return hashedPassword;
			}
		}

		public string Email {
			get {
				return email;
			}
		}

		public UserType UserType {
			get {
				return userType;
			}
		}

		//public string Guid
		//{
		//    get 
		//    {
		//        return guid; 
		//    }
		//}

		public bool EnableJs {
			get {
				return enableJs;
			}
			set {
				enableJs = value;
			}
		}

		public DateTime DateTime {
			get {
				return dateTime;
			}
		}
		#endregion

		internal static int GetId(string name) {
			DataTable userTable = Data.Membership.GetUserData(name);

			if (userTable.Rows.Count > 0) {
				return int.Parse(userTable.Rows[0]["Id"].ToString());
			}

			throw new ArgumentException("User, " + name + ", not found.");
		}
	}
}