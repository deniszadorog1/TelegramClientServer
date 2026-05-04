using System;
using System.Collections.Generic;
using System.IO;
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
using TelegramVisualPart.Services;

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

        public MediaElBoxItem()
        {
            InitializeComponent();
        }

        public MediaElBoxItem(string path)
        {
            _mediaPath = path;
            InitializeComponent();

            SetBaseParams();
        }

        public async ValueTask SetBaseParams()
        {
            if (FilesAction.IsFileIsImage(_mediaPath))
            {
                SetImage(_mediaPath);
            }
            if (FilesAction.IsFileIsVideo(_mediaPath))
            {
                Image img = await VisHelper.GetFirstFrameAsync(_mediaPath);
                if (img is null) return;

                Img.Source = img.Source;
                Img.Tag = _mediaPath;
            }
        }

        public async void SetImage(string path)
        {
            if (FilesAction.IsFileIsVideo(path))
            {
                Image img = await VisHelper.GetFirstFrameAsync(path);
                if (img is null) return;

                Img.Source = img.Source;
                Img.Tag = path;
                return;
            }

            BitmapImage cachedBitmap = ApiService.GetCachedBitmap(path);

            Img.Source = cachedBitmap is not null ? cachedBitmap : SignalRHelperService.LoadBitmap(path);// new BitmapImage(new Uri(path, UriKind.Absolute));
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

        private const int _height = 170;
        private const int _width = 285;
        public void SetChosenSize()
        {
            this.Height = _height;
            this.Width = _width;
        }

        public void ClearParams()
        {
            Img.Source = null;
        }

        public string GetMediaPath()
        {
            return _mediaPath;
        }
    }
}
