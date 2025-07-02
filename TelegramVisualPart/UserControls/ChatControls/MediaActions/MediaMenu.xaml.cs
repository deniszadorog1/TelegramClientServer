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

namespace TelegramVisualPart.UserControls.ChatControls.MediaActions
{
    /// <summary>
    /// Логика взаимодействия для MediaMenu.xaml
    /// </summary>
    public partial class MediaMenu : UserControl
    {
        public MediaMenu()
        {
            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            GoToMessage.Icon.Kind = PackIconKind.EyeOutline;
            GoToMessage.ButText.Text = "Go To Message";

            ShowInFolder.Icon.Kind = PackIconKind.FolderOutline;
            ShowInFolder.ButText.Text = "Show in Folder";

            CopyFrame.Icon.Kind = PackIconKind.ContentCopy;
            CopyFrame.ButText.Text = "Copy Frame";

            Forward.Icon.Kind = PackIconKind.Forward;
            Forward.ButText.Text = "Forward";

            Delete.Icon.Kind = PackIconKind.DeleteForeverOutline;
            Delete.ButText.Text = "Delete";

            SaveAs.Icon.Kind = PackIconKind.ContentSaveOutline;
            SaveAs.ButText.Text = "Save As...";
        }

        private void GoToMessage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ShowInFolder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CopyFrame_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Forward_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Delete_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SaveAs_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
