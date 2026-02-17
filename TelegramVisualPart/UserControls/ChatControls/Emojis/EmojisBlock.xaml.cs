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
                _baseEmojis.Add((char.ConvertFromUtf32(code), EmojiType.Other, code.ToString()));
            }

            for (int code = 0x1F300; code <= 0x1F5FF; code++)
            {
                _baseEmojis.Add((char.ConvertFromUtf32(code), EmojiType.Smiles, code.ToString()));
            }

            for (int code = 0x1F680; code <= 0x1F6FF; code++)
            {
                _baseEmojis.Add((char.ConvertFromUtf32(code), EmojiType.Symbols, code.ToString()));
            }

            return;

            _baseEmojis.Add(("🙂", EmojiType.Smiles, "NiceSmile"));
            _baseEmojis.Add(("😁", EmojiType.Smiles, "Laugh"));
            _baseEmojis.Add(("😅", EmojiType.Smiles, "GitItFace"));
            _baseEmojis.Add(("😊", EmojiType.Smiles, "Smile"));
            _baseEmojis.Add(("😉", EmojiType.Smiles, "Blink"));
            _baseEmojis.Add(("😎", EmojiType.Smiles, "LikeFace"));
            _baseEmojis.Add(("😆", EmojiType.Smiles, "EasyLaugh"));
            _baseEmojis.Add(("😀", EmojiType.Smiles, "Happy"));
            _baseEmojis.Add(("😹", EmojiType.Smiles, "CatLaugh"));
            _baseEmojis.Add(("😻", EmojiType.Smiles, "CatLove"));
            _baseEmojis.Add(("🙉", EmojiType.Smiles, "MonkeyOpen"));
            _baseEmojis.Add(("🙈", EmojiType.Smiles, "MonkeyClose"));
            _baseEmojis.Add(("🤣", EmojiType.Smiles, "HardLaugh"));

            _baseEmojis.Add(("☹️", EmojiType.Symbols, "Smthing"));
            _baseEmojis.Add(("😐", EmojiType.Symbols, "Nervous"));
            _baseEmojis.Add(("🤡", EmojiType.Symbols, "Clown"));
            _baseEmojis.Add(("💩", EmojiType.Symbols, "Poop"));
            _baseEmojis.Add(("😾", EmojiType.Symbols, "CatAngry"));
            _baseEmojis.Add(("😿", EmojiType.Symbols, "CatCry"));
            _baseEmojis.Add(("🙀", EmojiType.Symbols, "CatShock"));
            _baseEmojis.Add(("😶", EmojiType.Symbols, "NothingFace"));
            _baseEmojis.Add(("😈", EmojiType.Symbols, "DevilSmile"));
            _baseEmojis.Add(("🌑", EmojiType.Symbols, "BlackCirlce"));
            _baseEmojis.Add(("🍅", EmojiType.Symbols, "Tomato"));
            _baseEmojis.Add(("🍱", EmojiType.Symbols, "Sushi"));
            _baseEmojis.Add(("💀", EmojiType.Symbols, "Death"));
            
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
                icon == PosIcons ? EmojiType.Smiles :
                icon == NegIcons ? EmojiType.Symbols :
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

        private void ScrollViewer_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
