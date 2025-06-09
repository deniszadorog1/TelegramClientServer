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
using TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls;

namespace TelegramVisualPart.Pages.Settings.ChatSettings
{
    /// <summary>
    /// Логика взаимодействия для MainChatSettingsPage.xaml
    /// </summary>
    public partial class MainChatSettingsPage : UserControl
    {
        public MainChatSettingsPage()
        {
            InitializeComponent();

            SetBasicBlocks();

            PaletteBut.BgBorder.Background = Brushes.Aqua;
        }

        private void SetBasicBlocks()
        {
            SetIconsParams();
            SetColorCards();
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

            if(sender is CircleColor color)
            {
                color.WhiteCircle.Visibility = Visibility.Visible;
            }
        }

        public void HideChosenColorCircle()
        {
            for(int i = 0; i < ColorCirclesPanel.Children.Count; i++)
            {
                if (ColorCirclesPanel.Children[i] is CircleColor circle)
                {
                    circle.WhiteCircle.Visibility = Visibility.Hidden;
                }
            }
        }

        private void PaletteBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
