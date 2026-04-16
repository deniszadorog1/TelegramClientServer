using FFMpegCore;
using System.Configuration;
using System.Data;
using System.Windows;
using TelegramVisualPart.Helper;


namespace TelegramVisualPart
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MediaServerUrl.Load(); 
        }
    }

}
