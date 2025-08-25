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

namespace TelegramVisualPart.UserControls.ChatControls.Emojis
{
    /// <summary>
    /// Логика взаимодействия для Emoji.xaml
    /// </summary>
    public partial class Emoji : UserControl
    {
        public Emoji()
        {
            InitializeComponent();
        }

        public Emoji(string emoji)
        {
            InitializeComponent();
            EmojiBlock.Text = emoji;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            BorderBg.Background = 
                (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
            Cursor = Cursors.Hand;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            BorderBg.Background = new SolidColorBrush(Colors.Transparent);
            Cursor = null;
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).AddEmojiInChat(EmojiBlock.Text);
        }
    }
}
