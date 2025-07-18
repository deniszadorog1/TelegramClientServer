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

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для DoubleText.xaml
    /// </summary>
    public partial class DoubleText : UserControl
    {
        public DoubleText()
        {
            InitializeComponent();
        }

        public void SetUpperText(string text)
        {
            UpperText.Text = text;
        }

        public void SetBottomText(string text)
        {
            BottomText.Text = text;
        }
    }
}
