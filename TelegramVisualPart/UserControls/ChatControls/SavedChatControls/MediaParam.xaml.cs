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
using TelegramLib.Models;
using TelegramVisualPart.Enums;

namespace TelegramVisualPart.UserControls.ChatControls.SavedChatControls
{
    /// <summary>
    /// Логика взаимодействия для MediaParam.xaml
    /// </summary>
    public partial class MediaParam : UserControl
    {
        public MediaParam()
        {
            InitializeComponent();
        }

        public void SetControlParams(PackIconKind icon, int amount, string controlText)
        {
            MainIcon.Kind = icon;
            AmountRun.Text = amount.ToString();
            TextNameRun.Text = controlText;
        }

        public void SetValues(SentItemsTypes type, TelegramLib.MainClasses.UserChat chat)
        {
            switch (type)
            {
                case SentItemsTypes.Photos:
                    {
                        if (!chat.IsImageMessagesExist())
                        {
                            Height = 0;
                            return;
                        }
                        AmountRun.Text = chat.GetAmountOfImages().ToString();
                        return;
                    }
                case SentItemsTypes.Video:
                    {
                        if (!chat.IsVideoMessagesExist())
                        {
                            Height = 0;
                            return;
                        }
                        AmountRun.Text = chat.GetAmountOfVideos().ToString();
                        return;
                    }
                case SentItemsTypes.SharedLinks:
                    {
                        List<string> links = chat.GetLinks();

                        if (links.Count == 0)
                        {
                            Height = 0;
                            return;
                        }
                        AmountRun.Text = links.ToString();

                        return;
                    }
                case SentItemsTypes.GIFs:
                    {
                        if (!chat.IsGifMessagesExist())
                        {
                            Height = 0;
                            return;
                        }

                        AmountRun.Text = chat.GetAmountOfGifs().ToString();
                        return;
                    }
            }

        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            Background =
                 (SolidColorBrush)Application.Current.Resources["DarkThemeSecond"];
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            Background = new SolidColorBrush(Colors.Transparent);
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
