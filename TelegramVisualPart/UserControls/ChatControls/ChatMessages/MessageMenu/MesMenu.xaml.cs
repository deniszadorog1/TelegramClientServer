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
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
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
        private bool _isSchedMessages;

        private TelegramLib.MainClasses.Messages.Message _mes;
        private TelSystem _system;

        public MesMenu(MessageMenuType type,
            bool isOnlyPinnedChat,
            TelegramLib.MainClasses.Messages.Message mes,
            TelSystem system,
            bool isSchedMessages)
        {
            _type = type;
            _isOnlyPinnedChat = isOnlyPinnedChat;
            _mes = mes;
            _system = system;
            _isSchedMessages = isSchedMessages;

            InitializeComponent();

            SetBasicParams();
        }


        //Menu fro, contact info items
        public MesMenu(TelSystem system, TelegramLib.MainClasses.Messages.Message mes)
        {
            _mes = mes;
            _system = system;

            _type = MessageMenuType.ContactInfoMedias;
            _isSchedMessages = false;

            InitializeComponent();

            SetBasicParams();
        }

        //reset 
        public MesMenu(TelegramLib.MainClasses.Messages.Message mes)
        {
            _mes = mes;
            InitializeComponent();
        }

        public TelegramLib.MainClasses.Messages.Message GetMessage() => _mes;

        public void SetBasicParams()
        {
            GoToMessage.SetParams(PackIconKind.EyeOutline, "Go to message");
            ReplyBut.SetParams(PackIconKind.Reply, "Reply");

            EditBut.Icon.Kind = PackIconKind.PencilOutline;
            EditBut.ButText.Text = "Edit";
            IsAddEditBut();

            PinBut.SetParams(PackIconKind.PinOutline, "Pin");
            ShowInFolder.SetParams(PackIconKind.FolderOutline, "Show in folder");

            SaveAsBut.SetParams(PackIconKind.ContentSaveOutline, "Save as...");
            CopyBut.SetParams(PackIconKind.ContentCopy, "Copy this");

            ForwardBut.SetParams(PackIconKind.ForwardOutline, "Forward");
            DeleteBut.SetParams(PackIconKind.DeleteForeverOutline, "Delete");

            SelectBut.SetParams(PackIconKind.ProgressTick, "Select");

            SendNowBut.SetParams(PackIconKind.SendOutline, "Send now");
            RescheduleBut.SetParams(PackIconKind.Refresh, "Reschedule");


            RemoveUnnesBlocks();

            if (_type != MessageMenuType.ContactInfoMedias)
            {
                if (!_isOnlyPinnedChat) Buts.Children.Remove(GoToMessage);
                else Buts.Children.Remove(ReplyBut);
            }

            if (_mes is MediaAction media && !media.IsImage()) Buts.Children.Remove(CopyBut);
            if (_mes is TelegramLib.MainClasses.Messages.TextMessage text) SetEditMesVis(text);
        }

        public void SetEditMesVis(TelegramLib.MainClasses.Messages.TextMessage text)
        {
            const int timeToEdit = 30;

            if (text.SenderUserId != _system.LoggedUser.Id ||
               (DateTime.Now - text.SentTime).TotalSeconds > timeToEdit)
            {
                Buts.Children.Remove(EditBut);
            }
        }

        public void RemoveUnnesBlocks()
        {
            switch (_type)
            {
                case MessageMenuType.TextMessage:
                    {
                        Buts.Children.Remove(ShowInFolder);
                        Buts.Children.Remove(SaveAsBut);
                        break;
                    }
                case MessageMenuType.MediaMessage:
                    {
                        Buts.Children.Remove(EditBut);
                        break;
                    }
                case MessageMenuType.ShareContact:
                    {
                        Buts.Children.Remove(ShowInFolder);
                        Buts.Children.Remove(EditBut);
                        Buts.Children.Remove(SaveAsBut);
                        break;
                    }
                case MessageMenuType.StatMessage:
                    {
                        Buts.Children.Remove(GoToMessage);
                        Buts.Children.Remove(EditBut);
                        Buts.Children.Remove(PinBut);
                        Buts.Children.Remove(ShowInFolder);
                        Buts.Children.Remove(SaveAsBut);
                        Buts.Children.Remove(CopyBut);
                        Buts.Children.Remove(ForwardBut);
                        Buts.Children.Remove(SelectBut);
                        break;
                    }
                case MessageMenuType.ContactInfoMedias:
                    {
                        Buts.Children.Remove(EditBut);
                        Buts.Children.Remove(PinBut);
                        Buts.Children.Remove(ShowInFolder);
                        Buts.Children.Remove(SaveAsBut);
                        Buts.Children.Remove(CopyBut);
                        Buts.Children.Remove(SendNowBut);
                        Buts.Children.Remove(SelectBut);
                        Buts.Children.Remove(RescheduleBut);
                        Buts.Children.Remove(ForwardBut);
                        break;
                    }
            }

            if (_isSchedMessages)
            {
                RemoveButsForSchedMenu();
            }
            else RemoveSchedButs();
        }

        public void RemoveSchedButs()
        {
            Buts.Children.Remove(SendNowBut);
            Buts.Children.Remove(RescheduleBut);
        }

        public void RemoveButsForSchedMenu()
        {
            Buts.Children.Remove(GoToMessage);
            Buts.Children.Remove(ReplyBut);
            Buts.Children.Remove(PinBut);
            Buts.Children.Remove(ShowInFolder);
            Buts.Children.Remove(SaveAsBut);
            Buts.Children.Remove(ForwardBut);
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
        public event Action ShowInFolderAct;
        public event Action SaveAct;
        public event Action CopyAct;
        public event Action ForwardAct;
        public event Action DeleteAct;
        public event Action SelectAct;
        public event Action EditAct;
        public event Action SendNowAct;
        public event Action RescheduleMessageAct;
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
            if (mes is null) return;
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
            EditAct?.Invoke();
        }

        private void ShowInFolder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowInFolderAct?.Invoke();
        }

        private void SendNowBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SendNowAct?.Invoke();
        }

        private void RescheduleBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            RescheduleMessageAct?.Invoke();
        }
    }
}
