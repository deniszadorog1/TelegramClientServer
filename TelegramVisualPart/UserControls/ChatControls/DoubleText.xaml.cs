using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using static System.Net.Mime.MediaTypeNames;

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

        public void SetActions()
        {
            UpperTextGrid.MouseEnter += UpperText_MouseEnter;
            UpperTextGrid.MouseLeave += UpperText_MouseLeave;

            UpperTextGrid.PreviewMouseLeftButtonDown += 
                UpperTextGrid_PreviewMouseLeftButtonDown;
        }

        public void UpperText_MouseEnter(object sender, MouseEventArgs e)
        {
            UpperText.TextDecorations = TextDecorations.Underline;
            Cursor = Cursors.Hand;
        }

        public void UpperText_MouseLeave(object sender, MouseEventArgs e)
        {
            UpperText.TextDecorations = null;
            Cursor = null;
        }

        public void UpperTextGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set Click event
            SetClipboardTextSafe(UpperText.Text);

            Window window = Window.GetWindow(this);
            if(window is MainWindow main)
            {
                main.SetTemporaryText("Username copied to clipboard");
            }
        }

        private static void SetClipboardTextSafe(string text, int retryCount = 5)
        {
            if (Clipboard.GetText() == text) return;
                for (int i = 0; i < retryCount; i++)
            {
                try
                {
                   Clipboard.SetText(text);
                   return;
                }
                catch (COMException)
                {
                    return; 
                }
            }
        }

    }
}
