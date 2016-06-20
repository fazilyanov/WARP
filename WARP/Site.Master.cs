using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

/// scio me nihil scire
/// 19.06.2016
/// Artur Fazilyanov
namespace WARP
{
    public partial class Site : System.Web.UI.MasterPage
    {
        /// <summary>
        ///
        /// </summary>
        public string
             cur_id_base = "", cur_basename = "", _tmp = "", OpenHelp = "";

        protected DataTable GetUserInfo(string login)
        {
            return ComFunc.GetData("SELECT * FROM User WHERE Del = 0 AND Login = @login", new SqlParameter("login", login));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Для отладки: если ВСЕГДА надо загружать данные из базы
            // Session["user_login"] = null;

            // Если пользователь еще не авторизировался (любым из способов)
            if (Session["UserLogin"] == null)
            {
                // Попробуем выполнить windows авторизацию
                string WinLogin = Context.User.Identity.Name.Trim().ToLower();// Достаем windows логин
                WinLogin = WinLogin.Substring(WinLogin.LastIndexOf('\\') + 1);// Чистим
                DataTable dt = GetUserInfo(WinLogin);// Ищем в базе
                if (dt.Rows.Count==0)// если нет такого, просим залогиниться вручную
                {
                    //Достаем логин пользователя
                    if ((Session["UserFormLogin"] ?? "").ToString() == "")
                    {
                        Response.Redirect("Logon.aspx");
                    }
                }
                else // Раз есть - считываем данные в сессию
                {
                    Session["UserId"] = dt.Rows[0]["ID"];
                    Session["UserLogin"] = dt.Rows[0]["Login"];
                    Session["UserName"] = dt.Rows[0]["Name"];

                }

                
                    // Определяем местоположение.. пока не нужно..
                    // но возможно файлы будут открываться с разных серверов

                    
                    Session["user_mail"] = rdr["mail"];

                    rdr.Close();

                    // Если есть логин в базе
                    // то возможно и есть доступ
                    // к одной из баз, ищем.. SelectUserRoleBase Session["user_id"]
                    _tmp =
                        "SELECT a.id_role,_base.id as baseid,_base.name as basename, _base.namerus AS basenamerus, _base.tabindex, _base.active " +
                        "FROM  _base " +
                        "LEFT JOIN ( SELECT id_role, id_base FROM _user_role_base WHERE del=0 AND id_user=@p_id_user) a ON a.id_base = _base.id " +
                        "WHERE _base.del = 0 ORDER BY _base.tabindex";
                    cmd = new SqlCommand(_tmp, conn);
                    cmd.Parameters.AddWithValue("@p_id_user", Session["user_id"].ToString());
                    rdr = cmd.ExecuteReader();

                    string menu_other = "", list_other = "";
                    _tmp = "";
                    while (rdr.Read())
                    {
                        _base_name = rdr["basename"].ToString(); // Имя базы
                        if (rdr["id_role"].ToString() != "")
                        {
                            Session[_base_name] = rdr["basenamerus"].ToString();
                            Session[_base_name + "_id"] = rdr["baseid"].ToString();
                            Session[_base_name + "_id_role"] = rdr["id_role"].ToString();
                            _tmp = String.Format(menu_item_enabled, GetRouteUrl("default", new { p_base = _base_name }), rdr["basenamerus"].ToString(), "gi gi-blank");
                            if ((bool)rdr["active"]) menu_enabled += _tmp;
                            else menu_other += _tmp;
                            _tmp = String.Format(list_item_enabled_blank, GetRouteUrl("default", new { p_base = _base_name }), rdr["basenamerus"].ToString(), "gi gi-blank");
                            if ((bool)rdr["active"]) list_enabled += _tmp;
                            else list_other += _tmp;
                        }
                        else
                        {
                            menu_disabled += String.Format(menu_item_disabled, rdr["basenamerus"].ToString(), "gi gi-lock");
                            list_disabled += String.Format(list_item_disabled, rdr["basenamerus"].ToString(), "gi gi-lock");
                        }
                    }
                    Session["menubase"] =
                        "<li role='presentation' class='dropdown-header'>Доступные базы:</li>" + menu_enabled +
                        "<li role='presentation' class='dropdown-header'>Другие:</li>" + menu_other + menu_disabled;

                    Session["listbase"] = "<a href='#' class='list-group-item' style='padding:5px 0px 0px 10px;font-weight: 600;'>Доступные:</a>" + list_enabled +
                        "<a href='#' class='list-group-item' style='padding:5px 0px 0px 10px;font-weight: 600;'>Другие:</a>" + list_other + list_disabled;// Тут храним HTML код списка баз на странице выбора

                    rdr.Close();
                    cmd.Dispose();
                    // Грузим общие доступа
                    _tmp = "SELECT b.name  FROM _user_access a left join _access b ON a.id_access=b.id where a.id_user=@p_id_user AND a.del=0";
                    cmd = new SqlCommand(_tmp, conn);
                    cmd.Parameters.AddWithValue("@p_id_user", Session["user_id"].ToString());
                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                        Session["common_access_" + rdr["name"].ToString()] = 1;
                    rdr.Close();
                    cmd.Dispose();
                    if (Session["common_access_admin_menu"] != null)
                        Session["menuadmin"] =
                             String.Format(submenu_begin, "Пользователи") +
                            (Session["common_access_admin_user_view"] != null || Session["common_access_admin_user_edit"] != null ? String.Format(menu_item_enabled_blank, GetRouteUrl("user", new { }), "Пользователи", "gi gi-group") : String.Format(menu_item_disabled, "Пользователи", "gi gi-group")) +
                            (Session["common_access_admin_role_view"] != null || Session["common_access_admin_role_edit"] != null ? String.Format(menu_item_enabled_blank, GetRouteUrl("role", new { }), "Роли", "gi gi-keys") : String.Format(menu_item_disabled, "Роли", "gi gi-keys")) +
                            (Session["common_access_admin_access_view"] != null ? String.Format(menu_item_enabled_blank, GetRouteUrl("access", new { }), "Ключи доступа", "gi gi-blank") : String.Format(menu_item_disabled, "Ключи доступа", "gi gi-blank")) +
                            (Session["common_access_admin_user_view"] != null || Session["common_access_admin_user_edit"] != null ? String.Format(menu_item_enabled_blank, GetRouteUrl("userrolebase", new { }), "Доступы пользователей (Отчет)", "gi gi-blank") : String.Format(menu_item_disabled, "Доступы пользователей (Отчет)", "gi gi-blank")) +
                            (Session["common_access_admin_user_view"] != null || Session["common_access_admin_user_edit"] != null ? String.Format(menu_item_enabled_blank, GetRouteUrl("usersetting", new { }), "Настройки пользователей", "gi gi-blank") : String.Format(menu_item_disabled, "Настройки пользователей", "gi gi-blank")) +
                            submenu_end +

                           String.Format(submenu_begin, "Служебные") +
                            (Session["common_access_admin_journal_common"] != null ? String.Format(menu_item_enabled_blank, GetRouteUrl("journalcommon", new { }), "Общий журнал", "gi gi-history") : String.Format(menu_item_disabled, "Общий журнал", "gi gi-history")) +
                             String.Format(menu_item_enabled_blank, GetRouteUrl("sesval", new { }), "Сессия", "gi gi-cogwheel") +
                            (Session["common_access_admin_base_view"] != null ? String.Format(menu_item_enabled_blank, GetRouteUrl("base", new { }), "Список баз", "gi gi-blank") : String.Format(menu_item_disabled, "Список баз", "gi gi-blank")) +
                            (Session["common_access_admin_table_view"] != null ? String.Format(menu_item_enabled_blank, GetRouteUrl("table", new { }), "Список таблиц", "gi gi-blank") : String.Format(menu_item_disabled, "Список таблиц", "gi gi-blank")) +
                            (Session["common_access_admin_ad"] != null ? String.Format(menu_item_enabled_blank, GetRouteUrl("twit", new { }), "Сообщение всем", "gi gi-blank") : String.Format(menu_item_disabled, "Сообщение всем", "gi gi-blank")) +
                            submenu_end;

                    // TODO : Автоматизировать авто очичтку
                    faFunc.ToLog(1);

                    // Меню справочников (общие - нет привязки к конкретной базе, добавляется к меню конкретной базы)
                    string[] common_sprav = { "frm", "doctree", };
                    string[] common_spravru = { "Фирмы", "Формы документов" };
                    menu_enabled = "";
                    list_enabled = "";
                    /* По умолчанию, доступ к справочникам есть у всех, права выдаются только на редактирование*/
                    for (int i = 0; i < common_sprav.Length; i++)
                    {
                        menu_enabled += String.Format(menu_item_enabled, GetRouteUrl(common_sprav[i], new { }), common_spravru[i], "gi gi-blank");
                        list_enabled += String.Format(list_item_enabled_blank, GetRouteUrl(common_sprav[i], new { }), common_spravru[i], "gi gi-blank");
                    }
                    Session["menusprav"] = menu_enabled;
                    Session["listsprav"] = list_enabled;

                    ///////////

                    // Меню сервисов (общие - нет привязки к конкретной базе, добавляется к меню конкретной базы)
                    string[] common_service = { "shutdown", "log" };
                    string[] common_serviceru = { "Перезапустить текущий сеанс", "Лог" };
                    string[] common_serviceicon = { "gi gi-rotation_lock", "gi gi-history" };
                    menu_enabled = "";
                    list_enabled = "";

                    for (int i = 0; i < common_service.Length; i++)
                    {
                        if (Session["common_access_service_" + common_service[i]] != null || i == 0)
                        {
                            menu_enabled += String.Format(menu_item_enabled, GetRouteUrl(common_service[i], new { p_base = cur_basename }), common_serviceru[i], common_serviceicon[i]);
                            //list_enabled += String.Format(list_item_enabled, GetRouteUrl(common_service[i], new { p_base = cur_basename }), common_serviceru[i], common_serviceicon[i]);
                        }
                        else
                        {
                            menu_enabled += String.Format(menu_item_disabled, common_serviceru[i], common_serviceicon[i]);
                            //list_enabled += String.Format(list_item_disabled, common_serviceru[i], common_serviceicon[i]);
                        }
                    }

                    Session["menuservice"] = menu_enabled;
                    //Session["listservice"] = list_enabled;
                }
                // Если нет - накуй его
                else
                {
                    rdr.Close();
                    conn.Close();
                    //Response.Redirect(GetRouteUrl("error", new { p_base = "wtf", p_error = "usernotfound" }));
                    // Response.Clear();
                    Response.Write(faFunc.Alert(faAlert.SiteAccessDenied));
                    //Response.End();
                }
                rdr.Close();
                conn.Close();
            }//Session["user_login"]

