<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppError.aspx.cs" Inherits="WARP.AppError" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body >
    <form id="form1" runat="server">
     <div style="font-size:22px;padding-top:100px;width: 50%; margin: 0 auto; text-align: center;">
        К сожалению, что-то пошло не так. Администратор проинформирован. <br /><br /><a style="text-decoration:none;" href="<%Response.Write(GetRouteUrl("default", new { p_base = "dbselect" })); %>">Вернуться на сайт</a>
     </div>
      <div style="width:50%;padding-top:50px;text-align: center; font-size:12px; margin: 0 auto;"><asp:Label ID="Label1" runat="server" Text=""></asp:Label></div>
    </form>
</body>
</html>
