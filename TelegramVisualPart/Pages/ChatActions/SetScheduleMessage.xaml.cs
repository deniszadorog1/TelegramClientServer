using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.MainClasses.Messages;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.Pages.ChatActions
{
    /// <summary>
    /// Логика взаимодействия для SetScheduleMessage.xaml
    /// </summary>
    public partial class SetScheduleMessage : Page
    {
        private DateTime? _schedDate = DateTime.Now.AddDays(1);

        public List<Message> _messages;
        private TelegramLib.MainClasses.UserChat _chat;
        private TelegramLib.MainClasses.TelSystem _system;
        private List<Message> _forwarded;

        private bool _isUpdateDate = false;
        private bool _isBandMessage = false;

        public SetScheduleMessage(
            TelegramLib.MainClasses.UserChat chat,
            List<Message> messages,
            TelegramLib.MainClasses.TelSystem system,

            List<Message> forwardMeses,

            bool isUpdateDate = false,
            bool isBandMessages = false)
        {
            _messages = messages;
            _chat = chat;
            _system = system;
            _isUpdateDate = isUpdateDate;
            _forwarded = forwardMeses;
            _isBandMessage = isBandMessages;

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

            for (int i = 0; i < _messages.Count; i++) _messages[i].SentTime = (DateTime)_schedDate;
            if (_forwarded is not null) _forwarded.ForEach(x => x.SentTime = (DateTime)_schedDate);

            
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
            List<Message> addedMessages = new List<Message>();

            await SetMessagesInSchedList(addedMessages, _messages);
            if(_forwarded is not null) await SetMessagesInSchedList(addedMessages, _forwarded);

            if (addedMessages.First().Id < 0)
            {
                ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
                return;
            }
            //Add in Sched List
            _chat.AddScheduleMessage(addedMessages, _system.LoggedUser);

            if(_forwarded is not null)
            {
                _forwarded.Clear();
                _forwarded = null;
            }
        }

        public async Task SetMessagesInSchedList(List<Message> schedMeses,
            List<Message> toSetIn)
        {
            for (int i = 0; i < toSetIn.Count; i++) toSetIn[i].IsSchedule = true;

            List<MediaAction> bandMesses = new List<MediaAction>();
            for (int i = 0; i < toSetIn.Count; i++)
            {
                if (bandMesses.Count > 0 &&
                  (toSetIn[i] is TelegramLib.MainClasses.Messages.TextMessage ||
                  toSetIn[i] is MediaAction mediaCheck && mediaCheck.BandId != bandMesses.First().BandId))
                {
                    await SetBandInList(bandMesses, schedMeses);
                }

                if (toSetIn[i] is MediaAction media &&
                    (media.BandId != -1 || 
                    (toSetIn == _messages && _isBandMessage)) )
                {
                    bandMesses.Add(media);
                    continue;
                }

/*                await ApiService.AddMessage(toSetIn[i], _chat);
                schedMeses.Add(await ApiService.GetLastChatMessage(_chat.Id));*/
                schedMeses.Add(await ApiService.AddMessage(toSetIn[i], _chat));
            }

            if(bandMesses.Count != 0)
            {
                await SetBandInList(bandMesses, schedMeses);
            }
        }

        public async Task SetBandInList(List<MediaAction> bandMesses, 
            List<TelegramLib.MainClasses.Messages.Message> schedMeses)
        {
            int newBandId = await ApiService.GetLastMessageBandId() + 1;

            for (int j = 0; j < bandMesses.Count; j++)
            {
                bandMesses[j].BandId = newBandId;

/*                await ApiService.AddMessage(bandMesses[j], _chat);
                schedMeses.Add(await ApiService.GetLastChatMessage(_chat.Id));*/


                schedMeses.Add(await ApiService.AddMessage(bandMesses[j], _chat));
            }
            bandMesses.Clear();
        }

        public bool IsBandMessage()
        {
            List<MediaAction> medias = _messages.OfType<MediaAction>().ToList();

            return medias.Count == _messages.Count && !medias.Any(x => x.IsSticker) && !medias.Any(x => x.IsGif());
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
