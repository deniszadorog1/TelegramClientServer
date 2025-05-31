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
    /// Логика взаимодействия для TextMessage.xaml
    /// </summary>
    public partial class TextMessage : UserControl
    {
        private string _text;
        public TextMessage(string text, Image img)
        {
            _text = text;

            InitializeComponent();

            //BgBrush.ImageSource = img.Source;
            Message.Text = text;
            SetWidth();
        }

        private void SetWidth()
        {
            double blockSize = GetStringWidth();

            Width = blockSize + ImageColumnSize.Width.Value + Message.FontSize;
            SetTime();
            //Height = 50;
        }

        private void SetTime()
        {
            DateTime time = DateTime.Now;
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
