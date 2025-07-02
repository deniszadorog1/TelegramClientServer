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

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для ImageMessage.xaml
    /// </summary>
    public partial class MediaMessage : UserControl
    {
        public Image _img;
        public MediaElement _media;
        
        public MediaMessage(Image img)
        {
            _img = img;
            InitializeComponent();
            ImgMessage.ImageSource = _img.Source;
            VideoBorder.Visibility = Visibility.Hidden;
        }

        public MediaMessage(MediaElement media)
        {
            _media = media;
            InitializeComponent();

            ImageBorder.Visibility = Visibility.Hidden;
        }

        public MediaElement GetVideo()
        {
            return _media;
        }

        public Image GetImage()
        {
            return _img;
        }
    }
}
