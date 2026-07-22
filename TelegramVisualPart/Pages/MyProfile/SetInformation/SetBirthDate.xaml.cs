using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Services;
using User = TelegramLib.MainClasses.User;

namespace TelegramVisualPart.Pages.MyProfile.SetInformation
{
    /// <summary>
    /// Логика взаимодействия для SetBirthDate.xaml
    /// </summary>
    public partial class SetBirthDate : Page
    {
        private readonly Dictionary<int, string> _months = new Dictionary<int, string>()
        {
            { -1, " " },
            { 0, " " },
            {1, "January" },
            {2, "February" },
            {3, "March" },
            {4, "April" },
            {5, "May" },
            {6, "June" },
            {7, "July" },
            {8, "August" },
            {9, "September" },
            {10, "October" },
            {11, "November" },
            {12, "December" },
            { -100, "" },
            { -200, " " },
        };

        const int _minYear = 1875;

        private User _user;
        public SetBirthDate(User user)
        {
            _user = user;
            InitializeComponent();

            //SetBasicParams();

            //By temp item thing
            FillListsWithCustomControl();
            SetStartDate();

            SetLanguageText.SetBirthDate(this);
        }

        public void FillListsWithCustomControl()
        {
            //Set year 
            SetYears();

            //Set month
            SetMonths();

            //Set day
            SetDays();
            SetDaysUpdate();
        }

        public void SetDaysUpdate()
        {
            MonthsSpecial.SelectedIndexUpdate += () =>
            {
                SetDays();
            };
            YearSpecial.SelectedIndexUpdate += () =>
            {
                SetDays();
            };
        }

        public void SetDays()
        {
            int chosenMonth = MonthsSpecial.GetSelectedIndex() - 1;

            int days = DateTime.DaysInMonth(_minYear + YearSpecial.GetSelectedIndex(), chosenMonth);
            DaysSpecial.ClearCheckPanel();

            List<string> daysList = new List<string>();
            daysList.Add(" ");
            daysList.Add(" ");

            for (int i = 1; i <= days; i++)
            {
                daysList.Add(i.ToString());
            }

            daysList.Add(" ");
            daysList.Add(" ");

            DaysSpecial.SetListWithBlocks(daysList);
        }


        public void SetMonths()
        {
            List<string> monthsList = new List<string>();

            foreach (var month in _months)
            {
                monthsList.Add(month.Value);
            }

            MonthsSpecial.SetListWithBlocks(monthsList);
        }

        private const string _emptyYear = "---";
        public void SetYears()
        {
            List<string> yearList = new List<string>();
            //Set values in list
            yearList.Add(" ");
            yearList.Add(" ");

            for (int i = _minYear; i <= DateTime.Now.Year; i++)
            {
                yearList.Add(i.ToString());
            }

            yearList.Add(_emptyYear);

            yearList.Add(" ");
            yearList.Add(" ");

            //Set year list with blocks
            YearSpecial.SetListWithBlocks(yearList);
        }


        public void SetStartDate()
        {
            const int minYear = 1;
            const int maxYearAdd = 1;
            const int baseYearAddIndex = 2;

            if (_user.BirthDay is null) return;

            //DaysSpecial.SetSelectedIndex(1);
            DaysSpecial.ValueByIndex(_user.BirthDay.Value.Day);

            //MonthsSpecial.SetSelectedIndex(1);
            MonthsSpecial.ValueByIndex(_user.BirthDay.Value.Month);

            //YearSpecial.SetSelectedIndex(2);

            int yearIndex = _user.BirthDay.Value.Year == minYear ? DateTime.Now.Year - _minYear + baseYearAddIndex : (_user.BirthDay.Value.Year - _minYear) + maxYearAdd;
            YearSpecial.ValueByIndex(yearIndex);
        }


        private void CenterElementInScrollViewer(UIElement element, ScrollViewer scroll)
        {
            const int devider = 2;
            if (element == null)
                return;

            var transform = element.TransformToAncestor(scroll);
            Point position = transform.Transform(new Point(0, 0));

            double elementCenter = position.Y + ((FrameworkElement)element).ActualHeight / devider;
            double scrollViewerCenter = scroll.ViewportHeight / devider;

            double offset = scroll.VerticalOffset + (elementCenter - scrollViewerCenter);
            scroll.ScrollToVerticalOffset(offset);
        }

        private TextBlock GetTextBlock(string text)
        {
            const int baseHeight = 40;
            const int baseFontSize = 16;
            const int baseThickness = 10;

            TextBlock block = new TextBlock()
            {
                Text = text,
                Height = baseHeight,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = baseFontSize,
                Padding = new Thickness(baseThickness),
                Foreground = (SolidColorBrush)System.Windows.Application.Current.Resources["UsualTextColor"]
            };

            block.PreviewMouseDown += ChooseBlock_PreviewMouseDown;

            return block;
        }

        public void ChooseBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (sender is not TextBlock block ||
                string.IsNullOrWhiteSpace(block.Text)) return;
            //CenterElementInScrollViewer(block, GetScrollViewerByTextBlock(block));
        }

        private async void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            const int devIndex = 1;
            //Make check for date
            int day = DaysSpecial.GetSelectedIndex() - devIndex;
            int month = MonthsSpecial.GetSelectedIndex() - devIndex;

            int year = GetYear();

            _user.BirthDay = new DateTime(year, month, day);

            await ApiService.UpdateUser(_user);

            //Update in Signal R
            await SignalRService.UpdateBirtDate(_user);

            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
            ((MainWindow)Window.GetWindow(this)).UpdateLoggedUserPage();
        }

        public int GetYear()
        {
            const int minYear = 1;
            const int devIndex = 2;

            if (YearSpecial.GetValueBySelectedIndex() == _emptyYear)
            {
                return minYear;
            }
            return _minYear + YearSpecial.GetSelectedIndex() - devIndex;
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but)
                but.Background = (SolidColorBrush)System.Windows.Application.Current.Resources["DarkThemeProfileButEnter"];
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private async void RemoveBut_Click(object sender, RoutedEventArgs e)
        {
            _user.BirthDay = null;
            await SignalRService.UpdateBirtDate(_user);

            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();

            ((MainWindow)Window.GetWindow(this)).UpdateLoggedUserPage();
        }
    }
}
