using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TelegramLib.Enums.Messages;

using Newtonsoft.Json;

namespace TelegramLib.MainClasses.Messages
{
    public class Message : IMessage
    {
        public int Id { get; set; }
        public int SenderUserId { get; set; }
        public bool IsLoggedUserSent { get; set; }
        public DateTime SentTime { get; set; }
        public bool IsRead { get; set; }
        public bool IsPinned { get; set; }
        public int? ForwardedFromId { get; set; }

        public bool IsSchedule { get; set; }
        public string RepliedQuote { get; set; }

        public event Action SentTimeIsNow;

        public Message(int id, int senderUserId,
            DateTime sentTime, bool isRead, bool isPinned,
            int? forwardedFromId)
        {
            Id = id;
            SenderUserId = senderUserId;
            SentTime = sentTime;
            IsRead = isRead;
            IsPinned = isPinned;
            ForwardedFromId = forwardedFromId;

            StartTimer();
        }

        public Message()
        {
            Id = -1;
            IsLoggedUserSent = false;
            SentTime = DateTime.Now;
            IsRead = false;
            ForwardedFromId = null;
        }

        public DateTime? GetSentDate()
        {
            return SentTime;
        }

        public virtual string GetLastMessage()
        {
            return "This is last message";
        }

        public string GetSentTimeInString()
        {
            return $"{SentTime.Day}.{SentTime.Month}.{SentTime.Year}";
        }

        public void MirrorPinStatus()
        {
            IsPinned = !IsPinned;
        }

        public bool IsMessageForDate(DateTime date)
        {
            if (SentTime.Year != date.Year ||
               SentTime.Month != date.Month ||
               SentTime.Day != date.Day ||
               SentTime.Hour != date.Hour ||
               SentTime.Minute != date.Minute ||
               SentTime.Second != date.Second) return false;

            if (this is StaticMessage stat && !(stat.Date is null)) return false;

            return true;
        }

        public bool IsMessageIsAproximitlyInTime(DateTime date)
        {
            if (SentTime.Year != date.Year ||
                   SentTime.Month != date.Month ||
                   SentTime.Day != date.Day) return false;

            if (this is StaticMessage stat && !(stat.Date is null)) return false;

            return true;
        }

        private DispatcherTimer _timer;

        public void StartTimer()
        {
            _timer = null;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick -= Timer_Tick;
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        public void EndTimer()
        {
            if (_timer is null) return;
            _timer.Stop();
            _timer = null;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {      
            if (_timer is null) return;
            if(SentTime <= DateTime.Now)
            {     
                _timer.Stop();
                if (sender is DispatcherTimer timer) timer.Stop();

                _timer = null;
                SentTimeIsNow?.Invoke();
            }
        }

        public void ClearForwarded()
        {
            ForwardedFromId = null;
        }

        public void SetQuoteText(string text)
        {
            RepliedQuote = text;
        }

    }
}
