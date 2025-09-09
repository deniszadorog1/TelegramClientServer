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
using System.Windows.Threading;
using TelegramLib.MainClasses;
using TelegramLib.Models;
using static System.Net.Mime.MediaTypeNames;
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

            yearList.Add(" ");
            yearList.Add(" ");

            //Set year list with blocks
            YearSpecial.SetListWithBlocks(yearList);
        }


        public void SetStartDate()
        {
            if (_user.BirthDay is null) return;

            //DaysSpecial.SetSelectedIndex(1);
            DaysSpecial.ValueByIndex(_user.BirthDay.Value.Day);

            //MonthsSpecial.SetSelectedIndex(1);
            MonthsSpecial.ValueByIndex(_user.BirthDay.Value.Month);

            //YearSpecial.SetSelectedIndex(2);
            YearSpecial.ValueByIndex((_user.BirthDay.Value.Year - _minYear) + 1);

            //Set day
            /*            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                        {
                            DaysSpecial.SetSelectedIndex(_user.BirthDay.Value.Day + 1);
                            DaysSpecial.ScrollViewer_PreviewMouseWheel(this, new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, 0)
                            {
                                RoutedEvent = UIElement.PreviewMouseWheelEvent,
                                Source = DaysSpecial.ScrollView
                            });
                        }));

                        //Set month
                        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                        {
                            MonthsSpecial.SetSelectedIndex(_user.BirthDay.Value.Month + 1);
                            MonthsSpecial.ScrollViewer_PreviewMouseWheel(this, new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, 0)
                            {
                                RoutedEvent = UIElement.PreviewMouseWheelEvent,
                                Source = MonthsSpecial.ScrollView
                            });
                        }));*/

            /*          //Set Year   
                      Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                      {
                          _selectedYearIndex = (_user.BirthDay.Value.Year - _minYear) + 2;
                          YearScroll_PreviewMouseWheel(this, new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, 0)
                          {
                              RoutedEvent = UIElement.PreviewMouseWheelEvent,
                              Source = YearScroll
                          });
                      }));*/

        }
        /*        public void SetBasicParams()
                {
                    SetDays();
                    SetMonths();
                    SetYears();
                }
        */
        /*        public int chosenMonth = 1;
                public int _selectedDayIndex = 2;
                public void SetDays()
                {
                    int days = DateTime.DaysInMonth(_minYear + _selectedYearIndex, chosenMonth);
                    DaysPanel.Children.Clear();

                    DaysPanel.Children.Add(GetTextBlock(" "));
                    DaysPanel.Children.Add(GetTextBlock(" "));

                    for (int i = 1; i <= days; i++)
                    {
                        DaysPanel.Children.Add(GetTextBlock($"{i}"));
                    }

                    DaysPanel.Children.Add(GetTextBlock(" "));
                    DaysPanel.Children.Add(GetTextBlock(" "));
                }

                private void DaysScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
                {
                    if (DaysPanel.Children.Count == 0)
                        return;

                    if (e.Delta < 0)
                        _selectedDayIndex = Math.Min(DaysPanel.Children.Count - 1 - 2, _selectedDayIndex + 1);
                    else if (e.Delta > 0)
                        _selectedDayIndex = Math.Max(2, _selectedDayIndex - 1);

                    var selectedElement = DaysPanel.Children[_selectedDayIndex] as UIElement;
                    CenterElementInScrollViewer(selectedElement, DaysScroll);

                    e.Handled = true;
                }

                public void SetYears()
                {
                    YearPanel.Children.Add(GetTextBlock(" "));
                    YearPanel.Children.Add(GetTextBlock(" "));

                    for (int i = _minYear; i <= DateTime.Now.Year; i++)
                    {
                        YearPanel.Children.Add(GetTextBlock($"{i}"));
                    }

                    YearPanel.Children.Add(GetTextBlock(" "));
                    YearPanel.Children.Add(GetTextBlock(" "));
                }

                public void SetMonths()
                {
                    foreach (var month in _months)
                    {
                        MonthPanel.Children.Add(GetTextBlock(month.Value));
                    }
                }

                private int _selectedMonthIndex = 2;
                private void MonthScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
                {
                    if (MonthPanel.Children.Count == 0)
                        return;

                    if (e.Delta < 0)
                        _selectedMonthIndex = Math.Min(MonthPanel.Children.Count - 1 - 2, _selectedMonthIndex + 1);
                    else if (e.Delta > 0)
                        _selectedMonthIndex = Math.Max(2, _selectedMonthIndex - 1);

                    var selectedElement = MonthPanel.Children[_selectedMonthIndex] as UIElement;
                    CenterElementInScrollViewer(selectedElement, MonthScroll);

                    e.Handled = true;

                    chosenMonth = _selectedMonthIndex - 1;
                    SetDays();
                }

                private int _selectedYearIndex = 2;
                private void YearScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
                {
                    if (YearPanel.Children.Count == 0)
                        return;

                    if (e.Delta < 0)
                        _selectedYearIndex = Math.Min(YearPanel.Children.Count - 1 - 2, _selectedYearIndex + 1);
                    else if (e.Delta > 0)
                        _selectedYearIndex = Math.Max(2, _selectedYearIndex - 1);

                    var selectedElement = YearPanel.Children[_selectedYearIndex] as UIElement;
                    CenterElementInScrollViewer(selectedElement, YearScroll);

                    //HighlightSelectedElement(_selectedIndex); 
                    e.Handled = true;
                }*/

        private void CenterElementInScrollViewer(UIElement element, ScrollViewer scroll)
        {
            if (element == null)
                return;

            var transform = element.TransformToAncestor(scroll);
            Point position = transform.Transform(new Point(0, 0));

            double elementCenter = position.Y + ((FrameworkElement)element).ActualHeight / 2;
            double scrollViewerCenter = scroll.ViewportHeight / 2;

            double offset = scroll.VerticalOffset + (elementCenter - scrollViewerCenter);
            scroll.ScrollToVerticalOffset(offset);
        }

        private TextBlock GetTextBlock(string text)
        {
            TextBlock block = new TextBlock()
            {
                Text = text,
                Height = 40,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 16,
                Padding = new Thickness(10),
                Foreground = (SolidColorBrush)System.Windows.Application.Current.Resources["UsualTextColor"]
            };

            block.PreviewMouseDown += ChooseBlock_PreviewMouseDown;

            return block;
        }

        /*        private bool _isSelecting = false;
                private double _baseY;
                private int _baseIndex;*/
        public void ChooseBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            /*            if (e.OriginalSource is TextBlock tb)
                        {
                            _isSelecting = true;
                            Mouse.Capture(this, CaptureMode.Element);

                            _baseY = e.GetPosition(this).Y;
                            _baseIndex = ChatBox.SelectedIndex = ChatBox.Items.IndexOf(tb.Text);
                        }*/

            if (sender is not TextBlock block ||
                string.IsNullOrWhiteSpace(block.Text)) return;
            //CenterElementInScrollViewer(block, GetScrollViewerByTextBlock(block));
        }

        /*        public ScrollViewer GetScrollViewerByTextBlock(TextBlock block)
                {
                    return DaysPanel.Children.Contains(block) ? DaysScroll :
                        MonthPanel.Children.Contains(block) ? MonthScroll :
                        YearScroll;
                }
        */

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            //Make check for date
            int day = DaysSpecial.GetSelectedIndex() - 1;
            int month = MonthsSpecial.GetSelectedIndex() - 1;
            int year = _minYear + YearSpecial.GetSelectedIndex() - 2;

            _user.BirthDay = new DateTime(year, month, day);

            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();
            ((MainWindow)Window.GetWindow(this)).UpdateLoggedUserPage();
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

        private void RemoveBut_Click(object sender, RoutedEventArgs e)
        {
            _user.BirthDay = null;
            ((MainWindow)Window.GetWindow(this)).ClearThirdFrame();

            ((MainWindow)Window.GetWindow(this)).UpdateLoggedUserPage();
        }
    }
}
