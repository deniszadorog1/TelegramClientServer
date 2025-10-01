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

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для TextMessage.xaml
    /// </summary>
    public partial class TextMessage : UserControl
    {
        private string _text;
        private string _imgName;

        public TextMessage(string text, string senderImageName, string fontName)
        {
            _text = text;
            _imgName = senderImageName;

            InitializeComponent();

            //BgBrush.ImageSource = img.Source;
            Message.Text = text;
            SetWidth(fontName);

            SetImageSource();
            SetFont(fontName);
        }

        private void SetFont(string font)
        {
            if (font == string.Empty) font = "Times New Roman";
            Message.FontFamily = new FontFamily(font);
        }

        private void SetImageSource()
        {
            if(_imgName is null)
            {
                BgBrush.ImageSource = BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetSystemImagePath("StopSign.png"), UriKind.Absolute));
                return;
            }

            BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_imgName), UriKind.Absolute));
        }

        private const int _minMessageWidth = 30;
        private void SetWidth(string fontName)
        {
            double blockSize = GetStringWidth(fontFamily: fontName) + 10;

            if (blockSize < _minMessageWidth) blockSize = _minMessageWidth;

            Width = blockSize + ImageColumnSize.Width.Value + Message.FontSize;
            SetTime();
            //Height = 50;
        }

        private void SetTime()
        {
            DateTime time = DateTime.Now;
            SentTime.Text = $"{time.Hour}:{time.Minute}";
        }

        public void SetTime(DateTime time)
        {
            SentTime.Text = $"{time.Hour}:{time.Minute}";
        }

        public double GetStringWidth(string fontFamily = "Segoe UI")
        {
            var typeface = new Typeface(new FontFamily(fontFamily), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

            var formattedText = new FormattedText(
                _text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                Message.FontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip
            );
            return formattedText.Width;
        }
    }
}
