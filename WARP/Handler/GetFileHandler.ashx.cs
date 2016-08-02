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
            DataTable file = ComFunc.GetData("SELECT * FROM [dbo].[" + curBase + curTable + "Files] WHERE Id = " + IdFile);
            if (file.Rows.Count > 0)
            {
                Byte[] bytes = (Byte[])file.Rows[0]["fileData"];
                switch (Path.GetExtension(file.Rows[0]["fileName"].ToString()).ToUpper())
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
                }
                context.Response.AddHeader("content-length", bytes.Length.ToString());
                context.Response.AddHeader("content-disposition", "attachment;filename=" + file.Rows[0]["fileName"].ToString());
                context.Response.BinaryWrite(bytes);
                context.Response.Flush();
                context.Response.End();
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