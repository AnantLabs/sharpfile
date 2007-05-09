<%@ Page Language="C#" MasterPageFile="~/Slog/slog.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Slog_Default" Title="Untitled Page" %>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
	<asp:Repeater ID="rptContent" runat="server">
		<ItemTemplate>
			<div class="date">
				<%# DataBinder.Eval(Container.DataItem, "Date").ToString() %>
			</div>
			<br />
			
			<div class="post">
				<%# DataBinder.Eval(Container.DataItem, "Post").ToString() %>
			</div>
			<br />
		</ItemTemplate>
	</asp:Repeater>
</asp:Content>

