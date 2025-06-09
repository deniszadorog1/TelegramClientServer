using System;
using System.Collections.Generic;
using System.IO;
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

namespace TelegramVisualPart.UserControls.SettingsControls.ChatSettingsControls.PaletteControls
{
    /// <summary>
    /// Логика взаимодействия для PaletteRectangle.xaml
    /// </summary>
    public partial class PaletteRectangle : UserControl
    {
        Cursor _fanCurs;
        public PaletteRectangle()
        {
            InitializeComponent();

            string dir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            dir += "\\Visuals\\Cursors\\tungTungSahurCursor.cur";
            _fanCurs = new Cursor(dir);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = _fanCurs;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }
    }
}
