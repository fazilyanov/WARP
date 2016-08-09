<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WARP.Default" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">
    <script type="text/javascript">
        jQuery(document).ready(function () {
        <%
        if (Master.curBase == string.Empty)
        {
        %>
            $('#ddBase').addClass('open');
        <%
        }
        else
        {
        %>
            $('#ddDoc').addClass('open');
        <%
        }
        %>

        });
    </script>
</asp:Content>