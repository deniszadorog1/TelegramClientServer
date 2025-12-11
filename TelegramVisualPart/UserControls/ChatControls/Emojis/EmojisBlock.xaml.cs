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
using TelegramVisualPart.Enums;

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

            SetBaseEmojis();
            SetEmojisList();

            //SetEmojisBlock();
        }

        private List<(string, EmojiType, string)> _baseEmojis = new List<(string, EmojiType, string)>();
        private EmojiType? _chosenType;

        public void SetEmojisList()
        {
            ClearTypeIconsColor();
            EmojisPanel.Children.Clear();
            SarchBox.Text = string.Empty;
            _chosenType = null;

            for (int i = 0; i < _baseEmojis.Count; i++)
            {
                //_emojis.Add(i, _baseEmojis[i]);

                EmojisPanel.Children.Add(new Emoji(_baseEmojis[i].Item1, _baseEmojis[i].Item2));
            }
        }

        public void SetBaseEmojis()
        {
            _baseEmojis.Add(("🙂", EmojiType.Positive, "NiceSmile"));
            _baseEmojis.Add(("😁", EmojiType.Positive, "Laugh"));
            _baseEmojis.Add(("😅", EmojiType.Positive, "GitItFace"));
            _baseEmojis.Add(("😊", EmojiType.Positive, "Smile"));
            _baseEmojis.Add(("😉", EmojiType.Positive, "Blink"));
            _baseEmojis.Add(("😎", EmojiType.Positive, "LikeFace"));
            _baseEmojis.Add(("😆", EmojiType.Positive, "EasyLaugh"));
            _baseEmojis.Add(("😀", EmojiType.Positive, "Happy"));
            _baseEmojis.Add(("☹️", EmojiType.Negative, "Smthing"));
            _baseEmojis.Add(("😐", EmojiType.Negative, "Nervous"));
            _baseEmojis.Add(("🤣", EmojiType.Positive, "HardLaugh"));
            _baseEmojis.Add(("❤️", EmojiType.Other, "Heart"));
            _baseEmojis.Add(("🤷", EmojiType.Other, "Hands"));
            _baseEmojis.Add(("🤡", EmojiType.Negative, "Clown"));
            _baseEmojis.Add(("💩", EmojiType.Negative, "Poop"));
            _baseEmojis.Add(("😺", EmojiType.Other, "Cat"));
        }

        private void PackIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void PackIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void PackIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not PackIcon icon) return;
            //Set Gray color to icon
            ClearTypeIconsColor();

            //Set white to chosen
            icon.Foreground = new SolidColorBrush(Colors.White);

            //Clear Emojis Panel
            EmojisPanel.Children.Clear();

            //GetType
            _chosenType =
                icon == PosIcons ? EmojiType.Positive :
                icon == NegIcons ? EmojiType.Negative :
                EmojiType.Other;

            //Set Emojis By type
            SetIconsByTypeAndName();
        }

        public void ClearTypeIconsColor()
        {
            for (int i = 0; i < TypeIconsPanel.Children.Count; i++)
            {
                if (TypeIconsPanel.Children[i] is not PackIcon icon) continue;

                icon.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        public void SetIconsByTypeAndName()
        {
            EmojisPanel.Children.Clear();
            for (int i = 0; i < _baseEmojis.Count; i++)
            {
                if ((_chosenType is not null && _baseEmojis[i].Item2 != _chosenType)
                    || 
                (SarchBox.Text != string.Empty && !_baseEmojis[i].Item3.ToLower().Contains(SarchBox.Text.ToLower()))) continue;

                EmojisPanel.Children.Add(new Emoji(_baseEmojis[i].Item1, _baseEmojis[i].Item2));
            }
        }

        private void SarchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Update empjis by name
            SetIconsByTypeAndName();
        }

        /*   public void SetEmojisBlock()
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
           }*/
    }
}
