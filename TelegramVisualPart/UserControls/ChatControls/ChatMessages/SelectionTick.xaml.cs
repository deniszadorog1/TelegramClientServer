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

namespace TelegramVisualPart.UserControls.ChatControls.ChatMessages
{
    /// <summary>
    /// Логика взаимодействия для SelectionTick.xaml
    /// </summary>
    public partial class SelectionTick : UserControl
    {
        public bool _isChosen = false;
        public event Action StatusChanged;

        public SelectionTick()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isChosen = !_isChosen;
            SetVisPartByStatus();

            StatusChanged?.Invoke();
        }

        public void SetChosenParam(bool isChosen)
        {
            _isChosen = isChosen;
            SetVisPartByStatus();
        }

        public void SetVisPartByStatus()
        {
            if (_isChosen) SetChosenState();
            else SetUnchosenState();
        }

        public void SetUnchosenState()
        {
            ChosenTickIcon.Visibility = Visibility.Hidden;
            ChooseEllipse.Fill =
                (SolidColorBrush)Application.Current.Resources["DarkThemeOne"]; 
            
            //new SolidColorBrush(Colors.Transparent);

            //DarkThemeOne
        }

        public void SetChosenState()
        {
            ChosenTickIcon.Visibility = Visibility.Visible;
            ChooseEllipse.Fill = 
                (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];

        }

    }
}
