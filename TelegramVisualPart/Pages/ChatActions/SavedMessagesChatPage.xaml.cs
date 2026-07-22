using System.Windows.Controls;

namespace TelegramVisualPart.Pages.ChatActions
{
    /// <summary>
    /// Логика взаимодействия для SavedMessagesChatPage.xaml
    /// </summary>
    public partial class SavedMessagesChatPage : Page
    {
        private TelegramLib.MainClasses.SavedMessagesChat _chat;
        public SavedMessagesChatPage(TelegramLib.MainClasses.SavedMessagesChat chat)
        {
            _chat = chat;

            InitializeComponent();

            SetSavedMessagesManuParams();

            SetPageHeight();
        }

        public void SetSavedMessagesManuParams()
        {
            SavedChatMenuControl.SetChatParam(_chat);
        }

        public void SetPageHeight()
        {
            const int adder = 30;
            Height = SavedChatMenuControl.GetHeightOfControl() + adder;
        }

    }
}
