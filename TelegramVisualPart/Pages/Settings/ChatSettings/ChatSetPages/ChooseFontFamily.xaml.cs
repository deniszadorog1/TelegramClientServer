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
using TelegramVisualPart.Helper;
using TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls.FontFamilyChoose;

namespace TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages
{
    /// <summary>
    /// Логика взаимодействия для ChooseFontFamily.xaml
    /// </summary>
    public partial class ChooseFontFamily : Page
    {
        private TelegramLib.UserSettings.SettingsTypes.ChatSettings _settings;
        public ChooseFontFamily(TelegramLib.UserSettings.SettingsTypes.ChatSettings chatSettings)
        {
            _settings = chatSettings;

            InitializeComponent();

            SetFontRadio();

            SetClassParams();

            SetLanguageText.SetFontFamily(this);
        }
        public void SetClassParams()
        {
            FontToChoose? chosen = FontsPanel.Children.OfType<FontToChoose>().Where(
                x => x.FontName.Content.ToString() == _settings.FontName).FirstOrDefault();
            if (chosen is null) return;
            chosen.FontName.IsChecked = true;

            FontFamily? font = Fonts.SystemFontFamilies
                .FirstOrDefault(f => f.Source.ToString() == _settings.FontName);
            if (font is null) return;

            //Set chosen in first place
            FontsPanel.Children.Remove(chosen);
            FontsPanel.Children.Insert(0, chosen);

            CheckFontBlock.FontFamily = font;
        }

        private List<FontToChoose> _fonts = new List<FontToChoose>();
        public void SetFontRadio()
        {
            _fonts.Clear();

            ClearSearch.IconType.Kind = PackIconKind.Close;

            var fontFamilies = Fonts.SystemFontFamilies;

            foreach (FontFamily font in Fonts.SystemFontFamilies)
            {
                FontToChoose toChoose = new FontToChoose();
                toChoose.FontName.Content = font.Source;
                toChoose.FontName.Checked += ToChoose_Checked;

                _fonts.Add(toChoose);
            }

            foreach(FontToChoose font in _fonts)
            {
                FontsPanel.Children.Add(font);
            }
        }

        public void ToChoose_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radio)
            {
                UncheckRadios(radio);

                FontFamily font =
                    Fonts.SystemFontFamilies.Where(f => f.Source ==
                    radio.Content.ToString()).First();

                CheckFontBlock.FontFamily = font;

                //set font in db and etc...
            }
        }

        public void UncheckRadios(RadioButton chosen)
        {
            foreach (object obj in FontsPanel.Children)
            {
                if (obj is FontToChoose toChoose && toChoose.FontName.Content != chosen.Content)
                {
                    toChoose.FontName.IsChecked = false;
                }
            }
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
            //Set changings
            _settings.FontName = CheckFontBlock.FontFamily.Source.ToString();

            ((MainWindow)Window.GetWindow(this)).UpdateChatSettingsPage();
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void ClearSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Text = string.Empty;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FontsPanel.Children.Clear();

            foreach(FontToChoose font in _fonts)
            {
                if(font.FontName.Content.ToString().Contains(SearchBox.Text))
                {
                    FontsPanel.Children.Add(font);
                }
            }
        }
    }
}
