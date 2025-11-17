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
using TelegramLib.Models;
using TelegramVisualPart.Enums.Menus;

namespace TelegramVisualPart.UserControls.ChatControls.ChatMessages.MessageMenu
{
    /// <summary>
    /// Логика взаимодействия для MesMenu.xaml
    /// </summary>
    public partial class MesMenu : UserControl
    {
        private MessageMenuType _type;
        private bool _isOnlyPinnedChat;
        private TelegramLib.MainClasses.Messages.Message _mes;

        public MesMenu(MessageMenuType type, bool isOnlyPinnedChat,
            TelegramLib.MainClasses.Messages.Message mes)
        {
            _type = type;
            _isOnlyPinnedChat = isOnlyPinnedChat;
            _mes = mes;

            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            GoToMessage.Icon.Kind = PackIconKind.EyeOutline;
            GoToMessage.ButText.Text = "Go to message";

            ReplyBut.Icon.Kind = PackIconKind.Reply;
            ReplyBut.ButText.Text = "Reply";

            Buts.Children.Remove(EditBut);
            /*            EditBut.Icon.Kind = PackIconKind.PencilOutline;
                        EditBut.ButText.Text = "Edit";
                        IsAddEditBut();*/

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

            RemoveUnnesBlocks();

            if (!_isOnlyPinnedChat) Buts.Children.Remove(GoToMessage);
            else Buts.Children.Remove(ReplyBut);
        }
        
        public void RemoveUnnesBlocks()
        {
            switch (_type)
            {
                case MessageMenuType.TextMessage:
                    {
                        break;
                    }
                case MessageMenuType.MediaMessage:
                    {
                        Buts.Children.Remove(SaveAsBut);
                        break;
                    }
                case MessageMenuType.ShareContact:
                    {
                        break;
                    }
                case MessageMenuType.StatMessage:
                    {
                        Buts.Children.Remove(GoToMessage);
                        Buts.Children.Remove(EditBut);
                        Buts.Children.Remove(PinBut);
                        Buts.Children.Remove(SaveAsBut);
                        Buts.Children.Remove(CopyBut);
                        Buts.Children.Remove(ForwardBut);
                        Buts.Children.Remove(SelectBut);

                        break;
                    }
            }
        }

        public void IsAddEditBut()
        {
            return;
            const int timeDiffer = 30;

            if (_mes is null) return;
            if (Math.Abs((_mes.SentTime - DateTime.Now).TotalSeconds) < timeDiffer) return;

            //Remove edit button
            Buts.Children.Remove(GoToMessage);
        }

        private ListBoxItem _item;
        public void SetClickedListBoxItem(ListBoxItem item)
        {
            _item = item;
        }

        public ListBoxItem GetChosenListBoxItem() => _item;

        public event Action GoToMessageAct;
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
            ForwardAct?.Invoke();
        }

        private void DeleteBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DeleteAct?.Invoke();
        }

        private void SelectBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectAct?.Invoke();
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

        private void GoToMessage_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GoToMessageAct?.Invoke();
        }

        private void EditBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set edit menu in chat
        }
    }
}
