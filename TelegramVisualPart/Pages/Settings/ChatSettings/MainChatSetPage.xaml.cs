using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
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
using TelegramLib.Enums.Settings.ChatSettings;
using TelegramLib.MainClasses;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls;

namespace TelegramVisualPart.Pages.Settings.ChatSettings
{
    /// <summary>
    /// Логика взаимодействия для MainChatSetPage.xaml
    /// </summary>
    public partial class MainChatSetPage : Page
    {
        private TelSystem _system;
        private TelegramLib.UserSettings.SettingsTypes.ChatSettings _chatsSettings;

        public MainChatSetPage(TelSystem system)
        {
            _system = system;
            _chatsSettings = _system.Settings.GetChatSettings();

            InitializeComponent();

            SetBasicBlocks();

            //PaletteBut.BgBorder.Background = Brushes.Aqua;
        }

        private void SetBasicBlocks()
        {
            SetIconsParams();
            SetColorCards();

            SetCircleColors();
            SetButsParams();

            SetCheckEventForThemesCards();

            SetClassParams();

            SetChatBgLittleImage();
        }

        public void SetChatBgLittleImage()
        {
            if (_chatsSettings.Wallpaper.WallpaperName == string.Empty) return;
            ChosenWallpaperImage.Source = new BitmapImage(new Uri(TestThing.GetTestParams.GetWallpaperPath(_chatsSettings.Wallpaper.WallpaperName), UriKind.Absolute));

            ChosenWallpaperImage.Effect = _chatsSettings.Wallpaper.IsBlurred ? new BlurEffect() { Radius = 20 } : null;
        }

        public void SetClassParams()
        {
            SetThemeParam();
            SetColorParam();

            SetAutoNightBut();
            SetFontParam();

            SetChatWallpaperParam();
            SendMessageParam();
        }

        public void SetColorParam()
        {
            SolidColorBrush setColor = new SolidColorBrush(Color.FromRgb(
                _chatsSettings.ChosenColor.R, _chatsSettings.ChosenColor.G, _chatsSettings.ChosenColor.B));

            CircleColor chosenOne = ColorCirclesPanel.Children.OfType<CircleColor>().Where(x => CompareColors(x, setColor)).FirstOrDefault();
            if (chosenOne is null) 
            {
                chosenOne = SetCustomColor();
            };

            CircleColor_MouseDown(chosenOne, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = UIElement.MouseDownEvent,
                Source = chosenOne
            });

        }

        private CircleColor SetCustomColor()
        {
            CircleColor last = ColorCirclesPanel.Children.OfType<CircleColor>().LastOrDefault();

            SolidColorBrush newColor = new SolidColorBrush(
                Color.FromRgb(
                    _chatsSettings.ChosenColor.R,
                    _chatsSettings.ChosenColor.G,
                    _chatsSettings.ChosenColor.B));

            last.BgBorder.Background = newColor;
            return last;
        }

        private bool CompareColors(CircleColor one, SolidColorBrush two)
        {
            return one.BgBorder.Background is SolidColorBrush brush &&
                brush.Color == two.Color;
        }

        public void SetAutoNightBut()
        {
            AutoNightBut.ChosenType.Text = _chatsSettings.NightMode.ToString();
        }

        public void SetChatWallpaperParam()
        {
            ChosenWallpaperImage.Source = new Image
            {
                Width = 200,
                Height = 200,
                Source = new BitmapImage(new Uri(FilesAction.GetWallpaperPathByName(_chatsSettings.GetWallpaperName()), UriKind.Absolute)),
                Stretch = Stretch.UniformToFill
            }.Source;
        }

        public void SendMessageParam()
        {
            if (_chatsSettings.IsSendWithEnter)
            {
                SendEnterRadio.IsChecked = true;
                return;
            }
            SendCtrlEnterRadio.IsChecked = true;
        }

        public void SetFontParam()
        {
            FontFamalyBut.ChosenType.Text = _chatsSettings.FontName;
        }

        public void SetThemeParam()
        {
            ThemeCard card = ThemesWrap.Children.OfType<ThemeCard>().Where(x => x.Name ==
            _chatsSettings.Theme.ToString()).FirstOrDefault();

            if (card is null) return;
            card.RadioBut.IsChecked = true;
        }

        public void SetCheckEventForThemesCards()
        {
            Classic.RadioBut.Checked += Theme_Checked;
            Day.RadioBut.Checked += Theme_Checked;
            Tinted.RadioBut.Checked += Theme_Checked;
            Night.RadioBut.Checked += Theme_Checked;
        }

        public void Theme_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton but = sender as RadioButton;

