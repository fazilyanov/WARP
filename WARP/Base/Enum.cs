namespace WARP
{
    // Перечисление действий | операций над данными
    public enum TableAction
    {
        // Отсутствие
        None,

        // Создание записи средствами грида
        Create,

        // Создание записи в карточке
        CreateCard,

        // Редактирование записи средствами грида
        Edit,

        // Редактирование записи в карточке
        EditCard,

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