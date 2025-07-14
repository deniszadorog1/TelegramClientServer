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
using TelegramVisualPart.UserControls.ContactsControls.AutoDeleteControls;

namespace TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion
{
    /// <summary>
    /// Логика взаимодействия для SelfDestructTimer.xaml
    /// </summary>
    public partial class SelfDestructTimer : Page
    {
        public SelfDestructTimer()
        {
            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            //Set basic radioBut

            CloseBut.IconType.Kind = PackIconKind.Close;

            OffRadioBut.ButName.Text = "Off";
            OneDayBut.ButName.Text = "After 1 day";
            OneWeekBut.ButName.Text = "After 1 week";
            OneMonthBut.ButName.Text = "After 1 month";
            CustomTimeBut.ButName.Text = "Set Custom Time";
            CustomTimeBut.RadioButton.Visibility = Visibility.Hidden;
        }

        private void DisActivateAllRadios()
        {
            foreach(var obj in RadiosGrid.Children)
            {
                if(obj is RadioBut radioBut)
                {
                    radioBut.RadioButton.IsChecked = false;
                }
            }
        }

        private void SetAutoDeleteToContact_MouseEnter(object sender, MouseEventArgs e)
        {
            SetAutoDeleteToContact.TextDecorations = TextDecorations.Underline;
            Cursor = Cursors.Hand;
        }

        private void SetAutoDeleteToContact_MouseLeave(object sender, MouseEventArgs e)
        {
            SetAutoDeleteToContact.TextDecorations = null;
            Cursor = null;
        }

        private void SetAutoDeleteToContact_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new ToChooseChats());
        }

        private void DateButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DisActivateAllRadios();
            //here to do something with date
        }

        private void CustomTimeBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetSecondaryFrame(new SetCustomTime());
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }
    }
}
