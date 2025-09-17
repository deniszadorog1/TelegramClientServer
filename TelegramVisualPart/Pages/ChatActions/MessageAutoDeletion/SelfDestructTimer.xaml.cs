using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using TelegramLib.Enums.Chat;
using TelegramLib.MainClasses;
using TelegramVisualPart.Helper;
using TelegramVisualPart.UserControls.ContactsControls.AutoDeleteControls;

namespace TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion
{
    /// <summary>
    /// Логика взаимодействия для SelfDestructTimer.xaml
    /// </summary>
    public partial class SelfDestructTimer : Page
    {
        private TelSystem _system;
        private List<AutoDeleteType> _typesToChose = new List<AutoDeleteType>();
        private AutoDeleteType _chosenType;

        public SelfDestructTimer(TelSystem system)
        {
            _system = system;
            InitializeComponent();

            SetLanguageText.SetSelfDestTimer(this);
            SetBasicParams();

            //_typesToChose = null;
        }

        public void SetBasicParams()
        {
            //Set basic radioBut
            OffRadioBut.RadioButton.IsChecked = true;

            CloseBut.IconType.Kind = PackIconKind.Close;

/*            OffRadioBut.ButName.Text = "Off";
            OneDayBut.ButName.Text = "After 1 day";
            OneWeekBut.ButName.Text = "After 1 week";
            OneMonthBut.ButName.Text = "After 1 month";
            CustomTimeBut.ButName.Text = "Set Custom Time";*/
            CustomTimeBut.RadioButton.Visibility = Visibility.Hidden;

            _typesToChose.Add(AutoDeleteType.Nothing);
            _typesToChose.Add(AutoDeleteType.OneDay);
            _typesToChose.Add(AutoDeleteType.OneWeek);
            _typesToChose.Add(AutoDeleteType.OneMonth);
        }

        private void DisActivateAllRadios()
        {
            foreach (var obj in RadiosGrid.Children)
            {
                if (obj is RadioBut radioBut)
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
            ((MainWindow)Window.GetWindow(this)).
                SetThirdFrame(new ToChooseChats(_system, _chosenType));
        }

        private void DateButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DisActivateAllRadios();

            if (sender is not RadioBut button) return;

            int indexInList = RadiosGrid.Children.OfType<RadioBut>().ToList().IndexOf(button);
            if (indexInList == -1) return;

            _chosenType = _typesToChose[indexInList];
        }

        private void CustomTimeBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetCustomTime setTime = new SetCustomTime();

            setTime.ChosenAutoDelete += NewDurstion_Chosen;

            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(setTime);
        }

        private void CloseBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void NewDurstion_Chosen(object sender, EventArgs e)
        {
            if (sender is not SetCustomTime timePage) return;

            AutoDeleteType type = timePage._chosenDeletionType;

            // check if equaly type is already in list
            if (IsEqualDelTypeIsExist(type))
            {
                //check radio by type
                CheckRadioByType(type);
                _chosenType = type;
                return;
            }
            // if no => get element in list to swap with

            //swap in list
            int swapIndex = SwapTypesAndGetSwopIndex(type);

            //swap in visal
            SwapAutoDelInVisual(swapIndex);
        }

        public void SwapAutoDelInVisual(int swapIndex)
        {
            DisActivateAllRadios();

            RadioBut but = RadiosGrid.Children.OfType<RadioBut>().ToList()[swapIndex];

            if (but is null) return;

            but.ButName.Text =
                $"After {new AutoDeleteDuration(_chosenType).GetStringByType()}";
            but.RadioButton.IsChecked = true;
        }

        private int SwapTypesAndGetSwopIndex(AutoDeleteType chosen)
        {
            for (int i = _typesToChose.Count - 1; i >= 0; i--)
            {
                if ((int)_typesToChose[i] < (int)chosen)
                {
                    _typesToChose[i] = chosen;
                    _chosenType = chosen;
                    return i;
                }
            }
            return -1;
        }


        private bool IsEqualDelTypeIsExist(AutoDeleteType type)
        {
            return _typesToChose.Any(x => x == type);
        }

        public void CheckRadioByType(AutoDeleteType type)
        {
            DisActivateAllRadios();

            int listIndex = _typesToChose.IndexOf(type);
            RadioBut but = RadiosGrid.Children.OfType<RadioBut>().ToList()[listIndex];

            but.RadioButton.IsChecked = true;
        }

    }
}
