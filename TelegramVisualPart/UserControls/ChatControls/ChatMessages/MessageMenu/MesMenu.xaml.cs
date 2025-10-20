using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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
using TelegramVisualPart.Enums.Menus;

namespace TelegramVisualPart.UserControls.ChatControls.ChatMessages.MessageMenu
{
    /// <summary>
    /// Логика взаимодействия для MesMenu.xaml
    /// </summary>
    public partial class MesMenu : UserControl
    {
        private MessageMenuType _type;
        public MesMenu(MessageMenuType type)
        {
            _type = type;
            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            ReplyBut.Icon.Kind = PackIconKind.Reply;
            ReplyBut.ButText.Text = "Reply";

            PinBut.Icon.Kind = PackIconKind.PinOutline;
            PinBut.ButText.Text = "Pin";

            SaveAsBut.Icon.Kind = PackIconKind.ContentSaveOutline;
            SaveAsBut.ButText.Text = "Save as...";

            CopyBut.Icon.Kind = PackIconKind.ContentCopy;
            CopyBut.ButText.Text = "Copy this";

            ForwardBut.Icon.Kind = PackIconKind.ForwardOutline;
            ForwardBut.ButText.Text = "Forward";

            DeleteBut.Icon.Kind = PackIconKind.DeleteForeverOutline;
            DeleteBut.ButText.Text = "Delete";

            SelectBut.Icon.Kind = PackIconKind.ProgressTick;
            SelectBut.ButText.Text = "Select";

            if(_type != MessageMenuType.MediaMessage) Buts.Children.Remove(SaveAsBut);
        }


        private ListBoxItem _item;
        public void SetClickedListBoxItem(ListBoxItem item)
        {
            _item = item;
        }

        public ListBoxItem GetChosenListBoxItem() => _item;

        public event Action ReplyAct;
        public event Action PinAct;
        public event Action SaveAct;
        public event Action CopyAct;
        public event Action ForwardAct;
        public event Action DeleteAct;
        public event Action SelectAct;

        private void ReplyBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ReplyAct?.Invoke();
        }

        private void PinBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PinAct?.Invoke();
        }

        private void SaveAsBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SaveAct?.Invoke();
        }

        private void CopyBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CopyAct?.Invoke();
        }

        private void ForwardBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void DeleteBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DeleteAct?.Invoke();
        }

        private void SelectBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public void SetPinVisStatus(TelegramLib.MainClasses.Messages.Message mes)
        {
            if (mes.IsPinned)
            {
                PinBut.Icon.Kind = PackIconKind.PinOffOutline;
                PinBut.ButText.Text = "UnPin";
                return;
            }
            PinBut.Icon.Kind = PackIconKind.PinOutline;
            PinBut.ButText.Text = "Pin";
        }

    }
}
