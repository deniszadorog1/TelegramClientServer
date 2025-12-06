using Accessibility;
using FFMpegCore.Enums;
using MaterialDesignThemes.Wpf;
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
using TelegramLib.MainClasses;

namespace TelegramVisualPart.UserControls.ChatControls.SavedChatControls
{
    /// <summary>
    /// Логика взаимодействия для SavedChatMenu.xaml
    /// </summary>
    public partial class SavedChatMenu : UserControl
    {
        private SavedMessagesChat _chat;

        public event Action ControlLoaded;
        public event Action CloseControl;
        
        public SavedChatMenu()
        {
            InitializeComponent();
        }

        public void SetChatParam(SavedMessagesChat chat)
        {
            _chat = chat;

            SetBasicParams();

            SetHeightParams();
        }

        public void SetHeightParams()
        {
            double newHeight = PhotoMedia.Height + VideoMedia.Height + 
                LinkMedia.Height + GifMedia.Height;

            MediaTypesRow.Height = new GridLength(newHeight);
        }
        
        public void SetBasicParams()
        {
            //Set amount + clcks on this
            PhotoMedia.SetControlParams(PackIconKind.BrokenImage, 0, "photos");
            VideoMedia.SetControlParams(PackIconKind.VideoOutline, 0, "videos");
            LinkMedia.SetControlParams(PackIconKind.Link, 0, "shared links");
            GifMedia.SetControlParams(PackIconKind.Update, 0, "GIFs");

            SetMediaParamsStackHeight();
        }

        public void SetMediaParamsStackHeight()
        {
            PhotoMedia.SetValues(Enums.SentItemsTypes.Photos, _chat);
            VideoMedia.SetValues(Enums.SentItemsTypes.Video, _chat);
            LinkMedia.SetValues(Enums.SentItemsTypes.SharedLinks, _chat);
            GifMedia.SetValues(Enums.SentItemsTypes.GIFs, _chat);
        }


        private void CloseButtonGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            CloseIcon.Foreground = new SolidColorBrush(Colors.White);
        }

        private void CloseButtonGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            CloseIcon.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void CloseButtonGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void SentMedia_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_chat is null || sender is not MediaParam mediaPar) return;
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.
                SentObjectsUserInfo.SentItemsUserContact(
                ((MainWindow)Window.GetWindow(this)).GetSystem(),
                GetItemType(mediaPar.Name), _chat));
        }

        private Enums.SentItemsTypes GetItemType(string name)
        {
            return name == PhotoMedia.Name.ToString() ? Enums.SentItemsTypes.Photos :
                name == VideoMedia.Name.ToString() ? Enums.SentItemsTypes.Video :
                name == LinkMedia.Name.ToString() ? Enums.SentItemsTypes.GIFs :
                name == GifMedia.Name.ToString() ? Enums.SentItemsTypes.SharedLinks :
                Enums.SentItemsTypes.Photos;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ControlLoaded?.Invoke();
        }

        public double GetHeightOfControl()
        {
            double res = 0;
            
            res += UpperRow.Height.Value;
            res += GetMediaParamsHeight();

            res += DividerRow.Height.Value;
            res += GetMessagesHeight();

            return res;
        }

        public double GetMediaParamsHeight()
        {
            return PhotoMedia.Height +
                VideoMedia.Height +
                LinkMedia.Height +
                GifMedia.Height;
        }

        public double GetMessagesHeight()
        {
            double res = 0;

            for (int i = 0; i < ChatsMessagesListBox.Items.Count; i++)
            {
                if (ChatsMessagesListBox.Items[i] is not Control control)
                {

                }
            }

            return res;
        }
    }
}
