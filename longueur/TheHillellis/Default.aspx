<%@ Page Language="C#" MasterPageFile="~/TheHillellis/TheHillellis.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="TheHillellis_Default" %>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">	
	<div id="leftContent">
		<lng:content ID="ctlLeftContent" runat="server" />
	</div>
	<div id="middleSeperator">
		&nbsp;
	</div>
	<div id="rightContent">
		<lng:content ID="ctlRightContent" runat="server" />
	</div>
</asp:Content>