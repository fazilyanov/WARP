using System;
using System.Data;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace WARP
{
    /// <summary>
    /// Обрабатывает запросы на получение файлов
    /// </summary>
    public class GetFileHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            // Получаем параметры от грида
            string curBase = context.Request["curBase"];
            string curTable = context.Request["curTable"];
            string IdFile = context.Request["IdFile"];
            string key = context.Request["key"];
            context.Response.Clear();
            if (Func.GetMd5Hash(Func.GetMd5Hash(IdFile) + IdFile)== key)
            {
                DataTable file = Func.GetData("SELECT * FROM [dbo].[" + curBase + curTable + "Files] WHERE Id = " + IdFile);
                if (file.Rows.Count > 0)
                {
                    Byte[] bytes = (Byte[])file.Rows[0]["fileData"];
                    string fileExt = Path.GetExtension(file.Rows[0]["fileName"].ToString()).ToUpper();
                    switch (fileExt)
                    {
                        case ".PDF":
                            context.Response.ContentType = "application/pdf";
                            break;

                        case ".JPG":
                        case ".JPEG":
                            context.Response.ContentType = "image/jpeg";
                            break;

                        case ".BMP":
                            context.Response.ContentType = "image/bmp";
                            break;

                        case ".XLS":
                            context.Response.ContentType = "application/vnd.ms-excel";
                            break;

                        case ".XLSX":
                            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            break;

                        case ".ZIP":
                            context.Response.ContentType = "application/zip";
                            break;

                        case ".CSV":
                            context.Response.ContentType = "text/csv";
                            break;

                        case ".PNG":
                            context.Response.ContentType = "image/png";
                            break;
                    }
                    switch (fileExt)
                    {
                        case ".PDF":
                        case ".JPG":
                        case ".JPEG":
                        case ".BMP":
                        case ".PNG":
                            context.Response.AddHeader("content-disposition", "filename=" + file.Rows[0]["fileName"].ToString());
                            break;
                        default:
                            context.Response.AddHeader("content-disposition", "attachment;filename=" + file.Rows[0]["fileName"].ToString());
                            break;
                    }

                    context.Response.AddHeader("content-length", bytes.Length.ToString());
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                    context.Response.Flush();
                    context.Response.End();
                }
            }
            else
            {
                context.Response.Write("Bad key!");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}