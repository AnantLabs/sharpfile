<%@ Page Language="C#" MasterPageFile="~/Slog/slog.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Slog_Default" %>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
	<asp:Repeater ID="rptContent" runat="server">
		<ItemTemplate>
			<div class="title">
				<%# DataBinder.Eval(Container.DataItem, "Title").ToString() %>
			</div>
			<div class="date">
				<%# DataBinder.Eval(Container.DataItem, "DateTime").ToString() %>
			</div>
			<br />
			
			<div class="user">
				<%# DataBinder.Eval(Container.DataItem, "Username").ToString() %>
			</div>
			<br />
			
			<div class="content">
				<%# DataBinder.Eval(Container.DataItem, "Content").ToString() %>
			</div>
			<br />
		</ItemTemplate>
	</asp:Repeater>
</asp:Content>

