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
        CurrentDate,
        CurrentDateTime,
        Date,
        DateTime,
        String,
        Autocomplete,
        Integer,
        Money,
        DropDown,
        Files,
        Text
        
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
        LookUp,
        Files,
        Text
    }

    public enum TableSortDir
    {
        Asc,
        Desc
    }
}