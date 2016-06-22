using System;
using System.Web.Routing;

namespace WARP
{
    public class Global : System.Web.HttpApplication
    {
        public void Application_OnStart()
        {
            Application["ActiveSession"] = 0;
            RegRoutes(RouteTable.Routes);
        }

        public void Session_OnStart()
        {
            Application.Lock();
            Application["ActiveSession"] = (int)Application["ActiveSession"] + 1;
            Application.UnLock();
        }

        public void Session_OnEnd()
        {
            Application.Lock();
            Application["ActiveSession"] = (int)Application["ActiveSession"] - 1;
            Application.UnLock();
        }

        private void RegRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("default", "Default/{pBase}", "~/Default.aspx");
            routes.MapPageRoute("archive", "Archive/{pBase}/{pPage}", "~/Archive/Archive.aspx");

            //routes.MapPageRoute("user", "admin/user", "~/Admin/User.aspx");
            //routes.MapPageRoute("role", "admin/role", "~/Admin/Role.aspx");
            //routes.MapPageRoute("journalcommon", "admin/journalcommon", "~/Admin/JournalCommon.aspx");
            //routes.MapPageRoute("sesval", "admin/sesval", "~/Admin/SesVal.aspx");
            //routes.MapPageRoute("base", "admin/base", "~/Admin/Base.aspx");
            //routes.MapPageRoute("settings", "service/{p_base}/settings", "~/service/settings.aspx");

            //routes.MapPageRoute("table", "admin/table", "~/Admin/Table.aspx");
            //routes.MapPageRoute("twit", "admin/twit", "~/Admin/Twit.aspx");
            //routes.MapPageRoute("frm", "sprav/frm", "~/sprav/Frm.aspx");

            //routes.MapPageRoute("journal", "service/{p_base}/journal", "~/service/Journal.aspx");
            //routes.MapPageRoute("doctree", "sprav/doctree", "~/sprav/Doctree.aspx");
            //routes.MapPageRoute("person", "sprav/{p_base}/person", "~/sprav/Person.aspx");
            //routes.MapPageRoute("help", "help", "~/Site/Help.aspx");
            //routes.MapPageRoute("blog", "blog", "~/Site/Blog.aspx");
            //routes.MapPageRoute("docversion", "docversion/{p_base}", "~/Archive/Docversion.aspx");
            //routes.MapPageRoute("archivedel", "archivedel/{p_base}/{p_page}", "~/Archive/ArchiveDel.aspx");
            //routes.MapPageRoute("changedoctype", "changedoctype/{p_base}", "~/Archive/ChangeDoctype.aspx");
            //routes.MapPageRoute("shutdown", "service/shutdown", "~/Start.aspx");
            //routes.MapPageRoute("access", "admin/access", "~/Admin/AccessKeys.aspx");
            //routes.MapPageRoute("userrolebase", "admin/userrolebase", "~/Admin/UserRoleBase.aspx");
            //routes.MapPageRoute("uservisit", "admin/uservisit", "~/Admin/UserVisit.aspx");
            //routes.MapPageRoute("usersetting", "admin/usersetting", "~/Admin/UserSetting.aspx");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                Exception lastError = Server.GetLastError();
                if (lastError != null)
                {
                    Session["ErrorException"] = lastError;//.InnerException;
                    Server.ClearError();

                    Response.Redirect("/Site/AppError.aspx");
                }
            }
            catch (Exception ex)
            {
                Response.Write("Критическая ошибка. Закройте все окна браузера и попробуйте ещё раз. " + ex.Message);
            }
        }
    }
}