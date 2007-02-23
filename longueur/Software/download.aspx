<%@ Page Language="C#" AutoEventWireup="true" CodeFile="download.aspx.cs" Inherits="Software_download" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>longueur.org >> Software >> Download <%# filename %></title>
</head>
<body>
    Oops, looks like I couldn't find the file you requested. Maybe you should <a href="<%# Request.UrlReferrer.ToString() %>">return from whence you came</a> and try again?
</body>
</html>
