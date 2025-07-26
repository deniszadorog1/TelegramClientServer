using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.UserControls.ChatControls;

namespace TelegramVisualPart.Pages.UserInfoContact.SentObjectsUserInfo
{
    /// <summary>
    /// Логика взаимодействия для SentItemsUserContact.xaml
    /// </summary>
    public partial class SentItemsUserContact : Page
    {
        private const int mediaSize = 85;

        private TelSystem _system;
        private List<Image> _imgs;

        private Enums.SentItemsTypes _type;
        private UserChat _chat;
        public SentItemsUserContact(TelSystem system,
            Enums.SentItemsTypes type, UserChat chat)
        {
            _type = type;
            _chat = chat;
            _system = system;
            InitializeComponent();

            SetBasicBlocks();
            SetIconsKind();

            SetFiles();
        }

        private void SetFiles()
        {
            switch (_type)
            {
                case Enums.SentItemsTypes.Photos:
                    {
                        SetImagesInPanel();
                        break;
                    }
                case Enums.SentItemsTypes.Video:
                    {
                        break;
                    }
                case Enums.SentItemsTypes.File:
                    {
                        break;
                    }
                case Enums.SentItemsTypes.SharedLinks:
                    {
                        break;
                    }
                case Enums.SentItemsTypes.GIFs:
                    {
                        break;
                    }
            }
        }

        public void SetImagesInPanel()
        {
            _imgs = GetImages();

            for(int i = 0; i < _imgs.Count; i++)
            {
                _imgs[i].Width = mediaSize;
                _imgs[i].Height = mediaSize;

                _imgs[i].Margin = new Thickness(5);

                _imgs[i].PreviewMouseDown += Media_PreviewMouseDown;
                
                _imgs[i].MouseEnter += MediaElement_MouseEnter;
                _imgs[i].MouseLeave += MediaElement_MouseLeave;

                ElemsPanel.Children.Add(_imgs[i]);
            }
        }

        public void Media_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image img) return;

            VisualActionPage page = new VisualActionPage(img, _imgs);

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);

            page.SetUserChat(_system, GetImageMessages(), _imgs.IndexOf(img));
        }

        public List<MediaAction> GetImageMessages()
        {
            List<MediaAction> res = new List<MediaAction>();

            for(int i = 0; i < _chat.Messages.Count; i++)
            {
                if (_chat.Messages[i] is MediaAction media &&
                    FilesAction.IsFileIsImage(media.MediaName))
                {
                    res.Add(media);
                }
            }

            return res;
        }

        public List<Image> GetImages()
        {
            List<Image> res = new List<Image>();
            for(int i = 0; i < _chat.Messages.Count; i++)
            {
                if(_chat.Messages[i] is MediaAction media)
                {
                    if (!FilesAction.IsUserChatMediaIsExist(media.MediaName)) continue;

                    res.Add(FilesAction.GetImageFromChatImageFolder(media.MediaName));
                }
            }
            return res;
        }

        public void SetIconsKind()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;
        }

        public void SetBasicBlocks()
        {
            switch (_type)
            {
                case Enums.SentItemsTypes.Photos:
                    {
                        PageName.Text = "Photos";
                        break;
                    }
                case Enums.SentItemsTypes.Video:
                    {
                        PageName.Text = "Videos";
                        break;
                    }
                case Enums.SentItemsTypes.File:
                    {
                        PageName.Text = "Files";
                        break;
                    }
                case Enums.SentItemsTypes.SharedLinks:
                    {
                        PageName.Text = "Shared Links";
                        break;
                    }
                case Enums.SentItemsTypes.GIFs:
                    {
                        PageName.Text = "GIFs";
                        break;
                    }
            }
        }
        public void SetBasicIconsKind()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;
        }

        private void BackBut_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void CloseBut_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void MediaElement_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void MediaElement_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
