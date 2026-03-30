using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
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
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TelegramVisualPart.Pages.ChatActions
{
    /// <summary>
    /// Логика взаимодействия для SetScheduleMessage.xaml
    /// </summary>
    public partial class SetScheduleMessage : Page
    {
        private DateTime? _schedDate = DateTime.Now.AddDays(1);

        public List<TelegramLib.MainClasses.Messages.Message> _messages;
        private TelegramLib.MainClasses.UserChat _chat;
        private TelegramLib.MainClasses.TelSystem _system;

        private bool _isUpdateDate = false;

        public SetScheduleMessage(
            TelegramLib.MainClasses.UserChat chat,
            List<TelegramLib.MainClasses.Messages.Message> messages,
            TelegramLib.MainClasses.TelSystem system,
            bool isUpdateDate = false)
        {
            _messages = messages;
            _chat = chat;
            _system = system;
            _isUpdateDate = isUpdateDate;

            InitializeComponent();

            UpdateDate();
            SetStartDate();
        }

        private void UpdateDate()
        {
            if (!_isUpdateDate) return;
            _schedDate = _messages.First().SentTime;
        }

        public void SetStartDate()
        {
            if (_schedDate is null) return;

            DateBox.Text = ((DateTime)_schedDate).
                ToString("MMMM d", CultureInfo.InvariantCulture);

            HourBox.Text = _schedDate.Value.Hour.ToString();
            MinuteBox.Text = _schedDate.Value.Minute.ToString();
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
            const int minDifferInMinutes = 0;

            //Set hour and minutes to chosen date
            AddHourMinuteInDate();

            if (_schedDate is null || (_schedDate.Value - DateTime.Now).TotalMinutes < minDifferInMinutes) return;

            for(int i = 0; i < _messages.Count; i++) _messages[i].SentTime = (DateTime)_schedDate;

            if (_isUpdateDate)
            {
                for (int i = 0; i < _messages.Count; i++)
                {
                    await ApiService.UpdateSchedMessageDate(_messages[i].Id, (DateTime)_schedDate);
                }

                ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
                ((MainWindow)Window.GetWindow(this)).UpdateScheduleChat();
                return;
            }

            await ScheduleMessageAdding();
            ClosePageAfterAddingSchedMessage();
        }

        public void ClosePageAfterAddingSchedMessage()
        {
            ((MainWindow)Window.GetWindow(this)).ClearCommentChatBox();
            ((MainWindow)Window.GetWindow(this)).ClearReplyRow();
            ((MainWindow)Window.GetWindow(this)).UpdateScheduleIconVisibility();

            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
            ((MainWindow)Window.GetWindow(this)).UpdateScheduleChat();
        }

        public async Task ScheduleMessageAdding()
        {
            for (int i = 0; i < _messages.Count; i++) _messages[i].IsSchedule = true;

            List<Message> addedMessages = new List<Message>();

            if (IsBandMessage())
            {
                List<MediaAction> medias = _messages.Cast<MediaAction>().ToList();

                //Set band messages
                int newBandId = await ApiService.GetLastMessageBandId() + 1;

                for(int i = 0; i < _messages.Count; i++)
                {
                    medias[i].BandId = newBandId;
                    await ApiService.AddMessage(medias[i], _chat);

                    addedMessages.Add(await ApiService.GetLastChatMessage(_chat.Id));
                }
            }
            else
            {
                await ApiService.AddMessage(_messages.First(), _chat);
                addedMessages.Add(await ApiService.GetLastChatMessage(_chat.Id));
            }

            if (addedMessages.First().Id < 0)
            {
                ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
                return;
            }
            //Add in Sched List
            _chat.AddScheduleMessage(addedMessages, _system.LoggedUser);
        }

        public bool IsBandMessage()
        {
            return _messages.Count == _messages.OfType<MediaAction>().ToList().Count;
        }

        public async Task AddBandMessage(List<Message> addedMessages)
        {
            for (int i = 0; i < _messages.Count; i++)
            {
                await ApiService.AddMessage(_messages[i], _chat);
            }
        }

        public void AddHourMinuteInDate()
        {
            int.TryParse(HourBox.Text, out int hour);
            int.TryParse(MinuteBox.Text, out int minutes);

            if (_schedDate is null) return;

            _schedDate = new DateTime(
                _schedDate.Value.Year,
                _schedDate.Value.Month,
                _schedDate.Value.Day,
                hour,
                minutes,
                _schedDate.Value.Second
            );
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

                if (date is null || (date.Value - DateTime.Now).TotalDays < -1) return;

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

            const int maxHourVal = 23;
            const int maxMinuteVal = 59;

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
