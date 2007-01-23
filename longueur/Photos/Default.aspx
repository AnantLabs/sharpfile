<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Photos_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		Photos!
		
		<asp:Repeater ID="directories" runat="server">
			<HeaderTemplate>
				<table><tr>
			</HeaderTemplate>
			<ItemTemplate>
				<td><%# DataBinder.Eval(Container.DataItem, "photo") %></td>
			</ItemTemplate>
			<FooterTemplate>
				</tr></table>
			</FooterTemplate>
		</asp:Repeater>
    </div>
    </form>
</body>
</html>
