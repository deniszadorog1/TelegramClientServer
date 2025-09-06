using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using TelegramLib.UserSettings.SettingsTypes;
using TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls.PaletteControls;


namespace TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages
{
    /// <summary>
    /// Логика взаимодействия для ChatSetPalette.xaml
    /// </summary>
    public partial class ChatSetPalette : Page
    {
        public event EventHandler CustomMouseMoved;
        private TelegramLib.UserSettings.SettingsTypes.ChatSettings _settings;

        public ChatSetPalette(TelegramLib.UserSettings.SettingsTypes.ChatSettings settings)
        {
            _settings = settings;

            InitializeComponent();
            SetBasicBlocks();

            Palette.SpecialConditionTriggered += MyUserControl_SpecialConditionTriggered;
            ColorPickerLine.SpecialConditionTriggered += MyUserControl_SpecialConditionTriggered;

            Hex.Number.Padding = new Thickness(20, 0, 0, 0);
        }

        private void MyUserControl_SpecialConditionTriggered(object sender, EventArgs e)
        {
            //Update blocks

            Color color = Palette._tempColor.RgbValue;

            HueBox.Number.Text = (Palette._tempColor.H * 360).ToString();
            SaturationBox.Number.Text = (Palette._tempColor.S * 100).ToString();
            LuminisityBox.Number.Text = (Palette._tempColor.L * 100).ToString();

            RedBox.Number.Text = color.R.ToString();
            GreenBox.Number.Text = color.G.ToString();
            BlueBox.Number.Text = color.B.ToString();
            Hex.Number.Text = Palette._tempColor.GetHexFromRGB();

            FirstColorRect.Fill = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));

            ColorPickerLine._stop.Color = color;

            Palette.AdjustBrushLuminosity(ColorPickerLine._lum);
        }

        public void SetBasicBlocks()
        {
            HueBox.FrontLetter.Content = "H";
            HueBox.BackLetter.Content = "'";

            SaturationBox.FrontLetter.Content = "S";
            SaturationBox.BackLetter.Content = "%";

            LuminisityBox.FrontLetter.Content = "L";
            LuminisityBox.BackLetter.Content = "%";

            RedBox.FrontLetter.Content = "R";
            RedBox.BackLetter.Visibility = Visibility.Hidden;

            GreenBox.FrontLetter.Content = "G";
            GreenBox.BackLetter.Visibility = Visibility.Hidden;

            BlueBox.FrontLetter.Content = "B";
            BlueBox.BackLetter.Visibility = Visibility.Hidden;

            Hex.FrontLetter.Content = "#";
            Hex.BackLetter.Visibility = Visibility.Hidden;
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
                but.Background = (SolidColorBrush)Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush? bg = FirstColorRect.Fill as SolidColorBrush;

            _settings.ChosenColor = new TelegramLib.Helpers.ColorHelper(-1,
                bg.Color.R,
                bg.Color.G,
                bg.Color.B);

            ((MainWindow)Window.GetWindow(this)).UpdateChatSettingsPage();
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void ColumnDefinition_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }
    }
}
