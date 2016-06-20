using System;

namespace WARP.ErrorPages
{
    public partial class AppError : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        //private void SendMail()
        //{
        //    System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage();
        //    mm.From = new System.Net.Mail.MailAddress(Properties.Settings.Default.ArchiveMail);
        //    mm.To.Add(new System.Net.Mail.MailAddress(Properties.Settings.Default.AdminMail));
        //    mm.Subject = "Ошибка в Веб Архиве";
        //    mm.IsBodyHtml = true;//письмо в html формате (если надо)
        //    string _str = "";
        //    _str = "<b>Пользователь:</b> " + (Session["user_name"] ?? Context.User.Identity.Name.Trim()).ToString() + "<br/>";
        //    _str += (Session["user_login"] == null ? "" : " Логин: " + Session["user_login"].ToString()) + "<br/>";
        //    _str += (Session["user_winlogin"] == null ? "" : " WinLogin: " + Session["user_winlogin"].ToString()) + "<br/>";
        //    _str += (Session["user_mail"] == null ? "" : " E-mail: " + Session["user_mail"].ToString()) + "<br/>";
        //    _str += (Session["user_location"] == null ? "" : " Место: " + Session["user_location"].ToString()) + "<br/>";
        //    _str += "Ошибка:<br/><p>" + Session["ErrorException"].ToString() + "</p><br/><br/>";

        //    _str += faFunc.GetSessionValues();
            

        //    mm.Body = _str;

        //    faFunc.SendMail(mm);
        //    faFunc.ToLog(8, "Об общей ошибке приложения");
        //}

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Session["ErrorException"] != null )
            {
                Label1.Text = (Session["ErrorException"] ?? "").ToString();
                SendMail();
            }
            Session.Clear();
            ViewState.Clear();
        }

    }
}