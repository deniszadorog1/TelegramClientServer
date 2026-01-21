using System;
using System.Collections.Generic;
using System.Globalization;
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
using TelegramVisualPart.Services;

namespace TelegramVisualPart.Pages.ChatActions
{
    /// <summary>
    /// Логика взаимодействия для SetScheduleMessage.xaml
    /// </summary>
    public partial class SetScheduleMessage : Page
    {
        private DateTime? _schedDate = null;

        private TelegramLib.MainClasses.Messages.Message _message;
        private TelegramLib.MainClasses.UserChat _chat;
        private TelegramLib.MainClasses.TelSystem _system;

        public SetScheduleMessage(TelegramLib.MainClasses.UserChat chat,
            TelegramLib.MainClasses.Messages.Message mes,
            TelegramLib.MainClasses.TelSystem system)
        {
            _message = mes;
            _chat = chat;
            _system = system;

            InitializeComponent();
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["OtherButMouseEnter"];
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private async void ScheduleBut_Click(object sender, RoutedEventArgs e)
        {
            if (_schedDate is null) return;

            //Set hour and minutes to chosen date


            //Add in db
            TelegramLib.MainClasses.Messages.Message addSched = await ApiService.AddAndGetSchedMessage(_chat, _message);

            if (addSched.Id < 0)
            {
                return;
            }

            //Add in Sched List
            _chat.AddScheduleMessage(addSched);

        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void DateBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPage calPage = new CalendarPage(false);
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(calPage);

            calPage.Calendar.SelectedDatesChanged += (sender, e) =>
            {
                DateTime? date = calPage.Calendar.SelectedDate;

                if (date is null) return;
                _schedDate = date;

                string result = ((DateTime)date).
                ToString("MMMM d", CultureInfo.InvariantCulture);

                DateBox.Text = result;
            };
        }

        private void DigitInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void HourAndMinBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox tb)
                return;

            if (string.IsNullOrEmpty(tb.Text))
                return;

            if (!int.TryParse(tb.Text, out int value))
            {
                tb.Text = "0";
                tb.CaretIndex = tb.Text.Length;
                return;
            }

            const int maxHourVal = 24;
            const int maxMinuteVal = 60; ;

            int maxValue = sender == HourBox ?
                maxHourVal : maxMinuteVal;

            if (value > maxValue)
            {
                tb.Text = maxValue.ToString();
                tb.CaretIndex = tb.Text.Length;
            }
        }
    }
}
