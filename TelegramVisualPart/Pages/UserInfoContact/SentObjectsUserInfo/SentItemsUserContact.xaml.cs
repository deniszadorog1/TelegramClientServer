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

        private List<string> _videoPaths;

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
                        SetVideosInPanel();
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

        public void SetVideosInPanel()
        {
            //Get paths for 
            _videoPaths = GetVideoFileNames();

            //Set preview image
            for(int i = 0; i < _videoPaths.Count; i++)
            {
                Image img = FilesAction.GetImagePreviewForVideo(_videoPaths[i]);

                img.Tag = _videoPaths[i];

                img.Stretch = Stretch.Fill;

                img.Width = mediaSize;
                img.Height = mediaSize;

                img.Margin = new Thickness(5);

                img.PreviewMouseDown += MediaVideos_PreviewMouseDown;

                img.MouseEnter += MediaElement_MouseEnter;
                img.MouseLeave += MediaElement_MouseLeave;

                ElemsPanel.Children.Add(img);
            }
        }

        public void MediaVideos_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image img || 
                img.Tag is not string tag) return;

            MediaElement videoElement = FilesAction.GetMediaElementByVideoName(tag);

            _videoPaths = FilesAction.GetFullPathForVideos(_videoPaths);

            //SetVideo Paths
            VisualActionPage page = new VisualActionPage(videoElement, _videoPaths);

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);

            List<MediaAction> videos = GetVideoMessages();

            int chosenVideoIndex = GetImageIndex(img);// _videoPaths.IndexOf(tag);

            page.SetUserChat(_system, videos, chosenVideoIndex);
        }

        public int GetImageIndex(Image img)
        {
            return ElemsPanel.Children.IndexOf(img);
        }

        public List<MediaAction> GetVideoMessages()
        {
            List<MediaAction> res = new List<MediaAction>();

            for (int i = 0; i < _chat.Messages.Count; i++)
            {
                if (_chat.Messages[i] is MediaAction media &&
                    FilesAction.IsFileIsVideo(media.MediaName))
                {
                    res.Add(media);
                }
            }
            return res;
        }

        public void SetImagesInPanel()
        {
            _imgs = GetImages();

            for(int i = 0; i < _imgs.Count; i++)
            {
                _imgs[i].Width = mediaSize;
                _imgs[i].Height = mediaSize;

                _imgs[i].Margin = new Thickness(5);

                _imgs[i].PreviewMouseDown += MediaImages_PreviewMouseDown;
                
                _imgs[i].MouseEnter += MediaElement_MouseEnter;
                _imgs[i].MouseLeave += MediaElement_MouseLeave;

                ElemsPanel.Children.Add(_imgs[i]);
            }
        }

        public void MediaImages_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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

        public List<string> GetVideoFileNames()
        {
            List<string> res = new List<string>();

            for(int i = 0; i < _chat.Messages.Count; i++)
            {
                if (_chat.Messages[i] is MediaAction media)
                {
                    if (!FilesAction.IsFileIsVideo(media.MediaName)) continue;
                    res.Add(media.MediaName);
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
