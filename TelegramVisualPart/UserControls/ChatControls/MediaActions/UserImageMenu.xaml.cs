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
    /// Логика взаимодействия для UserImageMenu.xaml
    /// </summary>
    public partial class UserImageMenu : UserControl
    {
        public UserImageMenu()
        {
            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            Copy.Icon.Kind = PackIconKind.ContentCopy;
            Copy.ButText.Text = "Copy";

            Delete.Icon.Kind = PackIconKind.GarbageCanOutline;
            Delete.ButText.Text = "Delete";

            SaveAs.Icon.Kind = PackIconKind.ContentSaveOutline;
            SaveAs.ButText.Text = "Save As...";

            WatchInFiles.Icon.Kind = PackIconKind.FolderOutline;
            WatchInFiles.ButText.Text = "Show in file";

            Report.Icon.Kind = PackIconKind.InfoBoxOutline;
            Report.ButText.Text = "Report";
        }
    }
}
