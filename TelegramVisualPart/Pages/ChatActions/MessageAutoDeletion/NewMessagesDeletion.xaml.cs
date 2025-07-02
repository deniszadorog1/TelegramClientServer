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

namespace TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion
{
    /// <summary>
    /// Логика взаимодействия для NewMessagesDeletion.xaml
    /// </summary>
    public partial class NewMessagesDeletion : Page
    {
        public NewMessagesDeletion()
        {
            InitializeComponent();
        }


        private void DaysListBox_Loaded(object sender, RoutedEventArgs e)
        {
/*            // Добавляем дни
            DaysListBox.ItemsSource = new List<string>
            {
                "1 day",
                "2 days",
                "3 days",
                "4 days",
                "5 days"
            };*/

            for (int i = 0; i < 5; i++)
            {
                TextBlock asd = new TextBlock()
                {
                    Text = $"{i} days"
                };
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(new RotateTransform(25)); // вращение по оси Z
                transformGroup.Children.Add(new ScaleTransform(0.8, 0.8)); // уменьшение
               // transformGroup.Children.Add(new PlaneProjection(0.8, 0.8)); // уменьшение

                DaysListBox.LayoutTransform = transformGroup; DaysListBox.Items.Add(asd);
            }

            DaysListBox.SelectedIndex = 2; // Центр — "3 days"
            DaysListBox.ScrollIntoView(DaysListBox.SelectedItem);
        }

        private void DaysListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Опционально: что делать при выборе
            var selected = DaysListBox.SelectedItem as string;
            Console.WriteLine($"Выбрано: {selected}");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ApplySkewToEdgeItems();

        }

        private void ApplySkewToEdgeItems()
        {
            return;
            for (int i = 0; i < DaysListBox.Items.Count; i++)
            {
                var item = DaysListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (item == null)
                    continue;

                var contentPresenter = FindVisualChild<TextBlock>(item);
                if (contentPresenter == null)
                    continue;

                if (i == 0) // "1 day"
                {
                    contentPresenter.LayoutTransform = new SkewTransform(-100, 0);
                    contentPresenter.Opacity = 0.3;
                }
                else if (i == 1 || i == 3) // чуть затемнённые, но не наклон
                {
                    contentPresenter.Opacity = 0.6;
                }
                else if (i == 4) // "5 days"
                {
                    contentPresenter.LayoutTransform = new SkewTransform(100, 0);
                    contentPresenter.Opacity = 0.3;
                }
                else // центральный
                {
                    contentPresenter.LayoutTransform = Transform.Identity;
                    contentPresenter.Opacity = 1.0;
                }
            }
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                    return t;

                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