            UnCheckRadios(Classic.RadioBut, but);
            UnCheckRadios(Day.RadioBut, but);
            UnCheckRadios(Tinted.RadioBut, but);
            UnCheckRadios(Night.RadioBut, but);
        }

        public void UnCheckRadios(RadioButton toUncheck, RadioButton chosen)
        {
            if (toUncheck == chosen)
            {
                _chatsSettings.Theme = GetChosenType(chosen);

                ApiService.UpdateChatSettings(_chatsSettings);
                return;
            }
            toUncheck.IsChecked = false;
        }

        public ThemeType GetChosenType(RadioButton toCheck)
        {
            ThemeCard card = HelperService.FindParent<ThemeCard>(toCheck);
            if (card is null) return ThemeType.Tinted;
            for (int i = 0; i <= (int)ThemeType.Night; i++)
            {
                if (((ThemeType)i).ToString() == card.CardName.Text)
                    return (ThemeType)i;
            }
            return ThemeType.Tinted;
        }

        public void SetButsParams()
        {
            AutoNightBut.IconTest.Kind = PackIconKind.ShieldMoonOutline;
            AutoNightBut.ButName.Text = "Auto-night mode";
            AutoNightBut.PreviewMouseDown += AutoNight_PreviewMouseDown;

            FontFamalyBut.IconTest.Kind = PackIconKind.FormatFont;
            FontFamalyBut.ButName.Text = "Font family";
            FontFamalyBut.PreviewMouseDown += FontFamily_PreviewMouseDown;
        }

        public void AutoNight_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            //Set Auto night type
        }

        public void FontFamily_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new ChooseFontFamily(_chatsSettings));
        }

        public void SetCircleColors()
        {
            LightGreen.BgBorder.Background =
                (SolidColorBrush)Application.Current.Resources["PalleteCircleOneColor"];

            Blue.BgBorder.Background =
                (SolidColorBrush)Application.Current.Resources["PalleteCircleTwoColor"];

            Green.BgBorder.Background =
                (SolidColorBrush)Application.Current.Resources["PalleteCircleThreeColor"];

            Pink.BgBorder.Background =
                (SolidColorBrush)Application.Current.Resources["PalleteCircleFourColor"];

            Orange.BgBorder.Background =
                (SolidColorBrush)Application.Current.Resources["PalleteCircleFiveColor"];

            Purple.BgBorder.Background =
                (SolidColorBrush)Application.Current.Resources["PalleteCircleSixColor"];

            Red.BgBorder.Background =
                (SolidColorBrush)Application.Current.Resources["PalleteCircleSevenColor"];

            Gray.BgBorder.Background =
                (SolidColorBrush)Application.Current.Resources["PalleteCircleEightColor"];

            Yellow.BgBorder.Background =
                (SolidColorBrush)Application.Current.Resources["PalleteCircleNineColor"];
        }

        private void SetColorCards()
        {
            Classic.CardBg.Background = (SolidColorBrush)Application.Current.Resources["ColorCardClassic"];
            Classic.CardName.Text = "Classic";

            Day.CardBg.Background = (SolidColorBrush)Application.Current.Resources["ColorCardDay"];
            Day.CardName.Text = "Day";

            Tinted.CardBg.Background = (SolidColorBrush)Application.Current.Resources["ColorCardTinted"];
            Tinted.CardName.Text = "Tinted";

            Night.CardBg.Background = (SolidColorBrush)Application.Current.Resources["ColorCardNight"];
            Night.CardName.Text = "Night";
        }

        private const int _iconSize = 30;
        private void SetIconsParams()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;

            BackBut.Width = _iconSize;
            BackBut.Height = _iconSize;
            CloseBut.Width = _iconSize;
            CloseBut.Width = _iconSize;
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SettingsPage(_system));

        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void CircleColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HideChosenColorCircle();

            if (sender is CircleColor color)
            {
                color.WhiteCircle.Visibility = Visibility.Visible;

                SetNewTempColor(color);
                SaveChosenColor(color);

                MainWindow wind = ((MainWindow)Window.GetWindow(this));
                if (wind is not null) wind.UpdateChatSettingsPage();
            }
        }

        private void SaveChosenColor(CircleColor color)
        {
            SolidColorBrush bg = color.BgBorder.Background as SolidColorBrush;
            if (bg is null) return;

           _chatsSettings.ChosenColor = new TelegramLib.Helpers.ColorHelper
                (_chatsSettings.ChosenColor.Id ,bg.Color.R, bg.Color.G, bg.Color.B);
        }

        public void SetNewTempColor(CircleColor color)
        {
            Application.Current.Resources["TempActiveTextColor"] =
                color.BgBorder.Background as SolidColorBrush;
        }

        public void HideChosenColorCircle()
        {
            for (int i = 0; i < ColorCirclesPanel.Children.Count; i++)
            {
                if (ColorCirclesPanel.Children[i] is CircleColor circle)
                {
                    circle.WhiteCircle.Visibility = Visibility.Hidden;
                }
            }
        }

        private void PaletteBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new ChatSetPalette(_chatsSettings));
        }

        private void ChatWallpaperTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock text)
            {
                text.TextDecorations = TextDecorations.Underline;
            }
        }

        private void ChatWallpaperTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock text)
            {
                text.TextDecorations = null;
            }
        }

        private void SendEnterRadio_Checked(object sender, RoutedEventArgs e)
        {
            _chatsSettings.IsSendWithEnter = true;
            ApiService.UpdateChatSettings(_chatsSettings);
        }

        private void SendCtrlEnterRadio_Checked(object sender, RoutedEventArgs e)
        {
            _chatsSettings.IsSendWithEnter = false;
            ApiService.UpdateChatSettings(_chatsSettings);
        }

        private void ChooseWallpaperFromGalery_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SetChatWallpaper(_system.Settings.GetChatSettings()));
        }

        private void ChooseWallpaperFromFile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void AutoNightBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _chatsSettings.NightMode = _chatsSettings.NightMode == AutoNightMode.Off ?
                AutoNightMode.System : AutoNightMode.Off;

            AutoNightBut.ChosenType.Text = _chatsSettings.NightMode.ToString();

            ApiService.UpdateChatSettings(_chatsSettings);
        }
    }
}
