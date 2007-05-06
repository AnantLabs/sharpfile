<%@ Application Language="C#" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started
		if (Session[Constants.CurrentUser] == null)
		{
			bool storedCookie = false;

			if (storedCookie)
			{
				//get the stored user cookie and set the session with it
			}
			else
			{
				Session[Constants.CurrentUser] = LongueurData.GetAnonymousUser();
			}
		}
		else
		{
			//if the session isn't null, I guess nothing should be done...
		}

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
