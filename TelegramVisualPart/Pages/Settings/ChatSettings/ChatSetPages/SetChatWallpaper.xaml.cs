using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls.SetWallpapersControls;
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.MainClasses;
using Microsoft.Win32;
using System.IO;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using Microsoft.IdentityModel.Tokens;

namespace TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages
{
    /// <summary>
    /// Логика взаимодействия для SetChatWallpaper.xaml
    /// </summary>
    public partial class SetChatWallpaper : Page
    {
        private TelegramLib.UserSettings.SettingsTypes.ChatSettings _settings;
        private ChatBackground _chosenBackground;
        private UserChat _chat;

        public SetChatWallpaper(TelegramLib.UserSettings.SettingsTypes.ChatSettings settings)
        {
            _settings = settings;

            InitializeComponent();

            SetWallpapers();
            SetClickEventToWallpapers();

            SetLanguageText.SetChatWallpaper(this);
        }

        public SetChatWallpaper(ChatBackground background, UserChat chat,
            TelegramLib.UserSettings.SettingsTypes.ChatSettings settings)
        {
            _chosenBackground = background;
            _chat = chat;
            _settings = settings;

            InitializeComponent();

            SetWallpapers();
            SetClickEventToWallpapers();

            SetLanguageText.SetChatWallpaper(this); 
        }

        public void SetWallpapers()
        {
            //Set real ones
            SetRealWallpapers();

            //Test wallpapers
            //SetTestWallpapers();
        }

        public void SetRealWallpapers()
        {
            for (int i = 0; i < _settings.PossibleWallpapers.Count; i++)
            {
                Wallpaper paper = new Wallpaper();
                paper.WallpaperImage.Source = 
                    GetWallpaperImage(_settings.PossibleWallpapers[i]).Source;

                paper.WallpaperImage.Tag = _settings.PossibleWallpapers[i];

                WallpapersPanel.Children.Add(paper);
                //create wallpaper here + add source as bg for bgImage
            }
        }

        public void SetTestWallpapers()
        {
/*            OneTest.WallpaperImage.Source = GetWallpaperImage("Monkey.jpg").Source;
            OneTest.WallpaperImage.Tag = "Monkey.jpg";

            TwoTest.WallpaperImage.Source = GetWallpaperImage("Pineapple.jpg").Source;
            TwoTest.WallpaperImage.Tag = "Pineapple.jpg";

            ThreeTest.WallpaperImage.Source = GetWallpaperImage("Snowman.jpg").Source;
            ThreeTest.WallpaperImage.Tag = "Snowman.jpg";*/
        }
        public void SetClickEventToWallpapers()
        {
            for (int i = 0; i < WallpapersPanel.Children.Count; i++)
            {
                if (WallpapersPanel.Children[i] is Wallpaper wallpaper)
                {
                    wallpaper.PreviewMouseDown += Wallpaper_PreviewMouseDown;
                }
            }
        }

        public void Wallpaper_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is not Wallpaper wallpaper) return;

            Page page = _chosenBackground is not null ?
                new WallpaperPreview(_chosenBackground, wallpaper.WallpaperImage, _chat) :
                new WallpaperPreview(_settings, wallpaper.WallpaperImage);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
        }

        public Image GetWallpaperImage(string testWallpaperName)
        {
            return new Image()
            {
                Source = new BitmapImage(new Uri(TestThing.GetTestParams.GetWallpaperPath(testWallpaperName), UriKind.Absolute))
            };
        }

        private void CloseBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void ChooseFromFileGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            ChooseFromFileGrid.Background =
                (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
        }

        private void ChooseFromFileGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            ChooseFromFileGrid.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void ChooseFromFileGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set Add file 
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Choose wallpaper";
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            dlg.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Visuals", "Images");

            if (dlg.ShowDialog() == true)
            {
                //Get file name (is it an image)
                string selectedFile = dlg.FileName;
                string fileName = Path.GetFileName(selectedFile);

                if (!FilesAction.IsFileIsImage(selectedFile)) return;

                //if an image -> add in db
                ApiService.AddWallpaper(fileName);

                //Add in wallpapers folder
                FilesAction.AddNewWallpaper(selectedFile);

                //Add in system
                _settings.PossibleWallpapers.Add(fileName);
            }
        }

        private void WallpaperBotBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearPageFromParentFrame(this);
        }
    }
}
