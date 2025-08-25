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
    /// Логика взаимодействия для EmojisBlock.xaml
    /// </summary>
    public partial class EmojisBlock : UserControl
    {
        public EmojisBlock()
        {
            InitializeComponent();

            SetEmojisBlock();
        }

        public void SetEmojisBlock()
        {
            EmojisPanel.Children.Add(new Emoji("🙂"));
            EmojisPanel.Children.Add(new Emoji("😁"));
            EmojisPanel.Children.Add(new Emoji("😅"));
            EmojisPanel.Children.Add(new Emoji("😊"));
            EmojisPanel.Children.Add(new Emoji("😉"));
            EmojisPanel.Children.Add(new Emoji("😉"));
            EmojisPanel.Children.Add(new Emoji("😎"));
            EmojisPanel.Children.Add(new Emoji("😀"));
            EmojisPanel.Children.Add(new Emoji("😆"));
            EmojisPanel.Children.Add(new Emoji("☹️"));
            EmojisPanel.Children.Add(new Emoji("😐"));
            EmojisPanel.Children.Add(new Emoji("🤣"));
            EmojisPanel.Children.Add(new Emoji("❤️"));
            EmojisPanel.Children.Add(new Emoji("🤷"));
            EmojisPanel.Children.Add(new Emoji("🤡"));
            EmojisPanel.Children.Add(new Emoji("💩"));
            EmojisPanel.Children.Add(new Emoji("😺"));
        }
    }
}
