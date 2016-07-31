namespace WARP
{
    // Перечисление действий | операций над данными
    public enum TableAction
    {
        // Отсутствие
        None,

        // Создание записи средствами грида
        Create,

        // Редактирование записи средствами грида
        Edit,

        // Удаление записи|записей
        Remove,
    }

    public enum TableColumnAlign
    {
        Left,
        Center,
        Right,
    }

    public enum TableColumnEditType
    {
        None,
        CurrentUser,
        CurrentDateTime,
        String,
        Autocomplete,
        Integer,
        Money,
        DropDown,
    }

    public enum TableColumnFilterType
    {
        None,
        String,
        Autocomplete,
        Integer,
        Money,
        DropDown,
    }

    public enum TableColumnType
    {
        String,
        Integer,
        Money,
        DateTime,
        Date,
    }

    public enum TableSortDir
    {
        Asc,
        Desc
    }
}