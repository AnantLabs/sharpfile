<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Photos_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
	"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head>
	<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
	<title>CSS Layout - 100% height</title>
	<link rel="stylesheet" type="text/css" href="styles.css" />
</head>
<body>
	<div id="header">
		Photos
	</div>
	
	<div id="sidebar">
			<asp:Repeater ID="rptPhotos" runat="server">
				<ItemTemplate>
					<img src='<%# Eval("ThumbnailUrl") %>' /><br />
				</ItemTemplate>
			</asp:Repeater>
	</div>
	<div id="container">
		<div id="content">
			<asp:Image ID="imgCurrent" runat="server" />
		</div>
	</div>
	
	<div id="footer">
		Stuff here.
	</div>
</body>
</html>