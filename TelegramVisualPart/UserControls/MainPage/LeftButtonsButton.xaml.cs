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

namespace TelegramVisualPart.UserControls.MainPage
{
    /// <summary>
    /// Логика взаимодействия для LeftButtonsButton.xaml
    /// </summary>
    public partial class LeftButtonsButton : UserControl
    {
        private readonly SolidColorBrush _basicColor = new SolidColorBrush(Colors.Gray);
        //private readonly SolidColorBrush _activeColor = 
            

        public LeftButtonsButton()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public void SetIconKind(PackIconKind kind)
        {
            ButIcon.Kind = kind;
        }

        public void SetButtonText(string text)
        {
            ButText.Text = text;
        }

        public void SetActiveColor()
        {
            ButText.Foreground = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
            ButIcon.Foreground = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
        }

        public void SetBasicColors()
        {
            ButText.Foreground = _basicColor;
            ButIcon.Foreground = _basicColor;
        }
    }
}
