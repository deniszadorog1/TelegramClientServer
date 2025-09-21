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
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;

namespace TelegramVisualPart.Pages.Settings.Language
{
    /// <summary>
    /// Логика взаимодействия для LanguagePage.xaml
    /// </summary>
    public partial class LanguagePage : Page
    {
        private TelSystem _system;
        public LanguagePage(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            SetBasicParams();

            SetLanguageText.SetLanguagePage(this);
        }

        public void SetBasicParams()
        {
            SetLangTextBlocks();
        }

        public void SetLangTextBlocks()
        {
            EngLanguage.LangEngName.Text = "English";
            EngLanguage.LangNativeName.Text = "English";

            RusLanguage.LangEngName.Text = "Russian";
            RusLanguage.LangNativeName.Text = "Русский";
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (sender is System.Windows.Controls.Button but) but.Background =
                (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            if (sender is System.Windows.Controls.Button but)
                but.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void OkBut_Click(object sender, RoutedEventArgs e)
        {
            //Set Language in db
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }
    }
}
