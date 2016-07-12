<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Archive.aspx.cs" Inherits="WARP.Archive" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">
    <%=tableData.GenerateHtml()%>
</asp:Content>