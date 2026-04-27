using FFMpegCore;
using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Windows;
using TelegramVisualPart.Helper;


namespace TelegramVisualPart
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            MediaServerUrl.Load();

            await PierceNgrokShield();
        }

        private async Task PierceNgrokShield()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(MediaServerUrl.Url);
                    client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
                    // Прикидываемся обычным браузером
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                    var response = await client.GetAsync("");

                    if (response.IsSuccessStatusCode)
                        System.Diagnostics.Debug.WriteLine("Ngrok shield pierced!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to pierce ngrok: {ex.Message}");
            }
        }
    }

}
