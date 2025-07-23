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
using TelegramVisualPart.UserControls.SettingsControls.AutoDeleteMessages;

namespace TelegramVisualPart.UserControls.SettingsControls.FoldersPrivacy
{
    /// <summary>
    /// Логика взаимодействия для FolderIcons.xaml
    /// </summary>
    public partial class FolderIcons : UserControl
    {
        private PackIconKind _chosenIcon;

        public event EventHandler NewIconChosenEvent;

        public FolderIcons()
        {
            InitializeComponent();
            SetIconsKind();
        }

        private void SetIconsKind()
        {
            Folder.Icon.Kind = PackIconKind.Folder;
            Cat.Icon.Kind = PackIconKind.Cat;
            Book.Icon.Kind = PackIconKind.Book;
            Bitcoin.Icon.Kind = PackIconKind.Bitcoin;
            Lightbulb.Icon.Kind = PackIconKind.Lightbulb;

            Controller.Icon.Kind = PackIconKind.Controller;
            MusicNote.Icon.Kind = PackIconKind.MusicNote;
            Paintbrush.Icon.Kind = PackIconKind.Paintbrush;
            Aeroplane.Icon.Kind = PackIconKind.Aeroplane;
            Volleyball.Icon.Kind = PackIconKind.Volleyball;

            Star.Icon.Kind = PackIconKind.StarRate;
            Account.Icon.Kind = PackIconKind.AccountCircleOutline;
            Accounts.Icon.Kind = PackIconKind.AccountMultipleAddOutline;
            Chat.Icon.Kind = PackIconKind.Chat;
            Android.Icon.Kind = PackIconKind.Android;

            King.Icon.Kind = PackIconKind.ChessKing;
            Flower.Icon.Kind = PackIconKind.Flower;
            Heart.Icon.Kind = PackIconKind.Heart;
            GuyMask.Icon.Kind = PackIconKind.GuyFawkesMask;
            Bell.Icon.Kind = PackIconKind.Bell;

            Bag.Icon.Kind = PackIconKind.MedicalBag;
            Car.Icon.Kind = PackIconKind.CarSide;
            Pumpkin.Icon.Kind = PackIconKind.Pumpkin;
            Spapde.Icon.Kind = PackIconKind.CardsSpade;
            Apple.Icon.Kind = PackIconKind.Apple;
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if(sender is IconForFolder el)
            {
                el.BgGrid.Background = Brushes.DarkSlateGray;
            }
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is IconForFolder el)
            {
                el.BgGrid.Background = null;
            }
        }

        private void But_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not IconForFolder icon) return;
            _chosenIcon = icon.GetIconKindType();

            NewIconChosenEvent?.Invoke(this, EventArgs.Empty);
        }

        public PackIconKind GetChosenIconName()
        {
            return _chosenIcon;
        }
    }
}
