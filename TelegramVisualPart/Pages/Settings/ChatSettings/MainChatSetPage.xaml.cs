using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Win32;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.RightsManagement;
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
using System.Windows.Threading;
using TelegramLib.Enums.Settings.ChatSettings;
using TelegramLib.MainClasses;
using TelegramLib.Models;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls;
using TelegramVisualPart.UserControls.SettingsControls.FoldersPrivacy;
using Color = System.Windows.Media.Color;
using Path = System.IO.Path;

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

            CircleColor? chosenOne = ColorCirclesPanel.Children.OfType<CircleColor>().Where(x => CompareColors(x, setColor)).FirstOrDefault();
            if (chosenOne is null)
            {
                chosenOne = SetCustomColor();
            };

            ActivateClickCircleColorByCircle(chosenOne);
        }

        public void ActivateClickCircleColorByCircle(CircleColor chosenOne)
        {
            CircleColor_MouseDown(chosenOne, new MouseButtonEventArgs(
                Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = UIElement.MouseDownEvent,
                Source = chosenOne
            });
        }

        private CircleColor SetCustomColor()
        {
            CircleColor? last = ColorCirclesPanel.Children.OfType<CircleColor>().LastOrDefault();

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
            ChosenWallpaperImage.Source = new System.Windows.Controls.Image
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
            ThemeCard? card = ThemesWrap.Children.OfType<ThemeCard>().FirstOrDefault(x => x.Name ==
            _chatsSettings.Theme.ToString());

            if (card is null) return;
            card.RadioBut.IsChecked = true;

            //Set temp theme params (Color)
            SetThemesBaseParams();

            //Set events for themes
            SetThemeEvents();
        }

        public void SetThemeEvents()
        {
            List<ThemeCard> cards = ThemesWrap.Children.OfType<ThemeCard>().ToList();

            for (int i = 0; i < cards.Count; i++)
            {
                ThemeCard card = cards[i];
                card.RadioBut.Checked += (sender, e) =>
                {
                    int.TryParse(card.Tag.ToString(), out int tagThemeId);

                    //1 - Get specific theme (from system)
                    TelegramLib.MainClasses.ChatFitures.Theme? theme =
                        _system.Settings.ChatsSettings.Themes.FirstOrDefault(x => x.Id == tagThemeId);
                    if (theme is null) return;

                    //1.1 Change theme type
                    _system.Settings.ChatsSettings.Theme = theme.Type;

                    //2 - Set active text color 
                    SetActiveTextColor(theme);

                    //3 - set additional(inactive) text color(white or black)
                    SetAdditionalTextColor(theme);

                    UpdateBgColors(theme);

                    //5 - set right circle color (if not exist =>
                    //set color in last circle and choose it) + 
                    //+ Update Page with new colors
                    SetCircleColorByTheme(theme);

                    ((MainWindow)Window.GetWindow(this)).SetMainFrame(new MainChatPage(_system));
                    ((MainWindow)Window.GetWindow(this)).UpdateUpperBorder();
                };
            }
        }

        public void UpdateBgColors(TelegramLib.MainClasses.ChatFitures.Theme theme)
        {
            //Night - my basic
            //Tinted - colored
            //Day and Classic - White and THE SAME

            Application.Current.Resources["DarkThemeOne"] =
                theme.Type == ThemeType.Night ? new SolidColorBrush(Color.FromRgb(23, 33, 43)) : //Basic Dark 
                theme.Type == ThemeType.Tinted ? new SolidColorBrush(
                    Color.FromRgb(GetColorParam(theme.Color.R, 5),
                    GetColorParam(theme.Color.G, 5),
                    GetColorParam(theme.Color.B, 5))) :
                new SolidColorBrush(Colors.White);

            Application.Current.Resources["DarkThemeMouseEnterBut"] =
                theme.Type == ThemeType.Night ? new SolidColorBrush(Color.FromRgb(35, 46, 60)) : //Basic Dark 
                theme.Type == ThemeType.Tinted ? new SolidColorBrush(
                    Color.FromRgb(GetColorParam(theme.Color.R, 3),
                    GetColorParam(theme.Color.G, 3),
                    GetColorParam(theme.Color.B, 3))) :
                new SolidColorBrush(Color.FromRgb(222, 222, 222));

            Application.Current.Resources["DarkThemeDeviderField"] =
                theme.Type == ThemeType.Night ? new SolidColorBrush(Color.FromRgb(35, 45, 59)) : //Basic Dark 
                theme.Type == ThemeType.Tinted ? new SolidColorBrush(
                    Color.FromRgb(GetColorParam(theme.Color.R, 3),
                    GetColorParam(theme.Color.G, 3),
                    GetColorParam(theme.Color.B, 3))) :
                new SolidColorBrush(Color.FromRgb(222, 222, 222));

            Application.Current.Resources["DarkThemeSecond"] =
                theme.Type == ThemeType.Night ? new SolidColorBrush(Color.FromRgb(14, 22, 33)) : //Basic Dark 
                theme.Type == ThemeType.Tinted ? new SolidColorBrush(
                    Color.FromRgb(GetColorParam(theme.Color.R, 8),
                    GetColorParam(theme.Color.G, 8),
                    GetColorParam(theme.Color.B, 8))) :
                new SolidColorBrush(
                    Color.FromRgb(GetColorParam(theme.Color.R, 8),
                    GetColorParam(theme.Color.G, 8),
                    GetColorParam(theme.Color.B, 8)));

            Application.Current.Resources["UpperBangColor"] =
                theme.Type == ThemeType.Night ? new SolidColorBrush(Color.FromRgb(58, 64, 71)) : //Basic Dark 
                theme.Type == ThemeType.Tinted ? new SolidColorBrush(
                    Color.FromRgb(GetColorParam(theme.Color.R, 3),
                    GetColorParam(theme.Color.G, 3),
                    GetColorParam(theme.Color.B, 3))) :
                new SolidColorBrush(
                    Color.FromRgb(GetColorParam(theme.Color.R, 3),
                    GetColorParam(theme.Color.G, 3),
                    GetColorParam(theme.Color.B, 3)));

            //UpperBangColor


            Application.Current.Resources["DarkThemeProfileButEnter"] =
                theme.Type == ThemeType.Night ? new SolidColorBrush(Color.FromRgb(29, 42, 57)) : //Basic Dark 
                theme.Type == ThemeType.Tinted ? new SolidColorBrush(
                    Color.FromRgb(GetColorParam(theme.Color.R, 4),
                    GetColorParam(theme.Color.G, 4),
                    GetColorParam(theme.Color.B, 4))) :
                new SolidColorBrush(
                    Color.FromRgb(GetColorParam(theme.Color.R, 4),
                    GetColorParam(theme.Color.G, 4),
                    GetColorParam(theme.Color.B, 4)));
        }
        private byte GetColorParam(byte tempColor, int toAdd)
        {
            int value = tempColor / toAdd;
            return (byte)(value > 255 ? 255 : value < 0 ? 0 : value);
        }

        public void SetAdditionalTextColor(TelegramLib.MainClasses.ChatFitures.Theme theme)
        {
            Application.Current.Resources["UsualTextColor"] =
                _system.Settings.ChatsSettings.Theme == ThemeType.Classic ||
                _system.Settings.ChatsSettings.Theme == ThemeType.Day
                ? new SolidColorBrush(Colors.Black) :
                 new SolidColorBrush(Colors.White);
        }

        public void SetActiveTextColor(TelegramLib.MainClasses.ChatFitures.Theme theme)
        {
            Application.Current.Resources["TempActiveTextColor"] =
                new SolidColorBrush(Color.FromRgb(theme.Color.R, theme.Color.G, theme.Color.B));
        }

        public void SetCircleColorByTheme(TelegramLib.MainClasses.ChatFitures.Theme theme)
        {
            List<CircleColor> circles =
                ColorCirclesPanel.Children.OfType<CircleColor>().ToList();

            for (int i = 0; i < circles.Count; i++)
            {
                SolidColorBrush? brush =
                    circles[i].BgBorder.Background as SolidColorBrush;

                if (brush is null) continue;

                if (brush.Color.R == theme.Color.R &&
                    brush.Color.G == theme.Color.G &&
                    brush.Color.B == theme.Color.B)
                {
                    //found right circle + acivate it
                    ActivateClickCircleColorByCircle(circles[i]);
                    return;
                }
            }

            //Set cutom color circle
            if (circles.Count == 0) return;
            CircleColor last = circles.Last();

            last.BgBorder.Background = new SolidColorBrush(
                Color.FromRgb(theme.Color.R, theme.Color.G, theme.Color.B));

            ActivateClickCircleColorByCircle(last);
        }

        public void SetThemesBaseParams()
        {
            SetBaseThemeParam(Classic,
                _system.Settings.ChatsSettings.Themes.FirstOrDefault(x => x.Type == ThemeType.Classic));

            SetBaseThemeParam(Day,
                _system.Settings.ChatsSettings.Themes.FirstOrDefault(x => x.Type == ThemeType.Day));

            SetBaseThemeParam(Tinted,
                _system.Settings.ChatsSettings.Themes.FirstOrDefault(x => x.Type == ThemeType.Tinted));

            SetBaseThemeParam(Night,
                _system.Settings.ChatsSettings.Themes.FirstOrDefault(x => x.Type == ThemeType.Night));
        }

        public void SetBaseThemeParam(ThemeCard card,
            TelegramLib.MainClasses.ChatFitures.Theme? theme)
        {
            if (card is null || theme is null) return;

            card.Tag = theme.Id;
            card.CardBg.Background = new SolidColorBrush(
                Color.FromRgb(theme.Color.R, theme.Color.G, theme.Color.B));
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
            RadioButton? but = sender as RadioButton;

            if (but is null) return;

            UnCheckRadios(Classic, but);
            UnCheckRadios(Day, but);
            UnCheckRadios(Tinted, but);
            UnCheckRadios(Night, but);
        }

        public void UnCheckRadios(ThemeCard toUncheck, RadioButton chosen)
        {
            if (toUncheck.RadioBut == chosen)
            {
                ThemeType? type =
                   toUncheck == Classic ? ThemeType.Classic :
                   toUncheck == Day ? ThemeType.Day :
                   toUncheck == Tinted ? ThemeType.Tinted :
                   /*toUncheck == Night ? */ThemeType.Night;/*= GetChosenType(chosen);*/

                _chatsSettings.Theme = type is null ? ThemeType.Tinted : (ThemeType)type;

                ApiService.UpdateChatSettings(_chatsSettings);
                return;
            }
            toUncheck.RadioBut.IsChecked = false;
        }

        public ThemeType? GetChosenType(RadioButton toCheck)
        {
            ThemeCard card = HelperService.FindParent<ThemeCard>(toCheck);

            if (card is null) return null;
            for (int i = 0; i <= (int)ThemeType.Night; i++)
            {
                if (((ThemeType)i).ToString() == card.CardName.Text)
                    return (ThemeType)i;
            }
            return null;
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

                Console.WriteLine(_system.Settings.ChatsSettings.Theme);

                MainWindow wind = ((MainWindow)Window.GetWindow(this));
                if (wind is not null)
                {
                    wind.UpdateChatSettingsPage();
                }
                //Set them params
                UpdateThemeParams();
            }
        }
        public void UpdateThemeParams()
        {
            //1 - Get Theme
            TelegramLib.MainClasses.ChatFitures.Theme? theme =
                _system.Settings.ChatsSettings.Themes
                    .FirstOrDefault(x => x.Type == _system.Settings.ChatsSettings.Theme);

            if (theme is null) return;

            //2 - Update Color
            SolidColorBrush brush =
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];

            theme.Color = new TelegramLib.Helpers.ColorHelper(
                -1, brush.Color.R, brush.Color.G, brush.Color.B);


            //3 - Update it in db
        }

        private void SaveChosenColor(CircleColor color)
        {
            SolidColorBrush? bg = color.BgBorder.Background as SolidColorBrush;
            if (bg is null) return;

            _chatsSettings.ChosenColor = new TelegramLib.Helpers.ColorHelper
                 (_chatsSettings.ChosenColor.Id, bg.Color.R, bg.Color.G, bg.Color.B);
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
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(
                new ChatSetPalette(_chatsSettings));
        }

        private void ChatWallpaperTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock text)
            {
                Cursor = Cursors.Hand;
                text.TextDecorations = TextDecorations.Underline;
            }
        }

        private void ChatWallpaperTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock text)
            {
                Cursor = null;
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
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new SetChatWallpaper(_system.Settings.GetChatSettings()));
        }

        private void ChooseWallpaperFromFile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set Add file 
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Choose wallpaper";
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            dlg.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Visuals", "Images");

            if (dlg.ShowDialog() == true)
            {
                //Get file name (is it an image)
                string selectedFile = dlg.FileName;
                string fileName = Path.GetFileName(selectedFile);

                if (!FilesAction.IsFileIsImage(selectedFile)) return;

                //if an image -> add in db
                ApiService.AddWallpaper(fileName);

                //Add in wallpapers folder
                FilesAction.AddNewWallpaper(selectedFile);

                //Add in system
                _system.Settings.ChatsSettings.PossibleWallpapers.Add(fileName);
            }
        }

        private void AutoNightBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _chatsSettings.NightMode = _chatsSettings.NightMode == AutoNightMode.Off ?
                AutoNightMode.System : AutoNightMode.Off;

            AutoNightBut.ChosenType.Text = _chatsSettings.NightMode.ToString();

            ApiService.UpdateChatSettings(_chatsSettings);

            SetAutoNightTimer(_chatsSettings.NightMode);
        }

        private readonly DateTime _startNight = DateTime.Today.AddHours(23);
        private readonly DateTime _endNight = DateTime.Today.AddDays(1).AddHours(6);

        private DispatcherTimer _nightTimer = new DispatcherTimer();
        private void SetAutoNightTimer(AutoNightMode type)
        {
            if (!SetAutoNightModeType(type)) return;

            _nightTimer.Interval = TimeSpan.FromSeconds(10000);
            _nightTimer.Tick += NightTypeTimer_Tick;

            _nightTimer.Start();
        }

        public void NightTypeTimer_Tick(object sender, EventArgs e)
        {
            if (IsNightNow())
            {
                //Set it on start (when is base initioation)
                 
                //Set night theme
                //Get set timer activation
            }
        }

        public bool SetAutoNightModeType(AutoNightMode type)
        {
            if (type == AutoNightMode.Off)
            {
                _nightTimer.Tick -= NightTypeTimer_Tick;
                return false;
            }
            return true;
        }

        private bool IsNightNow()
        {
            DateTime now = DateTime.Now;
            return (now >= _startNight || now < _endNight);
        }


        private void RadioButton_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void RadioButton_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
