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

namespace TelegramVisualPart.Pages.ChatActions
{
    /// <summary>
    /// Логика взаимодействия для CalendarPage.xaml
    /// </summary>
    public partial class CalendarPage : Page
    {
        public CalendarPage()
        {
            InitializeComponent();
        }

        private void CancelBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void CancelBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.
                            Resources["OtherButMouseEnter"];
        }

        private void CloseBut_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);
        }

        private void SelectDaysBut_Click(object sender, RoutedEventArgs e)
        {
            Calendar.SelectionMode = CalendarSelectionMode.MultipleRange;

            CloseBut.Visibility = Visibility.Hidden;
            CancelBut.Visibility = Visibility.Visible;

            SelectDaysBut.Visibility = Visibility.Hidden;
            ClearHistoryBut.Visibility = Visibility.Visible;
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            Calendar.SelectionMode = CalendarSelectionMode.None;

            CloseBut.Visibility = Visibility.Visible;
            CancelBut.Visibility = Visibility.Hidden;

            SelectDaysBut.Visibility = Visibility.Visible;
            ClearHistoryBut.Visibility = Visibility.Hidden;
        }

        private void ClearHistoryBut_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background =
                            (SolidColorBrush)Application.Current.Resources["CloseButBg"];
        }

        private void ClearHistoryBut_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button but) but.Background = Brushes.Transparent;
        }

        private void ClearHistoryBut_Click(object sender, RoutedEventArgs e)
        { 
            ((MainWindow)Window.GetWindow(this)).ClearTempPageFrame(this);

            List<DateTime> chosenDates = Calendar.SelectedDates.ToList();
            if (chosenDates is null || chosenDates.Count == 0) return;

            ((MainWindow)Window.GetWindow(this)).RemoveMessagesByDates(chosenDates);
        }
    }
}
