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

namespace TelegramVisualPart.UserControls.ChatsControls.ToSendMedias
{
    /// <summary>
    /// Логика взаимодействия для MediaElBoxItem.xaml
    /// </summary>
    public partial class MediaElBoxItem : UserControl
    {
        public event Action ChangeMedia;
        public event Action DeleteMedia;

        private string _mediaPath;
        public MediaElBoxItem(string path)
        {
            _mediaPath = path;
            InitializeComponent();

            SetBaseParams();
        }

        public void SetBaseParams()
        {
            if (FilesAction.IsFileIsImage(_mediaPath))
            {
                SetImage(_mediaPath);
            }
        }

        public void SetImage(string path)
        {
            //path == Full file path
            Img.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            Img.Tag = path;
        }

        private void GridBut_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void GridBut_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void ChangeImage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangeMedia?.Invoke();
        }

        private void DeleteMedia_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DeleteMedia?.Invoke();
        }
    }
}
