namespace Cashbox.Services
{
    public interface IDialogService
    {
        /// <summary>
        /// Открывает диалог открытия файла.
        /// </summary>
        /// <returns></returns>
        public bool OpenFileDialog();
        /// <summary>
        /// Открывает диалог сохранения файла.
        /// </summary>
        /// <returns></returns>
        public bool SaveFileDialog();
        /// <summary>
        /// Показывает сообщение.
        /// </summary>
        /// <param name="text"></param>
        public void ShowMessage(string text);
        /// <summary>
        /// Путь к выбранному файлу.
        /// </summary>
        string FilePath { get; set; }  
        /// <summary>
        /// Индекс формата файла.
        /// </summary>
        int SelectedFormat { get; set; }
    }
}
