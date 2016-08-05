namespace WARP
{
    // Описывает столбец таблицы
    public class TableColumn
    {
        // Псевдоним поля после join'а в запросе 
        public string DataLookUpResult { get; set; } = string.Empty;

        // Таблица для связи
        public string DataLookUpTable { get; set; } = string.Empty;

        // Имя поля в SQL таблице
        public string DataNameSql { get; set; } = string.Empty;

        public TableColumnType DataType { get; set; } = TableColumnType.String;

        // Разрешено ли менять поле при массовом редактировании
        public bool EditBulk { get; set; } = false;

        // Текст по умолчанию для значений из спарвочника
        public string EditDefaultText { get; set; } = string.Empty;

        // Значение по умолчанию или ID для значений из спарвочника
        public string EditDefaultValue { get; set; } = string.Empty;

        // Подсказка снизу для поля при редактировании
        public string EditFieldInfo { get; set; } = string.Empty;

        // Максимальная длинна для текста или значение для целочисленных данных
        public int EditMax { get; set; } = -1;

        // Минимальная длинна для текста или значение для целочисленных данных
        public int EditMin { get; set; } = -1;

        // Обязательность заполнения
        public bool EditRequired { get; set; } = false;

        // Типы редактирования (влияет на контролы, правила сохранения и тд)
        public TableColumnEditType EditType { get; set; } = TableColumnEditType.None;

        // Заголовок для формы фильтров, если не указан, используется обычный заголовок
        public string FilterCaption { get; set; } = string.Empty;

        // Предустановленный фильтр для поля
        public string FilterDefaultValue { get; set; } = string.Empty;

        // Тип фильтра
        public TableColumnFilterType FilterType { get; set; } = TableColumnFilterType.None;

        // Тип фильтра
        public bool FilterShowInToolBar { get; set; } = false;

        // Выравнивание в ячейке текста
        public TableColumnAlign ViewAlign { get; set; } = TableColumnAlign.Left;

        // Заголовок столбца (Грид, карточка, форма редактирования и тд)
        public string ViewCaption { get; set; } = string.Empty;

        // Короткий заголовок, пока не использую
        public string ViewCaptionShort { get; set; } = string.Empty;

        // Ширина поля в гриде
        public int ViewWidth { get; set; } = 100;
    }
}