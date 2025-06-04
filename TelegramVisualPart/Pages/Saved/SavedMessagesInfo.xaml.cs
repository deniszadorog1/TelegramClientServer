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
using TelegramVisualPart.UserControls;

namespace TelegramVisualPart.Pages.Saved
{
    /// <summary>
    /// Логика взаимодействия для SavedMessagesInfo.xaml
    /// </summary>
    public partial class SavedMessagesInfo : Page
    {
        public SavedMessagesInfo()
        {
            InitializeComponent();

            SetIconsView();
        }

        private void PackIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            if(sender is PackIcon icon)
            {
                icon.Foreground = Brushes.White;
                Cursor = Cursors.Hand;
            }
        }

        private void PackIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is PackIcon icon)
            {
                icon.Foreground = Brushes.Gray;
                Cursor = null;
            }
        }

        private void Buts_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set page 
        }

        public void SetIconsView()
        {
            Images.IconType.Kind = PackIconKind.BrokenImage;
            Images.ButName.Text = $"photos";

            Videos.IconType.Kind = PackIconKind.VideoOutline;
            Videos.ButName.Text = $"videos";

            Files.IconType.Kind = PackIconKind.FileOutline;
            Files.ButName.Text = $"files";

            AudioFiles.IconType.Kind = PackIconKind.HeadphonesOutline;
            AudioFiles.ButName.Text = $"audio files";

            SharedLinks.IconType.Kind = PackIconKind.LinkVariant;
            SharedLinks.ButName.Text = $"shared links";

            VoiceMessages.IconType.Kind = PackIconKind.SettingsVoice;
            VoiceMessages.ButName.Text = $"voice messages";

            Gifs.IconType.Kind = PackIconKind.GiftOutline;
            Gifs.ButName.Text = $"GIF";
        }

    }
}
