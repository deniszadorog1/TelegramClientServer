using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

namespace TelegramVisualPart.Pages.Other
{
    /// <summary>
    /// Логика взаимодействия для InfoMessage.xaml
    /// </summary>
    public partial class InfoMessage : Page
    {
        private string _text;
        
        public InfoMessage(string text)
        {
            _text = text;
            InitializeComponent();

            SetParams();
        }

        public void SetParams()
        {
            InfoText.Text = _text;
        }
    }
}
