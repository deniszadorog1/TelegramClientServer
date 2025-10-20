using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
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
        private TelSystem _system;
        private TelegramLib.MainClasses.Messages.Message? _toReply;

        public TextMessage(TelSystem system,
            string text, string senderImageName,
            string fontName,
            TelegramLib.MainClasses.Messages.Message? toReply = null)
        {
            _system = system;
            _text = text;
            _imgName = senderImageName;
            _toReply = toReply;

            InitializeComponent();

            //BgBrush.ImageSource = img.Source;
            Message.Text = text;
            SetWidth(fontName);

            SetImageSource();
            SetFont(fontName);

            SetMessageReplyControl();
        }

        private void SetMessageReplyControl()
        {
            if (_toReply is null || _system is null)
            {
                ReplyedRow.Height = new GridLength(0);
                //Set null value
                return;
            }
            ReplyedRow.Height = new GridLength(50);
            ReplyControl.SetReplyMessageParams(_system, _toReply);

            ReplyControl.PreviewMouseDown += (sender, e) =>
            {
                if (_toReply is null) return;
                //Set scrolling to message
                ((MainWindow)Window.GetWindow(this))
                .ShowChosenMessageByMessageId(_toReply.Id);
            };
        }

        private void SetFont(string font)
        {
            if (font == string.Empty) font = "Times New Roman";
            Message.FontFamily = new FontFamily(font);
        }

        private void SetImageSource()
        {
            if (_imgName is null)
            {
                BgBrush.ImageSource = BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetSystemImagePath("StopSign.png"), UriKind.Absolute));
                return;
            }

            BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_imgName), UriKind.Absolute));
        }

        private const int _minMessageWidth = 75;
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

        private const int _tickColWidth = 25;
        public void SetTickVis(string iconName)
        {
            TickColumn.Width = new GridLength(_tickColWidth);
            SetVisibility(iconName);
        }

        public void SetVisibility(string iconName)
        {
            ReadIconFlag.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), iconName);
        }

        private const int _basePinWidth = 20;
        public void SetPinColumnState(bool isPinned)
        {
            if (isPinned) PinnIcon.Visibility = Visibility.Visible;
            else PinnIcon.Visibility = Visibility.Hidden;
        }

    }
}
