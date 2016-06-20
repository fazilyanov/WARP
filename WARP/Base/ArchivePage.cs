namespace WARP
{
    /// <summary>
    /// Перечисление типов документов / типа станицы
    /// </summary>
    public enum ArchivePage
    {
        /// <summary>
        /// Нет
        /// </summary>
        None = 0,

        /// <summary>
        /// Бухгалтерские
        /// </summary>
        Acc = 7,

        /// <summary>
        /// Договоры
        /// </summary>
        Dog = 15,

        /// <summary>
        /// ОРД
        /// </summary>
        Ord = 2,

        /// <summary>
        /// Прочие
        /// </summary>
        Oth = 56,

        /// <summary>
        /// По личному составу
        /// </summary>
        Empl = 50,

        /// <summary>
        /// Охрана труда
        /// </summary>
        Ohs = 60,

        /// <summary>
        /// Технические
        /// </summary>
        Tech = 11,

        /// <summary>
        /// Страница поиска, включает все доступные типы документов
        /// </summary>
        All = 1000,

        /// <summary>
        /// Страница выбора
        /// </summary>
        Select = 1001,

        /// <summary>
        /// Банковские документы
        /// </summary>
        Bank = 5574,

        /// <summary>
        /// Нормативные документы
        /// </summary>
        Norm = 5596
    }
}