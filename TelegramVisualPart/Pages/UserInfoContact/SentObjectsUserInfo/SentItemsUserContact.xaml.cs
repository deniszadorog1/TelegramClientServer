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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TelegramVisualPart.Pages.UserInfoContact.SentObjectsUserInfo
{
    /// <summary>
    /// Логика взаимодействия для SentItemsUserContact.xaml
    /// </summary>
    public partial class SentItemsUserContact : Page
    {
        private Enums.SentItemsTypes _type;
        public SentItemsUserContact(Enums.SentItemsTypes type)
        {
            _type = type;
            InitializeComponent();

            SetBasicBlocks();
            SetIconsKind();
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
    }
}
