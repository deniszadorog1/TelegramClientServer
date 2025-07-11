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
using TelegramLib.MainClasses;

namespace TelegramVisualPart.Pages.Settings.PrivAndSecurity.ButsPages
{
    /// <summary>
    /// Логика взаимодействия для SetLocalCode.xaml
    /// </summary>
    public partial class SetLocalCode : Page
    {
        private TelSystem _system;
        public SetLocalCode(TelSystem system)
        {
            _system = system;
            InitializeComponent();
            SetBasicParams();
        }

        public void SetBasicParams()
        {
            BackBut.IconType.Kind = PackIconKind.ArrowLeft;
            CloseBut.IconType.Kind = PackIconKind.Close;
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void BackBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(
                new PrivacyAndSecurity(_system));
        }
    }
}
