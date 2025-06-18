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
using TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages;
using TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls;

namespace TelegramVisualPart.Pages.Settings.ChatSettings
{
    /// <summary>
    /// Логика взаимодействия для MainChatSetPage.xaml
    /// </summary>
    public partial class MainChatSetPage : Page
    {
        public MainChatSetPage()
        {
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
        }

        public void SetCheckEventForThemesCards()
        {
            ClassicCard.RadioBut.Checked += Theme_Checked;
            DayCard.RadioBut.Checked += Theme_Checked;
            TintedCard.RadioBut.Checked += Theme_Checked;
            NightCard.RadioBut.Checked += Theme_Checked;
        }

        public void Theme_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton but = sender as RadioButton;

            UnCheckRadios(ClassicCard.RadioBut, but);
            UnCheckRadios(DayCard.RadioBut, but);
            UnCheckRadios(TintedCard.RadioBut, but);
            UnCheckRadios(NightCard.RadioBut, but);

        }

        public void UnCheckRadios(RadioButton toUncheck, RadioButton chosen)
        {
            if (toUncheck == chosen) return;
            toUncheck.IsChecked = false;
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
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new ChooseFontFamily());
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
            ClassicCard.CardBg.Background = (SolidColorBrush)Application.Current.Resources["ColorCardClassic"];
            ClassicCard.CardName.Text = "Classic";

            DayCard.CardBg.Background = (SolidColorBrush)Application.Current.Resources["ColorCardDay"];
            DayCard.CardName.Text = "Day";

            TintedCard.CardBg.Background = (SolidColorBrush)Application.Current.Resources["ColorCardTinted"];
            TintedCard.CardName.Text = "Tinted";

            NightCard.CardBg.Background = (SolidColorBrush)Application.Current.Resources["ColorCardNight"];
            NightCard.CardName.Text = "Night";

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
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SettingsPage());

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
            }
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
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new ChatSetPalette());
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

        }

        private void SendCtrlEnterRadio_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ChooseWallpaperFromGalery_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ChooseWallpaperFromFile_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
