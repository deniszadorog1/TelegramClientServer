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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramVisualPart.Services;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages
{
    /// <summary>
    /// Логика взаимодействия для WallpaperPreview.xaml
    /// </summary>
    public partial class WallpaperPreview : Page
    {
        private TelegramLib.UserSettings.SettingsTypes.ChatSettings _settings;
        private Image _img;
        private ChatBackground _chatBackground;
        private UserChat _chat;

        public WallpaperPreview(TelegramLib.UserSettings.SettingsTypes.ChatSettings settings,
            Image? bgImage)
        {
            _settings = settings;

            InitializeComponent();
            SetBasicParams();

            if (bgImage is not null)
            {
                BgImage.Source = bgImage.Source; 
                _img = bgImage;
            }
        }

        public WallpaperPreview(ChatBackground background, Image? img, UserChat chat)
        {
            _chatBackground = background;
            _chat = chat;

            InitializeComponent();
            SetBasicParams();

            if (img is not null)
            {
                BgImage.Source = img.Source;
                _img = img;
            }
        }

        public void SetBasicParams()
        {
            ChangeLightState.IconType.Kind = PackIconKind.WeatherSunny;

            Share.TextBlock.Text = "Share";
            Cancel.TextBlock.Text = "Cancel";
            Apply.TextBlock.Text = "Apply";
        }

        private void BlurGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void BlurGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void BlurGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsBlurCheckBox.IsChecked = !IsBlurCheckBox.IsChecked;

            if (ImageGrid.Effect is not null)
            {
                ImageGrid.Effect = null;
                return;
            }

            ImageGrid.Effect = new BlurEffect()
            {
                Radius = 20
            };
        }

        private void Cancel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private async void Apply_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Apply new bg Image for allChats //mb Grid
            if (_settings is not null)
            {
                _settings.Wallpaper.SetBlurParam(ImageGrid.Effect is not null);
                _settings.Wallpaper.WallpaperName = TestThing.GetTestParams.GetWallpaperPath(_img.Tag.ToString());

                _settings.Wallpaper.Id = 
                    await ApiService.GetChatBgIdByName(_settings.Wallpaper.WallpaperName);

                //Set Settings wallpaper indb 
                await ApiService.UpdateChatSettings(_settings);
            }
            else if(_chatBackground is not null)
            {
                _chatBackground.SetBlurState(ImageGrid.Effect is not null);
                _chatBackground.SetPath(TestThing.GetTestParams.GetWallpaperPath(_img.Tag.ToString()));
                _chatBackground.SetIsGeneral(false);

                //Set chat wallpaper in db

                await ApiService.SetChatWallpaper(_chatBackground, _chat.Id);
            }

            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();

            //Set user chat bg
            ((MainWindow)Window.GetWindow(this)).SetChatBg();
        }

        private void Share_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set Share option IDK what it does
        }

        private void ChangeLightState_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            InvertColors();
        }

        private const PackIconKind _moonIconKind = PackIconKind.MoonAndStars;

        public void InvertColors()
        {
            ChangeLightState.IconType.Kind = ChangeLightState.IconType.Kind == _moonIconKind ?
                PackIconKind.WeatherSunny : _moonIconKind;

            if (ChangeLightState.IconType.Kind == _moonIconKind)
            {
                SetLightState();
                return;
            }
            SetDarkState();
        }

        private readonly SolidColorBrush _whiteStateBgColor =
            new SolidColorBrush(Colors.White);

        private readonly SolidColorBrush _whiteStateTextBlockColor =
                new SolidColorBrush(Colors.Black);

        private readonly SolidColorBrush _whiteStateButTextColor =
                new SolidColorBrush(Colors.Blue);

        public void SetLightState()
        {
            BorderBg.Background = _whiteStateBgColor;
            FirstMessageBorder.Background = _whiteStateBgColor;
            SecondMessageBorder.Background = _whiteStateBgColor;

            PageName.Foreground = _whiteStateTextBlockColor;
            FirstMessageTextBlock.Foreground = _whiteStateTextBlockColor;
            SecondMessageTextBlock.Foreground = _whiteStateTextBlockColor;

            Share.TextBlock.Foreground = _whiteStateButTextColor;
            Cancel.TextBlock.Foreground = _whiteStateButTextColor;
            Apply.TextBlock.Foreground = _whiteStateButTextColor;
        }

        private readonly SolidColorBrush _darkStateTextBlockColor = new SolidColorBrush(Colors.White);

        public void SetDarkState()
        {
            SolidColorBrush darkBg = (SolidColorBrush)System.Windows.Application.Current.Resources["DarkThemeOne"];
            SolidColorBrush activeColor = new SolidColorBrush(Colors.White);

            BorderBg.Background = darkBg;
            FirstMessageBorder.Background = darkBg;
            SecondMessageBorder.Background = darkBg;

            PageName.Foreground = _darkStateTextBlockColor;
            FirstMessageTextBlock.Foreground = _darkStateTextBlockColor;
            SecondMessageTextBlock.Foreground = _darkStateTextBlockColor;

            Share.TextBlock.Foreground = activeColor;
            Cancel.TextBlock.Foreground = activeColor;
            Apply.TextBlock.Foreground = activeColor;
            ChangeLightState.IconType.Kind = PackIconKind.WeatherSunny;
        }
    }
}
