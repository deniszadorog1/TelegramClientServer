using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.Enums.Chat
{
    public class AutoDeleteDuration
    {
        public string Text { get; set; }
        public DateTime Duration { get; set; }
        public AutoDeleteType Type { get; set; }

        public AutoDeleteDuration(AutoDeleteType type)
        {
            Type = type;

            Text = GetStringByType();
            Duration = GetDurationByType();
        }

        public string GetStringByType()
        {
            return Type switch
            {
                AutoDeleteType.Nothing => "Nothing",
                AutoDeleteType.OneDay => "1 day",
                AutoDeleteType.TwoDays => "2 days",
                AutoDeleteType.ThreeDays => "3 days",
                AutoDeleteType.FourDays => "4 days",
                AutoDeleteType.FiveDays => "5 days",
                AutoDeleteType.SixDays => "6 days",
                AutoDeleteType.OneWeek => "1 week",
                AutoDeleteType.TwoWeeks => "2 weeks",
                AutoDeleteType.ThreeWeeks => "3 weeks",
                AutoDeleteType.OneMonth => "1 month",
                AutoDeleteType.TwoMonths => "2 months",
                AutoDeleteType.ThreeMonths => "3 months",
                AutoDeleteType.FourMonths => "4 months",
                AutoDeleteType.FiveMonths => "5 months",
                AutoDeleteType.SixMonths => "6 months",
                AutoDeleteType.OneYear => "1 year",
                _ => "Unknown"
            };
        }

        public DateTime GetDurationByType( )
        {
            return Type switch
            {
                AutoDeleteType.Nothing => DateTime.Now,
                AutoDeleteType.OneDay => DateTime.Now.AddDays(-1),
                AutoDeleteType.TwoDays => DateTime.Now.AddDays(-2),
                AutoDeleteType.ThreeDays => DateTime.Now.AddDays(-3),
                AutoDeleteType.FourDays => DateTime.Now.AddDays(-4),
                AutoDeleteType.FiveDays => DateTime.Now.AddDays(-5),
                AutoDeleteType.SixDays => DateTime.Now.AddDays(-6),
                AutoDeleteType.OneWeek => DateTime.Now.AddDays(-7),
                AutoDeleteType.TwoWeeks => DateTime.Now.AddDays(-14),
                AutoDeleteType.ThreeWeeks => DateTime.Now.AddDays(-21),
                AutoDeleteType.OneMonth => DateTime.Now.AddMonths(-1),
                AutoDeleteType.TwoMonths => DateTime.Now.AddMonths(-2),
                AutoDeleteType.ThreeMonths => DateTime.Now.AddMonths(-3),
                AutoDeleteType.FourMonths => DateTime.Now.AddMonths(-4),
                AutoDeleteType.FiveMonths => DateTime.Now.AddMonths(-5),
                AutoDeleteType.SixMonths => DateTime.Now.AddMonths(-6),
                AutoDeleteType.OneYear => DateTime.Now.AddYears(-1),
                _ => DateTime.Now
            };
        }
    }
}
