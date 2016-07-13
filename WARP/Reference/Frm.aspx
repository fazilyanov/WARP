<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Frm.aspx.cs" Inherits="WARP.Frm" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">
    <%=tableData.GenerateHtml()%>
</asp:Content>