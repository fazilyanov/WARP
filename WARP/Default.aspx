<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WARP.Default" %>

<%@ MasterType VirtualPath="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph" runat="server">
    <div class="row">
        <div class="col-sm-4">
            </div>
        <div class="col-sm-4">
<ul class="list-group">
<%=htmlBaseList%>  
</ul>
            <div class="col-sm-4">
                </div>
            </div>       
        </div>
</asp:Content>
