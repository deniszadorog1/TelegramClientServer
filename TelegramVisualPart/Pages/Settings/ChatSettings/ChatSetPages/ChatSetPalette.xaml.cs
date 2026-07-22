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
using TelegramLib.MainClasses;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
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
        private TelegramLib.MainClasses.TelSystem _system;

        public ChatSetPalette(TelegramLib.UserSettings.SettingsTypes.ChatSettings settings, TelSystem system)
        {
            const int leftPadding = 20;
            _settings = settings;
            _system = system;

            InitializeComponent();
            SetBasicBlocks();

            Palette.SpecialConditionTriggered += MyUserControl_SpecialConditionTriggered;
            ColorPickerLine.SpecialConditionTriggered += MyUserControl_SpecialConditionTriggered;

            Hex.Number.Padding = new Thickness(leftPadding, 0, 0, 0);

            SetLanguageText.SetChatSetPalette(this);
        }

        private void MyUserControl_SpecialConditionTriggered(object sender, EventArgs e)
        {
            //Update blocks
            const int maxH = 360;
            const int maxSL = 100;

            Color color = Palette._tempColor.RgbValue;

            HueBox.Number.Text = (Palette._tempColor.H * maxH).ToString();
            SaturationBox.Number.Text = (Palette._tempColor.S * maxSL).ToString();
            LuminisityBox.Number.Text = (Palette._tempColor.L * maxSL).ToString();

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

        private async void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush? bg = FirstColorRect.Fill as SolidColorBrush;

            //Get Get color id;

            int id = await GetColorId(bg);

            _settings.ChosenColor = new TelegramLib.Helpers.ColorHelper(id,
                bg.Color.R,
                bg.Color.G,
                bg.Color.B);


            ((MainWindow)Window.GetWindow(this)).UpdateChatSettingsPage();
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        public async Task<int> GetColorId(SolidColorBrush color)
        {
            //Is color exist in db
            bool isExist =  await ApiService.IsUserColorExist(_system.LoggedUser.Id);

            //if no => add it
            if (!isExist) await ApiService.AddUserColor(color.Color.R, color.Color.G, color.Color.B, _system.LoggedUser.Id);

            int res = await ApiService.GetUserColorId(_system.LoggedUser.Id);

            return res;
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
