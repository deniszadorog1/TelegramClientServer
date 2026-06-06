using MaterialDesignThemes.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.UserSettings;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Services;
using Image = System.Windows.Controls.Image;
using Size = System.Windows.Size;

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для ImageMessage.xaml
    /// </summary>
    public partial class MediaMessage : UserControl
    {
        public bool IsSticker { get; }

        public Image _img;
        public MediaElement _media;
        public string _gifPath;
        public string _stickerPath;

        public string _senderImgName;
        private int? _forwardedFrom = null;

        private TelSystem _system;

        private bool _isOnlyVisual = false;
        private MediaAction _message;

        public event Action PushForwarded;

        private List<string> _bandPaths = new List<string>();
        public List<Border> _bandBorders = new List<Border>();
        private List<ImageBrush> _bandBrushes = new List<ImageBrush>();
        public List<Border> _selectBorders = new List<Border>();

        //Send band of medias
        public MediaMessage(TelSystem system, List<string> paths)
        {
            _isOnlyVisual = false;
            _system = system;
            IsSticker = false;

            _bandPaths = paths;

            InitializeComponent();

            HideBordersExceptBand();
            SetBandBorderLists();
            SetMessages();
        }

        public bool IsBandBorderContainsId(int mesId)
        {
            return _bandBorders.Any(x => x.Tag is not null && x.Tag.ToString() == mesId.ToString());
        }

        public bool IsBandMedia() => _bandPaths.Count > 0;

        public async void SetMessages()
        {
            _maxHeight = 0;

            if (_bandPaths.Count == 1)
            {
                BottomBandRow.Height = new GridLength(0);
                DownBandRow.Height = new GridLength(0);
                MiddleBandRow.Height = new GridLength(0);

                SetupImageRow(
                    new List<Border>() { OneInGroupBorder },
                    new List<ImageBrush>() { OneImg },
                    await GetBitImagesByPaths(new List<string>() { _bandPaths[0]}),
                    _maxSize.Width, _maxSize.Height);

                HideBandBorder(TwoInGroupBorder);
                HideBandBorder(ThreeInGroupBorder);
                HideBandBorder(FourInGroupBorder);
                HideBandBorder(FiveInGroupBorder);
                HideBandBorder(SixInGroupBorder);
                HideBandBorder(SevenInGroupBorder);
                HideBandBorder(EightInGroupBorder);
                HideBandBorder(NineInGroupBorder);
                HideBandBorder(TenInGroupBorder);
                HideBandBorder(ElevenInGroupBorder);
                HideBandBorder(TwelveInGroupBorder);
            }
            else if(_bandPaths.Count == 2)
            {
                BottomBandRow.Height = new GridLength(0);
                DownBandRow.Height = new GridLength(0);
                MiddleBandRow.Height = new GridLength(0);

                SetupImageRow(
                    new List<Border>() { OneInGroupBorder, TwoInGroupBorder },
                    new List<ImageBrush>() { OneImg, TwoImg },
                    await GetBitImagesByPaths(new List<string>() { _bandPaths[0], _bandPaths[1] }),
                    _maxSize.Width, _maxSize.Height);

                HideBandBorder(ThreeInGroupBorder);
                HideBandBorder(FourInGroupBorder);
                HideBandBorder(FiveInGroupBorder);
                HideBandBorder(SixInGroupBorder);
                HideBandBorder(SevenInGroupBorder);
                HideBandBorder(EightInGroupBorder);
                HideBandBorder(NineInGroupBorder);
                HideBandBorder(TenInGroupBorder);
                HideBandBorder(ElevenInGroupBorder);
                HideBandBorder(TwelveInGroupBorder);
            }
            else if (_bandPaths.Count == 3)
            {
                BottomBandRow.Height = new GridLength(0);
                DownBandRow.Height = new GridLength(0);

                /*                SetBandImg(OneInGroupBorder, OneImg, _bandPaths[0], _minSize, _maxSize);
                                OneInGroupBorder.Width = _maxSize.Width;
                                OneInGroupBorder.Height = _maxSize.Height;*/

                SetupImageRow(
                    new List<Border>() { OneInGroupBorder },
                    new List<ImageBrush>() { OneImg },
                    await GetBitImagesByPaths(new List<string>() { _bandPaths[0] }),
                    _maxSize.Width, _maxSize.Height);


                _maxHeight = 0;

                //SetBandImg(FourInGroupBorder, FourImg, _bandPaths[1], _minSize, _maxSize);
                //SetBandImg(FiveInGroupBorder, FiveImg, _bandPaths[2], _minSize, _maxSize);

                SetupImageRow(
                    new List<Border>() { FourInGroupBorder, FiveInGroupBorder },
                    new List<ImageBrush>() { FourImg, FiveImg },
                    await GetBitImagesByPaths(new List<string>() { _bandPaths[1], _bandPaths[2] }),
                    _maxSize.Width, _maxSize.Height);

                HideBandBorder(TwoInGroupBorder);
                HideBandBorder(ThreeInGroupBorder);
                HideBandBorder(SixInGroupBorder);
                HideBandBorder(SevenInGroupBorder);
                HideBandBorder(EightInGroupBorder);
                HideBandBorder(NineInGroupBorder);
            }
            else if (_bandPaths.Count == 4)
            {
                BottomBandRow.Height = new GridLength(0);
                DownBandRow.Height = new GridLength(0);

                /*                SetBandImg(OneInGroupBorder, OneImg, _bandPaths[0], _minSize, _maxSize);
                                OneInGroupBorder.Width = _maxSize.Width;
                                OneInGroupBorder.Height = _maxSize.Height;*/

                SetupImageRow(
                    new List<Border>() { OneInGroupBorder },
                    new List<ImageBrush>() { OneImg },
                    await GetBitImagesByPaths(new List<string>() { _bandPaths[0] }),
                    _maxSize.Width, _maxSize.Height);


                _maxHeight = 0;

                //Size thirdPart = new Size(_maxSize.Width / 3, _maxSize.Height / 2);

                //SetBandImg(FourInGroupBorder, FourImg, _bandPaths[1], thirdPart, _maxSize);
                //SetBandImg(FiveInGroupBorder, FiveImg, _bandPaths[2], thirdPart, _maxSize);
                // SetBandImg(SixInGroupBorder, SixImg, _bandPaths[3], thirdPart, _maxSize);

                SetupImageRow(
                    new List<Border>() { FourInGroupBorder, FiveInGroupBorder, SixInGroupBorder },
                    new List<ImageBrush>() { FourImg, FiveImg, SixImg },
                    await GetBitImagesByPaths(new List<string>() { _bandPaths[1], _bandPaths[2], _bandPaths[3] }),
                    _maxSize.Width, _maxSize.Height);

                HideBandBorder(TwoInGroupBorder);
                HideBandBorder(ThreeInGroupBorder);
                HideBandBorder(SevenInGroupBorder);
                HideBandBorder(EightInGroupBorder);
                HideBandBorder(NineInGroupBorder);
            }
            else if (_bandPaths.Count == 5)
            {
                DownBandRow.Height = new GridLength(0);

                /*                SetBandImg(OneInGroupBorder, OneImg, _bandPaths[0], _minSize, _maxSize);
                                OneInGroupBorder.Width = _maxSize.Width;
                                OneInGroupBorder.Height = _maxSize.Height;*/

                SetupImageRow(
                    new List<Border>() { OneInGroupBorder },
                    new List<ImageBrush>() { OneImg },
                    await GetBitImagesByPaths(new List<string>() { _bandPaths[0] }),
                    _maxSize.Width, _maxSize.Height);


                //_maxHeight = 0;

                // Size secondPart = new Size(_maxSize.Width / 2, _maxSize.Height / 2);

                //SetBandImg(FourInGroupBorder, FourImg, _bandPaths[1], secondPart, _maxSize);
                //SetBandImg(FiveInGroupBorder, FiveImg, _bandPaths[2], secondPart, _maxSize);

                SetupImageRow(
                     new List<Border>() { FourInGroupBorder, FiveInGroupBorder },
                     new List<ImageBrush>() { FourImg, FiveImg },
                     await GetBitImagesByPaths(new List<string>() { _bandPaths[1], _bandPaths[2] }),
                     _maxSize.Width, _maxSize.Height);

                //_maxHeight = 0;

                //SetBandImg(SevenInGroupBorder, SevenImg, _bandPaths[3], secondPart, _maxSize);
                //SetBandImg(EightInGroupBorder, EightImg, _bandPaths[4], secondPart, _maxSize);

                SetupImageRow(
                     new List<Border>() { SevenInGroupBorder, EightInGroupBorder },
                     new List<ImageBrush>() { SevenImg, EightImg },
                     await GetBitImagesByPaths(new List<string>() { _bandPaths[3], _bandPaths[4] }),
                     _maxSize.Width, _maxSize.Height);


                HideBandBorder(TwoInGroupBorder);
                HideBandBorder(ThreeInGroupBorder);
                HideBandBorder(SixInGroupBorder);
                HideBandBorder(NineInGroupBorder);
            }
            else if (_bandPaths.Count == 6)
            {
                DownBandRow.Height = new GridLength(0);

                //Size secondPart = new Size(_maxSize.Width / 2, _maxSize.Height / 2);

                //SetBandImg(OneInGroupBorder, OneImg, _bandPaths[0], secondPart, _maxSize);
                //SetBandImg(TwoInGroupBorder, TwoImg, _bandPaths[1], secondPart, _maxSize);

                SetupImageRow(
                     new List<Border>() { OneInGroupBorder, TwoInGroupBorder },
                     new List<ImageBrush>() { OneImg, TwoImg },
                     await GetBitImagesByPaths(new List<string>() { _bandPaths[0], _bandPaths[1] }),
                     _maxSize.Width, _maxSize.Height);

                //_maxHeight = 0;

                //SetBandImg(FourInGroupBorder, FourImg, _bandPaths[2], secondPart, _maxSize);
                //SetBandImg(FiveInGroupBorder, FiveImg, _bandPaths[3], secondPart, _maxSize);
                SetupImageRow(
                     new List<Border>() { FourInGroupBorder, FiveInGroupBorder },
                     new List<ImageBrush>() { FourImg, FiveImg },
                     await GetBitImagesByPaths(new List<string>() { _bandPaths[2], _bandPaths[3] }),
                     _maxSize.Width, _maxSize.Height);

                //_maxHeight = 0;
                //SetBandImg(SevenInGroupBorder, SevenImg, _bandPaths[4], secondPart, _maxSize);
                //SetBandImg(EightInGroupBorder, EightImg, _bandPaths[5], secondPart, _maxSize);

                SetupImageRow(
                      new List<Border>() { SevenInGroupBorder, EightInGroupBorder },
                      new List<ImageBrush>() { SevenImg, EightImg },
                      await GetBitImagesByPaths(new List<string>() { _bandPaths[4], _bandPaths[5] }),
                      _maxSize.Width, _maxSize.Height);


                HideBandBorder(ThreeInGroupBorder);
                HideBandBorder(SixInGroupBorder);
                HideBandBorder(NineInGroupBorder);
            }
            else if (_bandPaths.Count == 7)
            {
                DownBandRow.Height = new GridLength(0);

                Size basePart = new Size(_maxSize.Width, _maxSize.Height);


                SetupImageRow(
                     new List<Border>() { OneInGroupBorder, TwoInGroupBorder, ThreeInGroupBorder },
                     new List<ImageBrush>() { OneImg, TwoImg, ThreeImg },
                     await GetBitImagesByPaths(new List<string>() { _bandPaths[0], _bandPaths[1], _bandPaths[2] }),
                     _maxSize.Width, _maxSize.Height);

                SetupImageRow(
                      new List<Border>() { FourInGroupBorder },
                      new List<ImageBrush>() { FourImg },
                      await GetBitImagesByPaths(new List<string>() { _bandPaths[3] }),
                      _maxSize.Width, _maxSize.Height);

                //SetBandImg(FourInGroupBorder, FourImg, _bandPaths[3], basePart, _maxSize);

                SetupImageRow(
                      new List<Border>() { SevenInGroupBorder, EightInGroupBorder, NineInGroupBorder },
                      new List<ImageBrush>() { SevenImg, EightImg, NineImg },
                      await GetBitImagesByPaths(new List<string>() { _bandPaths[4], _bandPaths[5], _bandPaths[6] }),
                      _maxSize.Width, _maxSize.Height);

                HideBandBorder(FiveInGroupBorder);
                HideBandBorder(SixInGroupBorder);
            }
            else if (_bandPaths.Count == 8)
            {
                DownBandRow.Height = new GridLength(0);

                //Size thirdPart = new Size(_maxSize.Width / 3, _maxSize.Height / 2);
                //Size secondPart = new Size(_maxSize.Width / 2, _maxSize.Height);

                //SetBandImg(OneInGroupBorder, OneImg, _bandPaths[0], thirdPart, _maxSize);
                //SetBandImg(TwoInGroupBorder, TwoImg, _bandPaths[1], thirdPart, _maxSize);
                //SetBandImg(ThreeInGroupBorder, ThreeImg, _bandPaths[2], thirdPart, _maxSize);

                SetupImageRow(
                     new List<Border>() { OneInGroupBorder, TwoInGroupBorder, ThreeInGroupBorder },
                     new List<ImageBrush>() { OneImg, TwoImg, ThreeImg },
                     await GetBitImagesByPaths(new List<string>() { _bandPaths[0], _bandPaths[1], _bandPaths[2] }),
                     _maxSize.Width, _maxSize.Height);

                //_maxHeight = 0;

                //SetBandImg(FourInGroupBorder, FourImg, _bandPaths[3], secondPart, _maxSize);
                //SetBandImg(FiveInGroupBorder, FiveImg, _bandPaths[4], secondPart, _maxSize);

                SetupImageRow(
                      new List<Border>() { FourInGroupBorder, FiveInGroupBorder },
                      new List<ImageBrush>() { FourImg, FiveImg },
                      await GetBitImagesByPaths(new List<string>() { _bandPaths[3], _bandPaths[4] }),
                      _maxSize.Width, _maxSize.Height);


                //_maxHeight = 0;

                //SetBandImg(SevenInGroupBorder, SevenImg, _bandPaths[5], thirdPart, _maxSize);
                //SetBandImg(EightInGroupBorder, EightImg, _bandPaths[6], thirdPart, _maxSize);
                //SetBandImg(NineInGroupBorder, NineImg, _bandPaths[7], thirdPart, _maxSize);

                SetupImageRow(
                      new List<Border>() { SevenInGroupBorder, EightInGroupBorder, NineInGroupBorder },
                      new List<ImageBrush>() { SevenImg, EightImg, NineImg },
                      await GetBitImagesByPaths(new List<string>() { _bandPaths[5], _bandPaths[6], _bandPaths[7] }),
                      _maxSize.Width, _maxSize.Height);

                HideBandBorder(SixInGroupBorder);
            }
            else if (_bandPaths.Count == 9)
            {
                //Size secondPart = new Size(_maxSize.Width / 2, _maxSize.Height / 3);
                //Size thirdPart = new Size(_maxSize.Width / 3, _maxSize.Height / 2);

                //SetBandImg(OneInGroupBorder, OneImg, _bandPaths[0], _maxSize, _maxSize);

                SetupImageRow(
                  new List<Border>() { OneInGroupBorder },
                  new List<ImageBrush>() { OneImg },
                  await GetBitImagesByPaths(new List<string>() { _bandPaths[0] }),
                  _maxSize.Width, _maxSize.Height);

                //_maxHeight = 0;
                //SetBandImg(FourInGroupBorder, FourImg, _bandPaths[1], thirdPart, _maxSize);
                //SetBandImg(FiveInGroupBorder, FiveImg, _bandPaths[2], thirdPart, _maxSize);
                //SetBandImg(SixInGroupBorder, SixImg, _bandPaths[3], thirdPart, _maxSize);

                SetupImageRow(
                      new List<Border>() { FourInGroupBorder, FiveInGroupBorder, SixInGroupBorder },
                      new List<ImageBrush>() { FourImg, FiveImg, SixImg },
                      await GetBitImagesByPaths(new List<string>() { _bandPaths[1], _bandPaths[2], _bandPaths[3] }),
                      _maxSize.Width, _maxSize.Height);

                //_maxHeight = 0;

                //SetBandImg(SevenInGroupBorder, SevenImg, _bandPaths[4], thirdPart, _maxSize);
                //SetBandImg(EightInGroupBorder, EightImg, _bandPaths[5], thirdPart, _maxSize);
                //SetBandImg(NineInGroupBorder, NineImg, _bandPaths[6], thirdPart, _maxSize);

                SetupImageRow(
                      new List<Border>() { SevenInGroupBorder, EightInGroupBorder, NineInGroupBorder },
                      new List<ImageBrush>() { SevenImg, EightImg, NineImg },
                      await GetBitImagesByPaths(new List<string>() { _bandPaths[4], _bandPaths[5], _bandPaths[6] }),
                      _maxSize.Width, _maxSize.Height);

                //_maxHeight = 0;
                //SetBandImg(TenInGroupBorder, NineImg, _bandPaths[7], secondPart, _maxSize);
                //SetBandImg(ElevenInGroupBorder, NineImg, _bandPaths[8], secondPart, _maxSize);

                SetupImageRow(
                      new List<Border>() { TenInGroupBorder, ElevenInGroupBorder },
                      new List<ImageBrush>() { TenImg, ElevenImg },
                      await GetBitImagesByPaths(new List<string>() { _bandPaths[7], _bandPaths[8] }),
                      _maxSize.Width, _maxSize.Height);

            }
            else
            {
                DownBandRow.Height = new GridLength(0);

                if (_bandPaths.Count <= 6) BottomBandRow.Height = new GridLength(0);
                if (_bandPaths.Count <= 3) MiddleBandRow.Height = new GridLength(0);

                for (int i = 0; i < _bandPaths.Count; i++)
                {
                    SetupImageRow(
                      new List<Border>() { _bandBorders[i] },
                      new List<ImageBrush>() { _bandBrushes[i] },
                      await GetBitImagesByPaths(new List<string>() { _bandPaths[i] }),
                      _maxSize.Width, _maxSize.Height);

                    //SetBandImg(_bandBorders[i], _bandBrushes[i], _bandPaths[i], _minSize, _maxSize);
                }

                for (int i = _bandPaths.Count; i < _bandBorders.Count; i++)
                {
                    HideBandBorder(_bandBorders[i]);
                }
            }
        }

        public async Task<List<BitmapImage>> GetBitImagesByPaths(List<string> paths)
        {
            List<BitmapImage> res = new List<BitmapImage>();

            foreach (var path in paths)
            {
                string newPath = FilesAction.GetPathByName(Path.GetFileName(path));
                if (newPath is null || newPath == string.Empty) continue;

                //Is Exist 
                BitmapImage bitmap = ApiService.GetCachedBitmap(newPath);
                if (bitmap is not null)
                {
                    res.Add(bitmap);
                    continue;
                }

                string tempPath = System.IO.Path.GetFileName(newPath);
                string videoExt = System.IO.Path.GetExtension(tempPath);

                if (videoExt == ".mp4" || videoExt == ".amv")
                {
                    Image videoFrame = await /*FilesAction.GetImagePreviewForVideo(tempPath);// */VisHelper.GetFirstFrameAsync(tempPath);
                    if (videoFrame is null) continue;

                    if (videoFrame.Source is BitmapImage bitImg)
                    {
                        ApiService.AddCashParams(newPath, bitImg);
                        res.Add(bitImg);
                    }
                    continue;
                }

                bitmap = SignalRHelperService.LoadBitmap(newPath);

                if (bitmap is not null)
                {
                    ApiService.AddCashParams(newPath, bitmap);
                    res.Add(bitmap);
                }
            }

            return res;
        }

        public void HideBandBorder(Border border)
        {
            border.Width = 0;
            border.Height = 0;
            border.Visibility = Visibility.Hidden;
        }

        public string GetImageBorderSource(int messageId)
        {
            for (int i = 0; i < _bandBorders.Count; i++)
            {
                if (_bandBorders[i].Tag is not null &&
                    _bandBorders[i].Tag.ToString() == messageId.ToString())
                {
                    string res = System.IO.Path.GetFileName(_bandPaths[i]);
                    return FilesAction.GetPathByName(res);// FilesAction.GetFullChatImagePath(res);
                }
            }
            return string.Empty;
        }

        private double _maxHeight = 0;

        Size _minSize = new Size(150, 150);
        Size _maxSize = new Size(300, 250);

        public async void SetBandImg(Border border, ImageBrush brush,
            string path, Size minSize, Size maxSize)
        {
            const Stretch strech = Stretch.Fill;

            path = System.IO.Path.GetFileName(path);
            string videoExt = System.IO.Path.GetExtension(path);

            if (videoExt == ".mp4" || videoExt == ".amv")
            {
                Image videoFrame = await VisHelper.GetFirstFrameAsync(path);
                if (videoFrame is null) return;

                brush.ImageSource = videoFrame.Source;

                border.Visibility = Visibility.Visible;
                brush.Stretch = strech;

                border.Width = minSize.Width;
                border.Height = minSize.Height;

                return;
            }

            border.Visibility = Visibility.Visible;
            brush.Stretch = strech;

            BitmapImage img = ApiService.GetCachedBitmap(path);
            string fullPath = FilesAction.GetFullChatImagePath(Path.GetFileName(path)); //FilesAction.GetFullChatImagePath(path);


            BitmapImage bitMap = img is not null ? img : SignalRHelperService.LoadBitmap(fullPath);//  new BitmapImage(new Uri(fullPath, UriKind.Absolute));


            double width = bitMap.Width / 2;
            double height = bitMap.Height / 2;

            border.Width = width > minSize.Width && width < maxSize.Width ? width : minSize.Width;
            border.Height = height > minSize.Height && height < maxSize.Height ? height : minSize.Height;



            if (_maxHeight == 0) _maxHeight = border.Height;
            else border.Height = _maxHeight;


            brush.ImageSource = bitMap;
        }

        public void SetupImageRow(List<Border> borders, List<ImageBrush> brushes, List<BitmapImage> images, double maxWidth, double maxHeight)
        {
            if (borders == null || images == null || borders.Count != images.Count || borders.Count == 0)
                return;

            double minHeight = 60; 

            double totalAspectRatio = 0;
            for (int i = 0; i < images.Count; i++)
            {
                totalAspectRatio += (double)images[i].PixelWidth / images[i].PixelHeight;
            }
            double finalHeight = maxWidth / totalAspectRatio;

            finalHeight = Math.Max(minHeight, Math.Min(finalHeight, maxHeight));

            double widthPerAspectUnit = maxWidth / totalAspectRatio;

            double currentTotalWidth = 0;

            for (int i = 0; i < borders.Count; i++)
            {
                var border = borders[i];
                var img = images[i];
                var brush = brushes[i];

                double aspect = (double)img.PixelWidth / img.PixelHeight;
                border.Height = finalHeight;
                double targetWidth = widthPerAspectUnit * aspect;

                if (i == borders.Count - 1)
                {
                    border.Width = maxWidth - currentTotalWidth;
                }
                else
                {
                    border.Width = targetWidth;
                    currentTotalWidth += targetWidth;
                }

                border.Margin = new Thickness(0);

                brush.ImageSource = img;

                brush.Stretch = Stretch.UniformToFill; 
            }
        }

        private void SetBandBorderLists()
        {
            _bandBorders.Clear();
            _bandBrushes.Clear();
            _selectBorders.Clear();

            if (_bandPaths.Count == 2)
            {
                #region //Count two 
                _bandBorders.Add(OneInGroupBorder);
                _bandBorders.Add(TwoInGroupBorder);
                _bandBorders.Add(ThreeInGroupBorder);
                _bandBorders.Add(FourInGroupBorder);
                _bandBorders.Add(FiveInGroupBorder);
                _bandBorders.Add(SixInGroupBorder);
                _bandBorders.Add(SevenInGroupBorder);
                _bandBorders.Add(EightInGroupBorder);
                _bandBorders.Add(NineInGroupBorder);
                _bandBorders.Add(TenInGroupBorder);
                _bandBorders.Add(ElevenInGroupBorder);
                _bandBorders.Add(TwelveInGroupBorder);

                _bandBrushes.Add(OneImg);
                _bandBrushes.Add(TwoImg);
                _bandBrushes.Add(ThreeImg);
                _bandBrushes.Add(FourImg);
                _bandBrushes.Add(FiveImg);
                _bandBrushes.Add(SixImg);
                _bandBrushes.Add(SevenImg);
                _bandBrushes.Add(EightImg);
                _bandBrushes.Add(NineImg);
                _bandBrushes.Add(TenImg);
                _bandBrushes.Add(ElevenImg);
                _bandBrushes.Add(TwelveImg);

                _selectBorders.Add(OneSelectedBorder);
                _selectBorders.Add(TwoSelectedBorder);
                _selectBorders.Add(ThreeSelectedBorder);
                _selectBorders.Add(FourSelectedBorder);
                _selectBorders.Add(FiveSelectedBorder);
                _selectBorders.Add(SixSelectedBorder);
                _selectBorders.Add(SevenSelectedBorder);
                _selectBorders.Add(EightSelectedBorder);
                _selectBorders.Add(NineSelectedBorder);
                _selectBorders.Add(TenSelectedBorder);
                _selectBorders.Add(ElevenSelectedBorder);
                _selectBorders.Add(TwelveSelectedBorder);

                #endregion
            }
            else if (_bandPaths.Count == 3)
            {
                #region //Count three
                _bandBorders.Add(OneInGroupBorder);
                _bandBorders.Add(FourInGroupBorder);
                _bandBorders.Add(FiveInGroupBorder);

                _bandBorders.Add(TwoInGroupBorder);
                _bandBorders.Add(ThreeInGroupBorder);
                _bandBorders.Add(SixInGroupBorder);
                _bandBorders.Add(SevenInGroupBorder);
                _bandBorders.Add(EightInGroupBorder);
                _bandBorders.Add(NineInGroupBorder);
                _bandBorders.Add(TenInGroupBorder);
                _bandBorders.Add(ElevenInGroupBorder);
                _bandBorders.Add(TwelveInGroupBorder);


                _bandBrushes.Add(OneImg);
                _bandBrushes.Add(FourImg);
                _bandBrushes.Add(FiveImg);

                _bandBrushes.Add(TwoImg);
                _bandBrushes.Add(ThreeImg);
                _bandBrushes.Add(SixImg);
                _bandBrushes.Add(SevenImg);
                _bandBrushes.Add(EightImg);
                _bandBrushes.Add(NineImg);
                _bandBrushes.Add(TenImg);
                _bandBrushes.Add(ElevenImg);
                _bandBrushes.Add(TwelveImg);

                _selectBorders.Add(OneSelectedBorder);
                _selectBorders.Add(FourSelectedBorder);
                _selectBorders.Add(FiveSelectedBorder);

                _selectBorders.Add(TwoSelectedBorder);
                _selectBorders.Add(ThreeSelectedBorder);
                _selectBorders.Add(SixSelectedBorder);
                _selectBorders.Add(SevenSelectedBorder);
                _selectBorders.Add(EightSelectedBorder);
                _selectBorders.Add(NineSelectedBorder);
                _selectBorders.Add(TenSelectedBorder);
                _selectBorders.Add(ElevenSelectedBorder);
                _selectBorders.Add(TwelveSelectedBorder);

                #endregion
            }
            else if (_bandPaths.Count == 4)
            {
                #region //Count four
                _bandBorders.Add(OneInGroupBorder);
                _bandBorders.Add(FourInGroupBorder);
                _bandBorders.Add(FiveInGroupBorder);
                _bandBorders.Add(SixInGroupBorder);

                _bandBorders.Add(TwoInGroupBorder);
                _bandBorders.Add(ThreeInGroupBorder);
                _bandBorders.Add(SevenInGroupBorder);
                _bandBorders.Add(EightInGroupBorder);
                _bandBorders.Add(NineInGroupBorder);
                _bandBorders.Add(TenInGroupBorder);
                _bandBorders.Add(ElevenInGroupBorder);
                _bandBorders.Add(TwelveInGroupBorder);

                _bandBrushes.Add(OneImg);
                _bandBrushes.Add(FourImg);
                _bandBrushes.Add(FiveImg);
                _bandBrushes.Add(SixImg);

                _bandBrushes.Add(TwoImg);
                _bandBrushes.Add(ThreeImg);
                _bandBrushes.Add(SevenImg);
                _bandBrushes.Add(EightImg);
                _bandBrushes.Add(NineImg);
                _bandBrushes.Add(TenImg);
                _bandBrushes.Add(ElevenImg);
                _bandBrushes.Add(TwelveImg);

                _selectBorders.Add(OneSelectedBorder);
                _selectBorders.Add(FourSelectedBorder);
                _selectBorders.Add(FiveSelectedBorder);
                _selectBorders.Add(SixSelectedBorder);

                _selectBorders.Add(TwoSelectedBorder);
                _selectBorders.Add(ThreeSelectedBorder);
                _selectBorders.Add(SevenSelectedBorder);
                _selectBorders.Add(EightSelectedBorder);
                _selectBorders.Add(NineSelectedBorder);
                _selectBorders.Add(TenSelectedBorder);
                _selectBorders.Add(ElevenSelectedBorder);
                _selectBorders.Add(TwelveSelectedBorder);

                #endregion
            }
            else if (_bandPaths.Count == 5)
            {
                #region //Count five
                _bandBorders.Add(OneInGroupBorder);
                _bandBorders.Add(FourInGroupBorder);
                _bandBorders.Add(FiveInGroupBorder);
                _bandBorders.Add(SevenInGroupBorder);
                _bandBorders.Add(EightInGroupBorder);

                _bandBorders.Add(TwoInGroupBorder);
                _bandBorders.Add(ThreeInGroupBorder);
                _bandBorders.Add(SixInGroupBorder);
                _bandBorders.Add(NineInGroupBorder);
                _bandBorders.Add(TenInGroupBorder);
                _bandBorders.Add(ElevenInGroupBorder);
                _bandBorders.Add(TwelveInGroupBorder);

                _bandBrushes.Add(OneImg);
                _bandBrushes.Add(FourImg);
                _bandBrushes.Add(FiveImg);
                _bandBrushes.Add(SevenImg);
                _bandBrushes.Add(EightImg);

                _bandBrushes.Add(TwoImg);
                _bandBrushes.Add(ThreeImg);
                _bandBrushes.Add(SixImg);
                _bandBrushes.Add(NineImg);
                _bandBrushes.Add(TenImg);
                _bandBrushes.Add(ElevenImg);
                _bandBrushes.Add(TwelveImg);

                _selectBorders.Add(OneSelectedBorder);
                _selectBorders.Add(FourSelectedBorder);
                _selectBorders.Add(FiveSelectedBorder);
                _selectBorders.Add(SevenSelectedBorder);
                _selectBorders.Add(EightSelectedBorder);

                _selectBorders.Add(TwoSelectedBorder);
                _selectBorders.Add(ThreeSelectedBorder);
                _selectBorders.Add(SixSelectedBorder);
                _selectBorders.Add(NineSelectedBorder);
                _selectBorders.Add(TenSelectedBorder);
                _selectBorders.Add(ElevenSelectedBorder);
                _selectBorders.Add(TwelveSelectedBorder);
                #endregion
            }
            else if (_bandPaths.Count == 6)
            {
                #region //Count six
                _bandBorders.Add(OneInGroupBorder);
                _bandBorders.Add(TwoInGroupBorder);
                _bandBorders.Add(FourInGroupBorder);
                _bandBorders.Add(FiveInGroupBorder);
                _bandBorders.Add(SevenInGroupBorder);
                _bandBorders.Add(EightInGroupBorder);

                _bandBorders.Add(ThreeInGroupBorder);
                _bandBorders.Add(SixInGroupBorder);
                _bandBorders.Add(NineInGroupBorder);
                _bandBorders.Add(TenInGroupBorder);
                _bandBorders.Add(ElevenInGroupBorder);
                _bandBorders.Add(TwelveInGroupBorder);

                _bandBrushes.Add(OneImg);
                _bandBrushes.Add(TwoImg);
                _bandBrushes.Add(FourImg);
                _bandBrushes.Add(FiveImg);
                _bandBrushes.Add(SevenImg);
                _bandBrushes.Add(EightImg);

                _bandBrushes.Add(ThreeImg);
                _bandBrushes.Add(SixImg);
                _bandBrushes.Add(NineImg);
                _bandBrushes.Add(TenImg);
                _bandBrushes.Add(ElevenImg);
                _bandBrushes.Add(TwelveImg);

                _selectBorders.Add(OneSelectedBorder);
                _selectBorders.Add(TwoSelectedBorder);
                _selectBorders.Add(FourSelectedBorder);
                _selectBorders.Add(FiveSelectedBorder);
                _selectBorders.Add(SevenSelectedBorder);
                _selectBorders.Add(EightSelectedBorder);

                _selectBorders.Add(ThreeSelectedBorder);
                _selectBorders.Add(SixSelectedBorder);
                _selectBorders.Add(NineSelectedBorder);
                _selectBorders.Add(TenSelectedBorder);
                _selectBorders.Add(ElevenSelectedBorder);
                _selectBorders.Add(TwelveSelectedBorder);
                #endregion
            }
            else if (_bandPaths.Count == 7)
            {
                #region //Count seven
                _bandBorders.Add(OneInGroupBorder);
                _bandBorders.Add(TwoInGroupBorder);
                _bandBorders.Add(ThreeInGroupBorder);
                _bandBorders.Add(FourInGroupBorder);
                _bandBorders.Add(SevenInGroupBorder);
                _bandBorders.Add(EightInGroupBorder);
                _bandBorders.Add(NineInGroupBorder);

                _bandBorders.Add(FiveInGroupBorder);
                _bandBorders.Add(SixInGroupBorder);
                _bandBorders.Add(TenInGroupBorder);
                _bandBorders.Add(ElevenInGroupBorder);
                _bandBorders.Add(TwelveInGroupBorder);

                _bandBrushes.Add(OneImg);
                _bandBrushes.Add(TwoImg);
                _bandBrushes.Add(ThreeImg);
                _bandBrushes.Add(FourImg);
                _bandBrushes.Add(SevenImg);
                _bandBrushes.Add(EightImg);
                _bandBrushes.Add(NineImg);

                _bandBrushes.Add(FiveImg);
                _bandBrushes.Add(SixImg);
                _bandBrushes.Add(TenImg);
                _bandBrushes.Add(ElevenImg);
                _bandBrushes.Add(TwelveImg);

                _selectBorders.Add(OneSelectedBorder);
                _selectBorders.Add(TwoSelectedBorder);
                _selectBorders.Add(ThreeSelectedBorder);
                _selectBorders.Add(FourSelectedBorder);
                _selectBorders.Add(SevenSelectedBorder);
                _selectBorders.Add(EightSelectedBorder);
                _selectBorders.Add(NineSelectedBorder);

                _selectBorders.Add(FiveSelectedBorder);
                _selectBorders.Add(SixSelectedBorder);
                _selectBorders.Add(TenSelectedBorder);
                _selectBorders.Add(ElevenSelectedBorder);
                _selectBorders.Add(TwelveSelectedBorder);
                #endregion
            }
            else if (_bandPaths.Count == 8)
            {
                #region //Count eight
                _bandBorders.Add(OneInGroupBorder);
                _bandBorders.Add(TwoInGroupBorder);
                _bandBorders.Add(ThreeInGroupBorder);
                _bandBorders.Add(FourInGroupBorder);
                _bandBorders.Add(FiveInGroupBorder);
                _bandBorders.Add(SevenInGroupBorder);
                _bandBorders.Add(EightInGroupBorder);
                _bandBorders.Add(NineInGroupBorder);

                _bandBorders.Add(SixInGroupBorder);
                _bandBorders.Add(TenInGroupBorder);
                _bandBorders.Add(ElevenInGroupBorder);
                _bandBorders.Add(TwelveInGroupBorder);

                _bandBrushes.Add(OneImg);
                _bandBrushes.Add(TwoImg);
                _bandBrushes.Add(ThreeImg);
                _bandBrushes.Add(FourImg);
                _bandBrushes.Add(FiveImg);
                _bandBrushes.Add(SevenImg);
                _bandBrushes.Add(EightImg);
                _bandBrushes.Add(NineImg);

                _bandBrushes.Add(SixImg);
                _bandBrushes.Add(TenImg);
                _bandBrushes.Add(ElevenImg);
                _bandBrushes.Add(TwelveImg);

                _selectBorders.Add(OneSelectedBorder);
                _selectBorders.Add(TwoSelectedBorder);
                _selectBorders.Add(ThreeSelectedBorder);
                _selectBorders.Add(FourSelectedBorder);
                _selectBorders.Add(FiveSelectedBorder);
                _selectBorders.Add(SevenSelectedBorder);
                _selectBorders.Add(EightSelectedBorder);
                _selectBorders.Add(NineSelectedBorder);

                _selectBorders.Add(SixSelectedBorder);
                _selectBorders.Add(TenSelectedBorder);
                _selectBorders.Add(ElevenSelectedBorder);
                _selectBorders.Add(TwelveSelectedBorder);
                #endregion
            }
            else if (_bandPaths.Count == 9)
            {
                #region //Count base
                _bandBorders.Add(OneInGroupBorder);

                _bandBorders.Add(FourInGroupBorder);
                _bandBorders.Add(FiveInGroupBorder);
                _bandBorders.Add(SixInGroupBorder);
                _bandBorders.Add(SevenInGroupBorder);
                _bandBorders.Add(EightInGroupBorder);
                _bandBorders.Add(NineInGroupBorder);
                _bandBorders.Add(TenInGroupBorder);
                _bandBorders.Add(ElevenInGroupBorder);

                _bandBorders.Add(TwoInGroupBorder);
                _bandBorders.Add(ThreeInGroupBorder);
                _bandBorders.Add(TwelveInGroupBorder);

                _bandBrushes.Add(OneImg);
                _bandBrushes.Add(FourImg);
                _bandBrushes.Add(FiveImg);
                _bandBrushes.Add(SixImg);
                _bandBrushes.Add(SevenImg);
                _bandBrushes.Add(EightImg);
                _bandBrushes.Add(NineImg);
                _bandBrushes.Add(TenImg);
                _bandBrushes.Add(ElevenImg);

                _bandBrushes.Add(TwoImg);
                _bandBrushes.Add(ThreeImg);
                _bandBrushes.Add(TwelveImg);

                _selectBorders.Add(OneSelectedBorder);
                _selectBorders.Add(FourSelectedBorder);
                _selectBorders.Add(FiveSelectedBorder);
                _selectBorders.Add(SixSelectedBorder);
                _selectBorders.Add(SevenSelectedBorder);
                _selectBorders.Add(EightSelectedBorder);
                _selectBorders.Add(NineSelectedBorder);
                _selectBorders.Add(TenSelectedBorder);
                _selectBorders.Add(ElevenSelectedBorder);

                _selectBorders.Add(TwoSelectedBorder);
                _selectBorders.Add(ThreeSelectedBorder);
                _selectBorders.Add(TwelveSelectedBorder);

                #endregion
            }
            else
            {
                #region //Count base
                _bandBorders.Add(OneInGroupBorder);
                _bandBorders.Add(TwoInGroupBorder);
                _bandBorders.Add(ThreeInGroupBorder);
                _bandBorders.Add(FourInGroupBorder);
                _bandBorders.Add(FiveInGroupBorder);
                _bandBorders.Add(SixInGroupBorder);
                _bandBorders.Add(SevenInGroupBorder);
                _bandBorders.Add(EightInGroupBorder);
                _bandBorders.Add(NineInGroupBorder);
                _bandBorders.Add(TenInGroupBorder);
                _bandBorders.Add(ElevenInGroupBorder);
                _bandBorders.Add(TwelveInGroupBorder);

                _bandBrushes.Add(OneImg);
                _bandBrushes.Add(TwoImg);
                _bandBrushes.Add(ThreeImg);
                _bandBrushes.Add(FourImg);
                _bandBrushes.Add(FiveImg);
                _bandBrushes.Add(SixImg);
                _bandBrushes.Add(SevenImg);
                _bandBrushes.Add(EightImg);
                _bandBrushes.Add(NineImg);
                _bandBrushes.Add(TenImg);
                _bandBrushes.Add(ElevenImg);
                _bandBrushes.Add(TwelveImg);

                _selectBorders.Add(OneSelectedBorder);
                _selectBorders.Add(TwoSelectedBorder);
                _selectBorders.Add(ThreeSelectedBorder);
                _selectBorders.Add(FourSelectedBorder);
                _selectBorders.Add(FiveSelectedBorder);
                _selectBorders.Add(SixSelectedBorder);
                _selectBorders.Add(SevenSelectedBorder);
                _selectBorders.Add(EightSelectedBorder);
                _selectBorders.Add(NineSelectedBorder);
                _selectBorders.Add(TenSelectedBorder);
                _selectBorders.Add(ElevenSelectedBorder);
                _selectBorders.Add(TwelveSelectedBorder);
                #endregion
            }

        }

        public void SetTagIdsToBandBorders(List<MediaAction> medias)
        {
            for (int i = 0; i < medias.Count; i++)
            {
                _bandBorders[i].Tag = medias[i].Id;
                _selectBorders[i].Tag = medias[i].Id;
            }
        }

        public List<int> GetBandMessagesIds()
        {
            List<int> res = new List<int>();

            for (int i = 0; i < _bandBorders.Count; i++)
            {
                if (_bandBorders[i] is not null && _bandBorders[i].Tag is not null &&
                    int.TryParse(_bandBorders[i].Tag.ToString(), out int id))
                {
                    res.Add(id);
                }
            }
            return res;
        }

        private void HideBordersExceptBand()
        {
            GifBorder.Visibility = Visibility.Hidden;
            MyVideoPlayer.Visibility = Visibility.Hidden;
            ImageBorder.Visibility = Visibility.Hidden;

            ImgGroupBorder.Visibility = Visibility.Visible;
        }

        public MediaMessage(TelSystem system, MediaAction media)
        {
            _isOnlyVisual = true;
            _message = media;

            _system = system;
            IsSticker = media.IsSticker;
            _forwardedFrom = media.ForwardedFromId;
            //_senderImgName = media.SenderUserId;

            InitializeComponent();
            HideAllBorders();
            SetMedia();

            SetTime(media.SentTime);
            SetForwardedFromRow();
        }

        public async ValueTask SetMedia()
        {
            if (_message.IsImage())
            {
                /*ImgMessage.ImageSource =
                   new BitmapImage(new Uri(
                   FilesAction.GetFullChatImagePath(_message.MediaName),
                   UriKind.Absolute));*/

                string fullPath = FilesAction.GetPathByName(_message.MediaName);

                BitmapImage bitmap = ApiService.GetCachedBitmap(_message.MediaName);

                ImgMessage.ImageSource = bitmap is not null ? bitmap :
                    SignalRHelperService.LoadBitmap(fullPath);
                    
/*                    new BitmapImage(new Uri(
                        FilesAction.GetPathByName(_message.MediaName),
                        UriKind.Absolute));*/


                //SetImgMessageSize(_img, ImageBorder);
                ImageBorder.Visibility = Visibility.Visible;
            }
            else if (_message.IsVideo())
            {
                //string name = System.IO.Path.GetFileName(_message.MediaName);
                string name = FilesAction.GetPathByName(_message.MediaName);

                Image img = await VisHelper.GetFirstFrameAsync(name);
                if (img is null) return;

                GifBorder.Visibility = Visibility.Visible;
                GifImage.Source = img.Source;
            }
            else if (_message.IsGif())
            {
                string name = System.IO.Path.GetFileName(_message.MediaName);
                //string gifPath = FilesAction.GetFullGifPath(name);

                string gifPath = FilesAction.GetPathByName(_message.MediaName);

                BitmapSource source = FilesAction.GetFirstImageFromGif(gifPath);
                if (source is null) return;

                GifBorder.Tag = name;
                GifBorder.Visibility = Visibility.Visible;
                GifImage.Source = source;
            }
        }

        public MediaMessage(TelSystem system,
            System.Windows.Controls.Image img, bool isSticker,
            string senderImgName, DateTime sendTime,
            int? forwardedFromId = null)
        {
            _img = img;
            IsSticker = isSticker;
            _senderImgName = senderImgName;
            _forwardedFrom = forwardedFromId;
            _system = system;

            InitializeComponent();
            ImgMessage.ImageSource = _img.Source;

            SetImgMessageSize(_img, ImageBorder);

            HideAllBorders();
            ImageBorder.Visibility = Visibility.Visible;
            SetSenderImage();

            SetTime(sendTime);

            SetTickEvent();
            SetForwardedFromRow();
        }

        private const int _minMediaSize = 205;
        private const int _maxMediaSize = 225;

        public void SetImgMessageSize(Image img, Border border, bool isVideo = false)
        {
            if (isVideo) return;

            if(!ScaleVideo(img, border))
            {
                border.Height = 150;
                border.Width = 225;
            }

            return;

            if (img.Source is not BitmapImage bitmap) return;

            border.Width = bitmap.PixelWidth;
            border.Height = bitmap.PixelHeight;

            if (border.Width < _minMediaSize) border.Width = _minMediaSize;
            if (border.Width > _maxMediaSize) border.Width = _maxMediaSize;

            if (border.Height < _minMediaSize) border.Height = _minMediaSize;
            if (border.Height > _maxMediaSize) border.Height = _maxMediaSize;
        }

        public bool ScaleVideo(Image img, Border border)
        {
            if (img.Source is not BitmapImage bitmap) return false;

            double w = bitmap.PixelWidth;
            double h = bitmap.PixelHeight;

            double longestSide = Math.Max(w, h);
            if (longestSide > _maxMediaSize)
            {
                double ratio = _maxMediaSize / longestSide;
                w *= ratio;
                h *= ratio;
            }

            double shortestSide = Math.Min(w, h);
            if (shortestSide < _minMediaSize)
            {
                double ratio = _minMediaSize / shortestSide;
                w *= ratio;
                h *= ratio;
            }

            border.Width = w;
            border.Height = h;

            return true;
        }

        public MediaMessage(TelSystem system, string gifPath,
            string senderImgName, DateTime sentTime,
            int? forwardedFromId)
        {
            _system = system;
            _gifPath = gifPath;
            _senderImgName = senderImgName;
            _forwardedFrom = forwardedFromId;

            InitializeComponent();

            HideAllBorders();
            GifBorder.Visibility = Visibility.Visible;

            SetGif(gifPath);
            SetSenderImage();

            SetTime(sentTime);

            SetTickEvent();
        }

        public void SetTime(DateTime time)
        {
            TimeBlock.Text = $"{VisHelper.GetCorrectTimeParamVis(time.Hour.ToString())}:" +
                $"{VisHelper.GetCorrectTimeParamVis(time.Minute.ToString())}";
        }

        public void SetGif(string gifPath)
        {
            ImgMessage = null;
            MyVideoPlayer.Visibility = Visibility.Hidden;
            ImageBorder.Visibility = Visibility.Hidden;

            var uri = new Uri(gifPath, UriKind.RelativeOrAbsolute);
            var source = new BitmapImage(uri);
            WpfAnimatedGif.ImageBehavior.SetAnimatedSource(GifImage, source);
            WpfAnimatedGif.ImageBehavior.SetRepeatBehavior(GifImage, RepeatBehavior.Forever);

            SetGifSize(GifImage, GifBorder, source);
        }

        public void SetGifSize(Image img, Border border, BitmapImage bitmap)
        {
            border.Width = bitmap.PixelWidth;
            border.Height = bitmap.PixelHeight;

            if (border.Width < _minMediaSize) border.Width = _minMediaSize;
            if (border.Width > _maxMediaSize) border.Width = _maxMediaSize;

            if (border.Height < _minMediaSize) border.Height = _minMediaSize;
            if (border.Height > _maxMediaSize) border.Height = _maxMediaSize;
        }

        public MediaMessage(TelSystem system,
            MediaElement media,
            string senderImgName,
            MediaAction mediaLogicEl,
            int? forwardedFromId)
        {
            _system = system;
            _media = media;
            _senderImgName = senderImgName;
            _forwardedFrom = forwardedFromId;

            InitializeComponent();

            HideAllBorders();
            //VideoBorder.Visibility = Visibility.Visible;
            ImageBorder.Visibility = Visibility.Visible;
            SetVideoPreview();
            SetSenderImage();

            SetTickEvent();

            SetTime(mediaLogicEl.SentTime);
        }

        public void SetTickEvent()
        {
            SelectionTickObj.StatusChanged += () =>
            {
                //Pressed on tick
                //Update counter on user chat
                ((MainWindow)Window.GetWindow(this)).UpdateUserChatSelectedAmount();
            };
        }

        private const int _visForwardRowHeight = 20;
        private async Task SetForwardedFromRow()
        {
            if (_forwardedFrom is null) return;
            TelegramLib.MainClasses.User from =
                await ApiService.GetUserById((int)_forwardedFrom);
            if (from is null) return;

            //Set forwarded from user id as tag
            LoginForwarded.Tag = from.Id;

            ForwardedRow.Height = new GridLength(_visForwardRowHeight);
            LoginForwarded.Text = from.Login;
        }

        public void SetForwardedRowHeight(bool isShow)
        {
            if (isShow) ForwardedRow.Height = new GridLength(_visForwardRowHeight);
            else ForwardedRow.Height = new GridLength(0);
        }

        public bool IsForwardedRowIsHidden()
        {
            return ForwardedRow.Height.Value == 0;
        }

        public bool IsMessageIdTicked()
        {
            return SelectionTickObj.GetChosenStatus();
        }

        public void SetSenderImage()
        {
            if (_senderImgName is null)
            {
                BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetSystemImagePath("StopSign.png"), UriKind.Absolute));
                return;
            }

