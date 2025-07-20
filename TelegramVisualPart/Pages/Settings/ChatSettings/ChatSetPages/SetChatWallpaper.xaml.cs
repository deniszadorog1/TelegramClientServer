using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
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
using TelegramLib.MainClasses.ChatFitures;
using TelegramLib.UserSettings.SettingsTypes;
using TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls.SetWallpapersControls;

namespace TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages
{
    /// <summary>
    /// Логика взаимодействия для SetChatWallpaper.xaml
    /// </summary>
    public partial class SetChatWallpaper : Page
    {
        private TelegramLib.UserSettings.SettingsTypes.ChatSettings _settings;
        private ChatBackground _chosenBackground;
        public SetChatWallpaper(TelegramLib.UserSettings.SettingsTypes.ChatSettings settings)
        {
            _settings = settings;

            InitializeComponent();

            SetWallpapers();
            SetClickEventToWallpapers();
        }

        public SetChatWallpaper(ChatBackground background)
        {
            _chosenBackground = background;
            InitializeComponent();

            SetWallpapers();
            SetClickEventToWallpapers();
        }

        public void SetWallpapers()
        {
            //Set real ones
            //SetRealWallpapers();

            //Test wallpapers
            SetTestWallpapers();
        }

        public void SetRealWallpapers()
        {
            for (int i = 0; i < _settings.PossibleWallpapers.Count; i++)
            {
                //create wallpaper here + add source as bg for bgImage
            }
        }

        public void SetTestWallpapers()
        {
            OneTest.WallpaperImage.Source = GetTestImage("Monkey.jpg").Source;
            OneTest.WallpaperImage.Tag = "Monkey.jpg";

            TwoTest.WallpaperImage.Source = GetTestImage("Pineapple.jpg").Source;
            TwoTest.WallpaperImage.Tag = "Pineapple.jpg";

            ThreeTest.WallpaperImage.Source = GetTestImage("Snowman.jpg").Source;
            ThreeTest.WallpaperImage.Tag = "Snowman.jpg";
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

            Page page = _settings is null ?
                new WallpaperPreview(_chosenBackground, wallpaper.WallpaperImage) :
                new WallpaperPreview(_settings, wallpaper.WallpaperImage);

            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(page);
        }

        public Image GetTestImage(string testWallpaperName)
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
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new WallpaperPreview(_settings, GetTestImage("Snowman.jpg")));
        }

        private void WallpaperBotBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearPageFromParentFrame(this);
        }
    }
}
