using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramLib.Models;

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
            Height = SavedChatMenuControl.GetHeightOfControl() + 30;
        }
        
    }
}
