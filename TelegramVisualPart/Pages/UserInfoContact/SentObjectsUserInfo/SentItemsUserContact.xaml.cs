using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls.ChatMessages.MessageMenu;
using TelegramVisualPart.UserControls.ContactsControls;
using TelegramVisualPart.Windows;

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
        private List<string> _gifPaths;

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

            AddDate();
        }

        public void AddDate()
        {
            Date.Text = DateTime.Now.ToString("MMMM", new CultureInfo("en-US"));
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
                case Enums.SentItemsTypes.SharedLinks:
                    {
                        SetLinksInPanel();
                        break;
                    }
                case Enums.SentItemsTypes.GIFs:
                    {
                        SetGifsInPanel();
                        break;
                    }
            }
        }

        public async Task SetLinksInPanel()
        {
            List<string> links = _chat.GetLinks();

            for (int i = 0; i < links.Count; i++)
            {
                (string title, string desc) siteParams = await GetParsedParams(links[i]);

                SentLinkControl linkControl = new SentLinkControl(siteParams.title, siteParams.desc, links[i]);
                ElemsPanel.Children.Add(linkControl);
            }
        }

        public async Task<(string, string)> GetParsedParams(string link)
        {
            var client = new HttpClient();
            var html = await client.GetStringAsync(link);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            string title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText;

            var node = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");
            string description = node?.GetAttributeValue("content", null);

            title = RemoveUnnesParts(title);
            description = RemoveUnnesParts(description);

            return (title, description);
        }

        public string RemoveUnnesParts(string str)
        {
            if (str is null || str == string.Empty) return string.Empty;
            str = str.Replace("\n", "");
            str = str.Replace("\t", "");

            return str;
        }


        public void SetGifsInPanel()
        {
            List<MediaAction> medias = GetGifFileNames();
            _gifPaths = medias.Select(x => x.MediaName).ToList();

            for (int i = 0; i < _gifPaths.Count; i++)
            {
                Image gifImg = new Image()
                {
                    Width = mediaSize,
                    Height = mediaSize,
                    Stretch = Stretch.Fill,
                    Margin = new Thickness(5)
                };

                gifImg.Tag = i;

                var uri = new Uri(FilesAction.GetFullGifPath(_gifPaths[i]), UriKind.RelativeOrAbsolute);
                var source = new BitmapImage(uri);
                WpfAnimatedGif.ImageBehavior.SetAnimatedSource(gifImg, source);
                WpfAnimatedGif.ImageBehavior.SetRepeatBehavior(gifImg, RepeatBehavior.Forever);

                gifImg.MouseEnter += MediaElement_MouseEnter;
                gifImg.MouseLeave += MediaElement_MouseLeave;

                gifImg.PreviewMouseDown += MediaGifs_PreviewMouseDown;

                ElemsPanel.Children.Add(gifImg);
            }
        }

        public void MediaGifs_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image img) return;
            int.TryParse(img.Tag.ToString(), out int imgIndex);

            //string path = _gifPaths[imgIndex];
            //string chosenGifPath = FilesAction.GetFullGifPath(_gifPaths[int.Parse(img.Tag.ToString())]);

            _gifPaths = FilesAction.GetFullGifPaths(_gifPaths);
            List<MediaAction> gifs = GetGifMessages();

            //List<MediaAction> videos = _system.GetAllVideoMessages();
            //int chosenVideoIndex = GetImageIndex(img);// _videoPaths.IndexOf(tag);

            MediaWindow mediaWindow = new MediaWindow(
                null, (MainWindow)Window.GetWindow(this),
                Enums.MediaShow.MediaShowType.Gif, _system);

            mediaWindow.SetGif(imgIndex, _gifPaths, gifs, false);
            mediaWindow.Show();

            //SetVideo Paths
            /*            VisualActionPage page = new VisualActionPage(chosenGifPath, _gifPaths);

                        ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);


                        int chosenGifIndex = GetImageIndex(img);// _videoPaths.IndexOf(tag);

                        page.SetUserChat(_system, gifs, chosenGifIndex, _chat);*/
        }

        public List<MediaAction> GetGifMessages()
        {
            List<MediaAction> res = new List<MediaAction>();

            for (int i = 0; i < _chat.Messages.Count; i++)
            {
                if (_chat.Messages[i] is MediaAction media &&
                    FilesAction.IsFileIsGif(media.MediaName))
                {
                    res.Add(media);
                }
            }
            return res;
        }

        public void SetVideosInPanel()
        {
            //Get paths for 
            _videoPaths = GetVideoFileNames();

            //Set preview image
            for (int i = 0; i < _videoPaths.Count; i++)
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

            List<MediaAction> videos = _chat.GetChatVideos();// _system.GetAllVideoMessages();
            //int chosenVideoIndex = GetImageIndex(img);// _videoPaths.IndexOf(tag);

            MediaWindow mediaWindow = new MediaWindow(
                null, (MainWindow)Window.GetWindow(this),
                Enums.MediaShow.MediaShowType.Videos, _system);

            mediaWindow.SetVideos(videoElement, _videoPaths, videos);
            mediaWindow.Show();


            /*            //SetVideo Paths
                        VisualActionPage page = new VisualActionPage(videoElement, _videoPaths);

                        ((MainWindow)Window.GetWindow(this)).SetThirdFrame(page);

                        List<MediaAction> videos = GetVideoMessages();

                        int chosenVideoIndex = GetImageIndex(img);// _videoPaths.IndexOf(tag);

                        page.SetUserChat(_system, videos, chosenVideoIndex, _chat);*/
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

        private MesMenu _menu;
        public void SetImagesInPanel()
        {
            SetChatMediaObjs();
            _imgs = GetImages();

            SetOnlyImagesInMedias();

            for (int i = 0; i < _imgs.Count; i++)
            {
                _imgs[i].Width = mediaSize;
                _imgs[i].Height = mediaSize;

                _imgs[i].Tag = _medias[i].Id;

                _imgs[i].Margin = new Thickness(5);

                _imgs[i].PreviewMouseLeftButtonDown += MediaImages_PreviewMouseDown;

                int index = i;
                _imgs[i].PreviewMouseRightButtonDown += (sender, e) =>
                {
                    MessageMenu.Children.Clear();

                    _menu = new MesMenu(_system, _medias[index]);

                    ListBoxItem item = ((MainWindow)Window.GetWindow(this)).GetChatListBoxItemByMesId(_medias[index].Id);
                    if (item is null) return;
                    _menu.SetClickedListBoxItem(item);

                    System.Windows.Point point = e.GetPosition(this);
                    _menu.Loaded += (send, ev) =>
                    {
                        //is x to big
                        if (point.X + _menu.ActualWidth > this.ActualWidth)
                        {
                            Canvas.SetLeft(_menu, point.X - _menu.Width);
                        }
                        else Canvas.SetLeft(_menu, point.X);

                        //is y too big
                        if (point.Y + _menu.ActualHeight > this.ActualHeight)
                        {
                            Canvas.SetTop(_menu, this.ActualHeight - _menu.ActualHeight);
                        }
                        else Canvas.SetTop(_menu, point.Y);

                        Keyboard.ClearFocus();
                    };

                    MessageMenu.Children.Add(_menu);
                    SetMediaEvents();

                };

                _imgs[i].MouseEnter += MediaElement_MouseEnter;
                _imgs[i].MouseLeave += MediaElement_MouseLeave;



                ElemsPanel.Children.Add(_imgs[i]);
            }
        }

        public void SetMediaEvents()
        {
            if (_menu is null) return;

            _menu.GoToMessageAct += () =>
            {
                ClearPages();
                ((MainWindow)Window.GetWindow(this)).ShowChosenMessageByMessageId(_menu.GetMessage().Id);
            };

            _menu.ReplyAct += () =>
            {
                ClearPages();

                ListBoxItem item = _menu.GetChosenListBoxItem();
                TelegramLib.MainClasses.Messages.Message mes = _menu.GetMessage();

                if (item.Content is not UserControl control || mes is null) return;

                ((MainWindow)Window.GetWindow(this)).SetReplyMessage(control, new List<TelegramLib.MainClasses.Messages.Message>() { mes });
            };

            _menu.DeleteAct += () =>
            {
                ClearPages();
                TelegramLib.MainClasses.Messages.Message mes = _menu.GetMessage();
                if (mes is null) return;

                ((MainWindow)Window.GetWindow(this)).DeleteMessage(mes);
            };
        }

        public void ClearPages()
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private List<MediaAction> _medias = new List<MediaAction>();
        public void SetChatMediaObjs()
        {
            _medias.Clear();
            for (int i = 0; i < _chat.Messages.Count; i++)
            {
                if (_chat.Messages[i] is MediaAction media)
                {
                    _medias.Add(media);
                }
            }
        }

        public void SetOnlyImagesInMedias()
        {
            List<MediaAction> toRemove = new List<MediaAction>();

            for (int i = 0; i < _medias.Count; i++)
            {
                if (_medias[i].IsSticker || !FilesAction.IsFileIsImage(_medias[i].MediaName))
                {
                    toRemove.Add(_medias[i]);
                }
            }

            foreach (var val in toRemove)
            {
                _medias.Remove(val);
            }
        }

        public void MediaImages_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Image img) return;

            int.TryParse(img.Tag.ToString(), out int chosenId);

            Message chosen = _system.GetMessageById(chosenId);

            MediaWindow mediaWindow = new MediaWindow(
                null, (MainWindow)Window.GetWindow(this),
                Enums.MediaShow.MediaShowType.ChatImages, _system);

            mediaWindow.SetChatImageMessages(chosen, _medias);

            //Is exist
            mediaWindow.Show();
        }

        public List<MediaAction> GetImageMessages()
        {
            List<MediaAction> res = new List<MediaAction>();

            for (int i = 0; i < _chat.Messages.Count; i++)
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
            for (int i = 0; i < _chat.Messages.Count; i++)
            {
                if (_chat.Messages[i] is MediaAction media &&
                    FilesAction.IsFileIsImage(media.MediaName))
                {
                    if (media.IsImage() && !media.IsSticker)//!FilesAction.IsUserChatMediaIsExist(media.MediaName)) continue;
                    {
                        string path = FilesAction.GetUserImagePath(media.MediaName);
                        BitmapImage bitmap = ApiService.GetCachedBitmap(media.MediaName);
                        
                        if(bitmap is not null) res.Add(new Image() { Source = bitmap});
                        //UserImage.ImageSource = ApiService.GetCashedBitmap(path) is BitmapImage b and not null ? b : SignalRHelperService.LoadBitmap(path);
                        //res.Add(FilesAction.GetImageFromChatImageFolder(media.MediaName));
                    }
                }
            }
            return res;
        }

        private List<MediaAction> GetGifFileNames()
        {
            List<MediaAction> res = new List<MediaAction>();

            for (int i = 0; i < _chat.Messages.Count; i++)
            {
                if (_chat.Messages[i] is MediaAction media &&
                    media.IsGif()/*
                    !FilesAction.IsGifNameIsExist(media.MediaName)*/)
                {
                    res.Add(media);
                }
            }
            return res;
        }

        public List<string> GetVideoFileNames()
        {
            List<string> res = new List<string>();

            for (int i = 0; i < _chat.Messages.Count; i++)
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
