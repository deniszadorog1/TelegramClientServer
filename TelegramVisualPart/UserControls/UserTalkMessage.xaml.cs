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

namespace TelegramVisualPart.UserControls
{
    /// <summary>
    /// Логика взаимодействия для UserTalkMessage.xaml
    /// </summary>
    public partial class UserTalkMessage : UserControl
    {
        public UserTalkMessage()
        {
            InitializeComponent();
        }

        public string GetLastMessageText()
        {
            return LastMessage.Text;
        }
    }
}