            // Получаем параметр текущей базы данных
            // Достаем доступа и настройки относящиеся к этой базе

            cur_basename = Page.RouteData.Values["p_base"] != null ? Page.RouteData.Values["p_base"].ToString() : "";
            if (cur_basename != "error" && Session["user_login"] != null)
            {
                // TODO : Последние изменения в отдельной вкладке
                /*
                if (Session["NewChangeLogCount"] == null)
                {
                    // Достаем id последней прочитанной новости
                    int last_msg_id = 0;
                    int.TryParse(faFunc.GetUserSetting("last_msg_id"), out last_msg_id);

                    // Достаем общее количество новостей
                    int all_change_log_count = 0;
                    int.TryParse(faFunc.GetSiteSetting("all_change_log_count"), out all_change_log_count);

                    Session["NewChangeLogCount"] = all_change_log_count - last_msg_id;
                }
                // Открытие дополнительной вкладки со списком изменений если есть новые
                OpenHelp = ((int)(Session["NewChangeLogCount"] ?? 0) > 0) ? "<script type='text/javascript'>window.open('" + GetRouteUrl("blog", new { }) + "', 'История изменений');</script> " : "";

                */

                // Если пустой - кидаем на страницу выбора баз
                //if (cur_basename == "") Response.Redirect(GetRouteUrl("default", new { p_base = "dbselect" }));
                // Если указана какая то база - проверяем доступ к ней
                if (cur_basename != "" && cur_basename != "dbselect" && Session[cur_basename + "_loaded"] == null)
                {
                    conn.Open();
                    cmd = new SqlCommand("SELECT id FROM _base WHERE name=@p_basename and del=0", conn);
                    cmd.Parameters.AddWithValue("@p_basename", cur_basename);
                    sqlDataAdapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    sqlDataAdapter.Fill(dt);
                    cur_id_base = dt.Rows[0]["id"].ToString();
                    if (cur_id_base == "")
                        Response.Redirect(GetRouteUrl("default", new
                        {
                            p_base = "dbselect"
                        }));

                    _tmp =
                         "SELECT c.name as akey " +
                         "FROM _user_role_base a " +
                         "LEFT JOIN _role_access b on a.id_role=b.id_role and b.del=0 " +
                         "LEFT JOIN _access c on b.id_access=c.id " +
                         "WHERE a.id_user=@p_id_user AND a.id_base=@p_id_base AND a.del=0 ";
                    cmd = new SqlCommand(_tmp, conn);
                    cmd.Parameters.AddWithValue("@p_id_user", Session["user_id"].ToString());
                    cmd.Parameters.AddWithValue("@p_id_base", cur_id_base);
                    sqlDataAdapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    sqlDataAdapter.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                        Session[cur_basename + "_access_" + r["akey"].ToString()] = 1;
                    cmd.Dispose();

                    _tmp =
                         "SELECT b.id_table,c.name as [table], b.value FROM _user_role_base a " +
                         "JOIN _role_where b on a.id_role=b.id_role AND b.del=0 " +
                         "JOIN _table c on b.id_table=c.id AND c.del=0 " +
                         "WHERE a.id_user=@p_id_user AND a.id_base=@p_id_base AND a.del=0 ";
                    cmd = new SqlCommand(_tmp, conn);
                    cmd.Parameters.AddWithValue("@p_id_user", Session["user_id"].ToString());
                    cmd.Parameters.AddWithValue("@p_id_base", cur_id_base);
                    sqlDataAdapter = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    sqlDataAdapter.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                        Session[cur_basename + "_role_where_" + r["table"].ToString()] = r["value"].ToString();
                    cmd.Dispose();

                    // Меню разделов
                    string _menu = "", _list = "";
                    menu_enabled = menu_disabled = "";
                    list_enabled = list_disabled = "";
                    string[] _p = { "acc", "dog", "ord", "oth", "empl", "ohs", "tech", "bank", "norm" };
                    foreach (string name in _p)//Enum.GetNames(typeof(faPage))
                    {
                        _tmp = faFunc.GetDocTypeName(name);
                        if (Session[cur_basename + "_access_archive_" + name + "_view"] != null || Session[cur_basename + "_access_archive_" + name + "_edit"] != null)
                        {
                            menu_enabled += String.Format(menu_item_enabled, GetRouteUrl("archive", new { p_base = cur_basename, p_page = name }), _tmp, "gi gi-blank");
                            list_enabled += String.Format(list_item_enabled, GetRouteUrl("archive", new { p_base = cur_basename, p_page = name }), _tmp, "gi gi-blank");
                        }
                        else
                        {
                            menu_disabled += String.Format(menu_item_disabled, _tmp, "gi gi-lock");
                            list_disabled += String.Format(list_item_disabled, _tmp, "gi gi-lock");
                        }
                    }

                    _menu = String.Format(menu_item_enabled, GetRouteUrl("archive", new { p_base = cur_basename, p_page = "srch" }), faFunc.GetDocTypeName("srch"), "gi gi-search") +
                       "<li role='presentation' class='dropdown-header'>Тип документа:</li>" + menu_enabled + menu_disabled + "<li role='presentation' class='dropdown-header'>Другое:</li>";

                    _list = "<a href=\"#\" class=\"list-group-item navbar-default\" style=\"font-size:15px;\">Архив (" + (Session[cur_basename] ?? "").ToString() + ")</a>" +
                        String.Format(list_item_enabled, GetRouteUrl("archive", new { p_base = cur_basename, p_page = "srch" }), faFunc.GetDocTypeName("srch"), "gi gi-search") +
                         "<a href='#' class='list-group-item' style='padding:5px 0px 0px 10px;font-weight: 600;'>Тип документа:</a>" + list_enabled + list_disabled +
                         "<a href='#' class='list-group-item' style='padding:5px 0px 0px 10px;font-weight: 600;'>Другое:</a>";

                    if (Session[cur_basename + "_access_docversion_view"] != null || Session[cur_basename + "_access_docversion_edit"] != null)
                    {
                        _menu += String.Format(menu_item_enabled, GetRouteUrl("docversion", new { p_base = cur_basename }), "Версии", "gi gi-blank");
                        _list += String.Format(list_item_enabled, GetRouteUrl("docversion", new { p_base = cur_basename }), "Версии", "gi gi-blank");
                    }
                    else
                    {
                        _menu += String.Format(menu_item_disabled, "Версии", "gi gi-lock");
                        _list += String.Format(list_item_disabled, "Версии", "gi gi-lock");
                    }

                    if (Session[cur_basename + "_access_archive_acc_edit"] != null)
                    {
                        _menu += String.Format(menu_item_enabled, GetRouteUrl("archivedel", new { p_base = cur_basename, p_page = "srch" }), "Удаленные документы", "gi gi-blank");
                        _list += String.Format(list_item_enabled, GetRouteUrl("archivedel", new { p_base = cur_basename, p_page = "srch" }), "Удаленные документы", "gi gi-blank");
                    }
                    else
                    {
                        _menu += String.Format(menu_item_disabled, "Удаленные документы", "gi gi-lock");
                        _list += String.Format(list_item_disabled, "Удаленные документы", "gi gi-lock");
                    }

                    Session[cur_basename + "_menupage"] = _menu;
                    Session[cur_basename + "_listpage"] = _list;

                    // Меню справочников
                    string[] sprav = { "person" };//"country", "region", "town",
                    string[] spravru = { "Сотрудники" };//"Страны", "Регионы", "Населенные пункты",
                    menu_enabled = "";
                    list_enabled = "";
                    /* По умолчанию, доступ к справочникам есть у всех, права выдаются только на редактирование*/

                    for (int i = 0; i < sprav.Length; i++)
                    {
                        menu_enabled += String.Format(menu_item_enabled_blank, GetRouteUrl(sprav[i], new { p_base = cur_basename }), spravru[i], "gi gi-blank");
                        list_enabled += String.Format(list_item_enabled_blank, GetRouteUrl(sprav[i], new { p_base = cur_basename }), spravru[i], "gi gi-blank");
                    }
                    Session[cur_basename + "_menusprav"] = menu_enabled;
                    Session[cur_basename + "_listsprav"] = list_enabled;

                    // Меню сервис
                    string[] service = { "journal", "settings" };
                    string[] serviceru = { "Журнал изменений", "Настройки" };
                    string[] serviceicon = { "gi gi-history", "gi gi-settings" };
                    menu_enabled = "";

                    for (int i = 0; i < service.Length; i++)
                    {
                        menu_enabled += String.Format(menu_item_enabled_blank, GetRouteUrl(service[i], new { p_base = cur_basename }), serviceru[i], serviceicon[i]);
                    }

                    Session[cur_basename + "_menuservice"] = menu_enabled;

                    Session[cur_basename + "_loaded"] = 1;

                    conn.Close();
                }
            }
        }
    }
}