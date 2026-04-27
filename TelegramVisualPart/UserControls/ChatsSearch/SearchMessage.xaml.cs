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
using TelegramVisualPart.Helper;

namespace TelegramVisualPart.UserControls.ChatsSearch
{
    /// <summary>
    /// Логика взаимодействия для SearchMessage.xaml
    /// </summary>
    public partial class SearchMessage : UserControl
    {
        public SearchMessage()
        {
            try
            {
                InitializeComponent();
                CloseBut.IconType.Kind = PackIconKind.Close; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Minstake in SearchMessage: {ex.Message}");
                throw;
            }
        }

        public void SetUserImage(string userImageName)
        {
            ImgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(userImageName), UriKind.Absolute));
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(this.Parent is Grid) ((Grid)this.Parent).Visibility = Visibility.Hidden;
            ((MainWindow)Window.GetWindow(this)).SetChatsMessages();
        }
    }
}
