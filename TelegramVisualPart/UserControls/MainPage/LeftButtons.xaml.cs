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

namespace TelegramVisualPart.UserControls.MainPage
{
    /// <summary>
    /// Логика взаимодействия для LeftButtons.xaml
    /// </summary>
    public partial class LeftButtons : UserControl
    {
        public LeftButtons()
        {
            InitializeComponent();
        }

        public event EventHandler? OnMenuClick;

        private void ShowMenu_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            OnMenuClick?.Invoke(this, EventArgs.Empty); 
        }
    }
}
