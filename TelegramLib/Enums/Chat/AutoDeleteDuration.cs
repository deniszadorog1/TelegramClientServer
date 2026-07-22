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
            switch (Type)
            {
                case AutoDeleteType.Nothing:
                    return  "Nothing";
                case AutoDeleteType.OneDay:
                    return "1 day";
                case AutoDeleteType.TwoDays:
                    return "2 days";
                case AutoDeleteType.ThreeDays:
                    return "3 days";
                case AutoDeleteType.FourDays:
                    return "4 days";
                case AutoDeleteType.FiveDays:
                    return "5 day";
                case AutoDeleteType.SixDays:
                    return "6 days";
                case AutoDeleteType.OneWeek:
                    return "1 week";
                case AutoDeleteType.TwoWeeks:
                    return "2 weeks";
                case AutoDeleteType.ThreeWeeks:
                    return "3 weeks";
                case AutoDeleteType.OneMonth:
                    return "1 month";
                case AutoDeleteType.TwoMonths:
                    return "2 months";
                case AutoDeleteType.ThreeMonths:
                    return "3 months";
                case AutoDeleteType.FourMonths:
                    return "4 months";
                case AutoDeleteType.FiveMonths:
                    return "5 months";
                case AutoDeleteType.SixMonths:
                    return "6 month";
                case AutoDeleteType.OneYear:
                    return "1 year";
                default:
                    return "Unknown";
            }

/*            return Type switch
            {
                
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
            };*/
        }

        public DateTime GetDurationByType()
        {
            switch (Type)
            {
                case AutoDeleteType.Nothing:
                    return DateTime.Now;
                case AutoDeleteType.OneDay:
                    return DateTime.Now.AddDays(-1);
                case AutoDeleteType.TwoDays:
                    return DateTime.Now.AddDays(-2);
                case AutoDeleteType.ThreeDays:
                    return DateTime.Now.AddDays(-3);
                case AutoDeleteType.FourDays:
                    return DateTime.Now.AddDays(-4);
                case AutoDeleteType.FiveDays:
                    return DateTime.Now.AddDays(-5);
                case AutoDeleteType.SixDays:
                    return DateTime.Now.AddDays(-6);
                case AutoDeleteType.OneWeek:
                    return DateTime.Now.AddDays(-7);
                case AutoDeleteType.TwoWeeks:
                    return DateTime.Now.AddDays(-14);
                case AutoDeleteType.ThreeWeeks:
                    return DateTime.Now.AddDays(-21);
                case AutoDeleteType.OneMonth:
                    return DateTime.Now.AddMonths(-1);
                case AutoDeleteType.TwoMonths:
                    return DateTime.Now.AddMonths(-2);
                case AutoDeleteType.ThreeMonths:
                    return DateTime.Now.AddMonths(-3);
                case AutoDeleteType.FourMonths:
                    return DateTime.Now.AddMonths(-4);
                case AutoDeleteType.FiveMonths:
                    return DateTime.Now.AddMonths(-5);
                case AutoDeleteType.SixMonths:
                    return DateTime.Now.AddMonths(-6);
                case AutoDeleteType.OneYear:
                    return DateTime.Now.AddYears(-1);
                default:
                    return DateTime.Now;
            }


/*            return Type switch
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
            };*/
        }
    }
}
