<%@ Page Language="C#" MasterPageFile="~/TheHillellis/TheHillellis.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="TheHillellis_Default" %>
<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder" Runat="Server">
	<script type="text/javascript" language="javascript">
		addListener(this, 'load', function() { onLoad(); });
		addListener(this, 'resize', function() { onLoad(); });
	</script>
	
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