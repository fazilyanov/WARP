using System.Collections.Generic;
using System.Text;

namespace WARP
{
    // Перечисление действий | операций над данными
    public enum Action
    {
        // Отсутствие
        None,

        // Создание записи средствами грида
        Create,

        // Редактирование записи средствами грида
        Edit,

        // Копирование записи средствами грида
        Copy,

        // Удаление записи|записей
        Remove,
    }

    public enum Align
    {
        Left,
        Center,
        Right,
    }

    public class FieldErrors
    {
        public string name { get; set; }
        public string status { get; set; }
    }

    public class RequestData
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }

    // Описывает столбец таблицы
    public class Field
    {
        // Имя поля в SQL таблице
        public string Name { get; set; } = string.Empty;

        public string Caption { get; set; } = string.Empty;

        // Выравнивание в ячейке текста
        public Align Align { get; set; } = Align.Left;

        // Ширина поля в гриде
        public int Width { get; set; } = 100;
    }

    public class Table
    {
        public List<Field> FieldList { get; set; } = null;
        public bool ShowRowInfoButton { get; set; } = false;

        // HTML Таблица
        public string GenerateHtmlTable()
        {
            StringBuilder sb = new StringBuilder();

            if (ShowRowInfoButton)
                sb.AppendLine("               <th></th>");
            foreach (Field field in FieldList)
                sb.AppendLine("               <th>" + field.Caption + "</th>");

            return sb.ToString();
        }

        public string GenerateJSTableColumns()
        {
            StringBuilder sb = new StringBuilder();
            if (ShowRowInfoButton)
                sb.AppendLine("                    {\"className\": 'details-control',\"orderable\": false,\"data\":null,\"defaultContent\": '', \"width\":\"20px\"},");
            foreach (Field field in FieldList)
                sb.AppendLine("                    { \"data\": \"" + field.Name + "\", className:\"dt-body-" + field.Align.ToString().ToLower() + "\", \"width\": \"" + field.Width + "px\" },");
            return sb.ToString();
        }
    }
}