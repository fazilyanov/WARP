<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppError.aspx.cs" Inherits="WARP.AppError" %>

<html>
<head runat="server">
    <title>Ошибка</title>
</head>
<body >
    <form id="form1" runat="server">
     <div style="font-size:22px;padding-top:100px;width: 50%; margin: 0 auto; text-align: center;">
        Ошибка приложения. Администратор проинформирован. <br /><br /><a style="text-decoration:none;" href="<%Response.Write(GetRouteUrl("default", new { pBase = "dbselect" })); %>">Вернуться на сайт</a>
     </div>
      <div style="width:50%;padding-top:50px;text-align: center; font-size:12px; margin: 0 auto;">
          <%=errorText%>
      </div>
    </form>
</body>
</html>
