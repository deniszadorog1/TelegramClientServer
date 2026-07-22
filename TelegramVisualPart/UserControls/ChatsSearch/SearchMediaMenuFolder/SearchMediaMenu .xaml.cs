using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using System.Windows.Input;
using TelegramLib.MainClasses;
using TelegramVisualPart.Pages.ChatActions.MessageMenuPages;

namespace TelegramVisualPart.UserControls.ChatsSearch.SearchMediaMenuFolder
{
    /// <summary>
    /// Логика взаимодействия для SearchMediaMenu.xaml
    /// </summary>
    public partial class SearchMediaMenu : UserControl
    {
        private TelegramLib.MainClasses.Messages.Message _mes;
        private TelSystem _system;
        private MainWindow _main;
        public SearchMediaMenu(
            MainWindow main,
            TelSystem system,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            _main = main;
            _mes = mes;
            _system = system;

            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            GoToMessage.SetParams(PackIconKind.EyeOutline, "Go to message");
            ForwardMessage.SetParams(PackIconKind.ArrowRight, "Forward message");
        }

        private void GoToMessage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _main.ShowChosenMessageByMessageId(_mes.Id);
        }

        private void ForwardMessage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ForwardToPage forward = new ForwardToPage(_system, _mes);
            _main.SetSecondaryFrame(forward);
        }

    }
}
