using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TelegramVisualPart.UserControls.ContactsControls
{
    /// <summary>
    /// Логика взаимодействия для SentLinkControl.xaml
    /// </summary>
    public partial class SentLinkControl : UserControl
    {
        private string _name;
        private string _description;
        private string _link;

        public SentLinkControl(string name, string description, string link)
        {
            _name = name;
            _description = description;
            _link = link;

            InitializeComponent();

            SetBasicParams();
        }

        public void SetBasicParams()
        {
            SiteName.Text = _name;
            DescriptionBlock.Text = _description;
            LinkBlock.Text = _link;

            LikeImgTextBlock.Text = _name.First().ToString();

            SetSizes();
        }

        public void SetSizes()
        {
            if(DescriptionBlock.Text is null || 
                DescriptionBlock.Text == string.Empty)
            {
                DescriptionRow.Height = new GridLength(0);
            }
        }

        private void LinkBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not TextBlock block) return;

            block.TextDecorations = TextDecorations.Underline;
            Cursor = Cursors.Hand;
        }

        private void LinkBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not TextBlock block) return;

            block.TextDecorations = null;
            Cursor = null;
        }

        private void LinkBlock_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TextBlock block) return;

            try
            {
                Process.Start(new ProcessStartInfo(block.Text)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Smth wrong with your url!!");
            }
        }
    }
}