/*            BitmapImage bitmap = ApiService.GetCachedBitmap(_senderImgName);

            string fullPath = FilesAction.GetUserImagePath(_senderImgName);
            ImgMessage.ImageSource = bitmap is not null ? bitmap :
                SignalRHelperService.LoadBitmap(fullPath);*/

            BgBrush.ImageSource = new BitmapImage(new Uri(
                FilesAction.GetUserImagePath(_senderImgName), UriKind.Absolute));
        }

        private async Task SetVideoPreview()
        {
            string fileName = System.IO.Path.GetFileName(_media.Source.LocalPath);

            Image img = await FilesAction.GetImagePreviewForVideo(fileName);

            ImgMessage.ImageSource = img.Source;
           // VideoParams.Visibility = Visibility.Visible;

            SetImgMessageSize(img, ImageBorder, isVideo: false);

            _media.MediaOpened += (sender, e) =>
            {
                if (_media.NaturalDuration.HasTimeSpan)
                {
                    VideoDuration.Text = _media.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                }
                else VideoDurationGrid.Visibility = Visibility.Hidden;
            };
        }

        public void HideAllBorders()
        {
            MyVideoPlayer.Visibility = Visibility.Hidden;
            ImageBorder.Visibility = Visibility.Hidden;
            GifBorder.Visibility = Visibility.Hidden;
        }

        public MediaElement GetVideo()
        {
            return _media;
        }

        public Image GetImage()
        {
            return _img;
        }

        public string GetGifPath()
        {
            return _gifPath;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isOnlyVisual) return;
            SendInfoGrid.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            SendInfoGrid.Visibility = Visibility.Hidden;
        }

        private const int _tickColWidth = 20;
        public void SetTickVis(string iconName, bool isCanBeVis)
        {
            TickColumn.Width = new GridLength(_tickColWidth);
            SetVisibility(iconName);
            if (isCanBeVis) TickIcon.Visibility = Visibility.Visible;
        }

        private const int _selectTickColWidth = 30;
        public void SetTickVisibility(bool isVis)
        {
            if (isVis && TickColumnDef.Width.Value == 0)
            {
                this.Width += _selectTickColWidth;
                TickColumnDef.Width = new GridLength(_selectTickColWidth);
            }
            else
            {
                this.Width -= _selectTickColWidth;
                TickColumnDef.Width = new GridLength(0);
            }
        }

        public void SetTickVisOnlyTockCol(bool isVis)
        {
            if (isVis)
            {
                TickColumnDef.Width = new GridLength(_selectTickColWidth);
            }
            else if (TickColumnDef.Width.Value != 0)
            {
                TickColumnDef.Width = new GridLength(0);
            }
        }

        public void SetVisibility(string iconName)
        {
            TickIcon.Kind = (PackIconKind)Enum.Parse(typeof(PackIconKind), iconName);
        }

        public void SetPinColumnState(bool isPinned)
        {
            if (isPinned) PinnIcon.Visibility = Visibility.Visible;
            else PinnIcon.Visibility = Visibility.Hidden;
        }

        private void LoginForwarded_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_isOnlyVisual) return;
            Cursor = Cursors.Hand;
        }

        private void LoginForwarded_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private async void LoginForwarded_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isOnlyVisual) return;

            int.TryParse(LoginForwarded.Tag.ToString(), out int userId);
            var user = Task.Run(() => ApiService.GetUserById(userId)).Result;

            if (user is null) return;
            if (!await SetIsUserCanSeeChattersInfo(user))
            {
                MessageBox.Show("No no no mister fish, you go to tasik");
                return;
            }

            if (_system.LoggedUser.Id == userId)
            {
                //set logged user info page
                LoggedUserProfile logged = new LoggedUserProfile(_system.LoggedUser, _system);
                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(logged);
                return;
            }

            //set chatter info page
            TelegramLib.MainClasses.UserChat chat = _system.GetChatByChatterId(userId);
            if (chat is null) return;

            UserInfo infoPage = new UserInfo(chat, _system);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(infoPage);
        }

        public async Task<bool> SetIsUserCanSeeChattersInfo(TelegramLib.MainClasses.User user)
        {
            MainSettings setUserSettings = await SignalRHelperService.GetMainSettings(user, null);

            if (setUserSettings.PrivacySettings.ForwardMesPrivacy
                .ShareWithExps.Any(x => x.Id == _system.LoggedUser.Id)) return true;
            else if (setUserSettings.PrivacySettings.ForwardMesPrivacy
                .NeverShareExps.Any(x => x.Id == _system.LoggedUser.Id)) return false;

            return setUserSettings.PrivacySettings.
                ForwardMesPrivacy.IsUserPageCanBeSeen(_system.Contacts, user.Id);
        }
        public bool IsTickVisible()
        {
            return TickColumnDef.Width.Value != 0;
        }

        public void ChangeTickStatus()
        {
            if (!IsTickVisible()) return;
            SelectionTickObj.SetMirrorStatus();
        }

        public MediaAction GetMessage()
        {
            return _message;
        }

        private void GoToForwardedGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void GoToForwardedGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void GoToForwardedGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PushForwarded.Invoke();
        }

        public void SetPushForwardedVis()
        {
            const int goToMesGridWidth = 30;

            GoToMessage.Width = new GridLength(goToMesGridWidth);
            Width += goToMesGridWidth;

            ForwardedGrid.Visibility = Visibility.Hidden;
            Height -= _visForwardRowHeight;
            ForwardedRow.Height = new GridLength(0);
        }

        private void ImageBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void ImageBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void ImageBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_system is null) return;
            //Get user 
            DependencyObject check = this.Parent;
            if (check is not ListBoxItem item || item.Tag is null) return;

            int.TryParse(item.Tag.ToString(), out int mesId);

            TelegramLib.MainClasses.Messages.Message mes =
                _system.GetMessageById(mesId);
            if (mes is null) return;

            bool isSavedChat = _system.GetIsSavedMesChatStatus();

            //Settings logged user page
            if ((_system.LoggedUser.Id == mes.SenderUserId && !isSavedChat) ||

                (isSavedChat && mes.ForwardedFromId is null && mes.SenderUserId == 0) ||
                (isSavedChat && _system.LoggedUser.Id == mes.ForwardedFromId))
            {

                UserInfo logged = new UserInfo(_system.SavedMesesChat, _system);
                ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(logged);
                return;
            }

            //Set other user page
            TelegramLib.MainClasses.UserChat chat = isSavedChat && mes.ForwardedFromId is not null ?
                _system.GetChatByChatterId((int)mes.ForwardedFromId) :
                _system.GetChatByMessage(mes);

            UserInfo info = new UserInfo(chat, _system);
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(info);
        }

        public string GetMediaPath()
        {
            if (_gifPath is not null && _gifPath != string.Empty) return _gifPath;
            if (_stickerPath is not null && _stickerPath != string.Empty) return _stickerPath;
            if (_message is not null) return _message.MediaName;

            return string.Empty;
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public void HideAllSelectionBorders()
        {
            foreach (var border in _selectBorders)
            {
                border.Visibility = Visibility.Hidden;
            }
        }

        public void ChangeSelectionBorderStatus(MediaAction action)
        {
            for (int i = 0; i < _bandBorders.Count; i++)
            {
                if (_bandBorders[i].Tag is null) continue;
                int.TryParse(_bandBorders[i].Tag.ToString(), out int index);

                if (index == action.Id)
                {
                    _selectBorders[i].Visibility =
                        _selectBorders[i].Visibility == Visibility.Visible ?
                        Visibility.Hidden : Visibility.Visible;
                }
            }
        }

        public void MirrorSelectionById(int id)
        {
            Border? border = _selectBorders.FirstOrDefault(x => x.Tag is not null && x.Tag.ToString() == id.ToString());

            if (border is null) return;
            border.Visibility = border.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        public void SetChoseVisById(int id, Visibility vis)
        {
            Border? border = _selectBorders.FirstOrDefault(x => x.Tag is not null && x.Tag.ToString() == id.ToString());

            if (border is null) return;
            border.Visibility = vis;
        }

        public bool IsAllMediasInBandAreChosen()
        {
            for (int i = 0; i < _selectBorders.Count; i++)
            {
                if (_selectBorders[i].Visibility == Visibility.Hidden) return false;

                if (i == _bandPaths.Count - 1) return true;
            }
            return true;
        }


        public int GetAmountOfSelectedMediasInBand()
        {
            int res = 0;
            foreach (var border in _selectBorders)
            {
                if (border.Visibility == Visibility.Visible) res++;
            }
            return res;
        }

        public void SetBandSelection(bool isVis)
        {
            for (int i = 0; i < _selectBorders.Count; i++)
            {
                _selectBorders[i].Visibility = isVis ? Visibility.Visible : Visibility.Hidden;

                if (i == _bandPaths.Count - 1) return;
            }
        }

        public List<int> GetSelectedMessagesIdsInBand()
        {
            List<int> res = new List<int>();

            for (int i = 0; i < _selectBorders.Count; i++)
            {
                if (_selectBorders[i].Visibility == Visibility.Visible)
                {
                    if (_selectBorders[i].Tag is null) continue;
                    int.TryParse(_selectBorders[i].Tag.ToString(), out int id);

                    res.Add(id);
                }
                if (i == _bandPaths.Count - 1) return res;
            }
            return res;
        }

        public bool IsBandMessageExistById(int id)
        {
            return _selectBorders.Any(x => x.Tag is not null && x.Tag.ToString() == id.ToString());
        }
    }
}
