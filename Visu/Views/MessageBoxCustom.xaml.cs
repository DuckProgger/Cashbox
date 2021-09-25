using System.Windows;

namespace Cashbox.Visu
{
    /// <summary>
    /// Логика взаимодействия для MessageBoxCustom.xaml
    /// </summary>
    public partial class MessageBoxCustom : Window
    {
        public MessageBoxCustom(string message, MessageType type, MessageButtons buttons)
        {
            InitializeComponent();
            txtMessage.Text = message;
            switch (type)
            {

                case MessageType.Info:
                    txtTitle.Text = "Информация";
                    break;
                case MessageType.Confirmation:
                    txtTitle.Text = "Подтверждение";
                    break;
                case MessageType.Success:
                    txtTitle.Text = "Выполнено";
                    break;
                case MessageType.Warning:
                    txtTitle.Text = "Предупреждение";
                    break;
                case MessageType.Error:
                    txtTitle.Text = "Ошибка";
                    break;
                default:
                    break;
            }
            switch (buttons)
            {
                case MessageButtons.OkCancel:
                    btnYes.Visibility = Visibility.Collapsed; btnNo.Visibility = Visibility.Collapsed;
                    break;
                case MessageButtons.YesNo:
                    btnOk.Visibility = Visibility.Collapsed; btnCancel.Visibility = Visibility.Collapsed;
                    break;
                case MessageButtons.Ok:
                    btnOk.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnYes.Visibility = Visibility.Collapsed; btnNo.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        public static void Show(string message, MessageType type, MessageButtons buttons)
        {
            new MessageBoxCustom(message, type, buttons).ShowDialog();
        }
    }  

    public enum MessageType
    {
        Info,
        Confirmation,
        Success,
        Warning,
        Error,
    }
    public enum MessageButtons
    {
        OkCancel,
        YesNo,
        Ok,
    }
}
