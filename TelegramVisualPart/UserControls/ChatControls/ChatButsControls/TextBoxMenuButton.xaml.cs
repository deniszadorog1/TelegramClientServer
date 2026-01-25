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

namespace TelegramVisualPart.UserControls.ChatControls.ChatButsControls
{
    /// <summary>
    /// Логика взаимодействия для TextBoxMenuButton.xaml
    /// </summary>
    public partial class TextBoxMenuButton : UserControl
    {
        public TextBoxMenuButton()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeSecond"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            Background = (SolidColorBrush)Application.Current.Resources["DarkThemeOne"];
        }

        public void SetTextParams(string leftText, string rightText)
        {
            LeftTextBlock.Text = leftText;
            RightTextBlock.Text = rightText;
        }

        public void SetEnableStatus(bool isEnable)
        {
            if (!isEnable)
            {
                IsEnabled = false;
                LeftTextBlock.Foreground = new SolidColorBrush(Colors.Gray);
                RightTextBlock.Foreground = new SolidColorBrush(Colors.Gray);
                return;
            }

            IsEnabled = true;
            LeftTextBlock.Foreground = 
                (SolidColorBrush)Application.Current.Resources["UsualTextColor"];
            RightTextBlock.Foreground =
                (SolidColorBrush)Application.Current.Resources["UsualTextColor"];
        }
    }
}
