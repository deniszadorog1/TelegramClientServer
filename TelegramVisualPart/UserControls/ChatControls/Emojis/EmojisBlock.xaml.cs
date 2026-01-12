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
using System.Xml.Serialization;
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

            SetEmojis();

            //SetEmojisBlock();
        }

        public void SetEmojis()
        {
            SetBaseEmojis();
            SetEmojisList();
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
            _baseEmojis.Clear();
            Random rnd = new Random();

            for (int code = 0x1F600; code <= 0x1F64F; code++)
            {
                _baseEmojis.Add((char.ConvertFromUtf32(code), ((EmojiType)rnd.Next(0, 3)), code.ToString()));
            }

            for (int code = 0x1F300; code <= 0x1F5FF; code++)
            {
                _baseEmojis.Add((char.ConvertFromUtf32(code), EmojiType.Other, code.ToString()));
            }

            for (int code = 0x1F680; code <= 0x1F6FF; code++)
            {
                _baseEmojis.Add((char.ConvertFromUtf32(code), EmojiType.Other, code.ToString()));
            }


            return;

            _baseEmojis.Add(("🙂", EmojiType.Positive, "NiceSmile"));
            _baseEmojis.Add(("😁", EmojiType.Positive, "Laugh"));
            _baseEmojis.Add(("😅", EmojiType.Positive, "GitItFace"));
            _baseEmojis.Add(("😊", EmojiType.Positive, "Smile"));
            _baseEmojis.Add(("😉", EmojiType.Positive, "Blink"));
            _baseEmojis.Add(("😎", EmojiType.Positive, "LikeFace"));
            _baseEmojis.Add(("😆", EmojiType.Positive, "EasyLaugh"));
            _baseEmojis.Add(("😀", EmojiType.Positive, "Happy"));
            _baseEmojis.Add(("😹", EmojiType.Positive, "CatLaugh"));
            _baseEmojis.Add(("😻", EmojiType.Positive, "CatLove"));
            _baseEmojis.Add(("🙉", EmojiType.Positive, "MonkeyOpen"));
            _baseEmojis.Add(("🙈", EmojiType.Positive, "MonkeyClose"));
            _baseEmojis.Add(("🤣", EmojiType.Positive, "HardLaugh"));

            _baseEmojis.Add(("☹️", EmojiType.Negative, "Smthing"));
            _baseEmojis.Add(("😐", EmojiType.Negative, "Nervous"));
            _baseEmojis.Add(("🤡", EmojiType.Negative, "Clown"));
            _baseEmojis.Add(("💩", EmojiType.Negative, "Poop"));
            _baseEmojis.Add(("😾", EmojiType.Negative, "CatAngry"));
            _baseEmojis.Add(("😿", EmojiType.Negative, "CatCry"));
            _baseEmojis.Add(("🙀", EmojiType.Negative, "CatShock"));
            _baseEmojis.Add(("😶", EmojiType.Negative, "NothingFace"));
            _baseEmojis.Add(("😈", EmojiType.Negative, "DevilSmile"));
            _baseEmojis.Add(("🌑", EmojiType.Negative, "BlackCirlce"));
            _baseEmojis.Add(("🍅", EmojiType.Negative, "Tomato"));
            _baseEmojis.Add(("🍱", EmojiType.Negative, "Sushi"));
            _baseEmojis.Add(("💀", EmojiType.Negative, "Death"));
            
            _baseEmojis.Add(("😺", EmojiType.Other, "Cat"));
            _baseEmojis.Add(("❤️", EmojiType.Other, "Heart"));
            _baseEmojis.Add(("🤷", EmojiType.Other, "Hands"));
            _baseEmojis.Add(("✈", EmojiType.Other, "Plane"));
            _baseEmojis.Add(("✂", EmojiType.Other, "Cisers"));
            _baseEmojis.Add(("✔", EmojiType.Other, "Tick"));
            _baseEmojis.Add(("❔", EmojiType.Other, "QuestionMark"));
            _baseEmojis.Add(("🚀", EmojiType.Other, "Rocket"));
            _baseEmojis.Add(("🚗", EmojiType.Other, "Car"));
            _baseEmojis.Add(("🚢", EmojiType.Other, "Ship"));
            _baseEmojis.Add(("🚩", EmojiType.Other, "Flag"));
            _baseEmojis.Add(("🚪", EmojiType.Other, "Door"));
            _baseEmojis.Add(("🚲", EmojiType.Other, "Bike"));
            _baseEmojis.Add(("⏰", EmojiType.Other, "Clock"));
            _baseEmojis.Add(("✨", EmojiType.Other, "Stars"));
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

            SetBackIcon(true);
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

        private void BackIconGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void BackIconGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private const PackIconKind _backKind = PackIconKind.ArrowLeft;
        private void BackIconGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BackIcon.Kind != _backKind) return;
            SetBackIcon(false);

            SetEmojis();
        }

        public void SetBackIcon(bool isBack)
        {
            BackIcon.Kind = isBack ? _backKind : PackIconKind.Magnify;
        }

    }
}
