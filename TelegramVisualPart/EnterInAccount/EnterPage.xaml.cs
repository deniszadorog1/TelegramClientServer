using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TelegramLib.MainClasses;

using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Contacts;
using TelegramVisualPart.Services;

namespace TelegramVisualPart.EnterInAccount
{
    /// <summary>
    /// Логика взаимодействия для EnterPage.xaml
    /// </summary>
    public partial class EnterPage : Page
    {
        public event EventHandler SetSystemPage;
        public TelSystem _system;

        public EnterPage()
        {
            InitializeComponent();
        }

        private void RegistrationGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Register.TextDecorations = null;
        }

        private void RegistrationGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Register.TextDecorations = TextDecorations.Underline;
        }

        private void RegistrationGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set registration page
            ((MainWindow)Window.GetWindow(this)).SetMainFrameContent(new RegistrationPage());
        }

        private async void EnterBut_Click(object sender, RoutedEventArgs e) 
        {
            //Is field are empty 
            if (string.IsNullOrWhiteSpace(LoginBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Text)) return;

            string token = await ApiService.GetToken(LoginBox.Text, PasswordBox.Text);

            if (!string.IsNullOrEmpty(token))
            {
                ApiService.SetAuthToken(token);
                _system = await ApiService.GetPartlyTelSystem();// await ApiService.GetTelSystem();

                _system.Token = token;

                if (_system is null)
                {
                    MessageBox.Show("Bruh...");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Wrong login or password");
                return;
            }

            SignalRHelperService.SetStatSystem(_system);


            if (_system is null)
            {
                MessageBox.Show("No user with such params");
                ClearBoxes();
                return;
            }

            _system.SetEmptyUserImages();


            await ShitTest();


            bool isOnline = await ApiService.IsUserOnline(_system.LoggedUser.Id);
            if (isOnline)
            {
                MessageBox.Show("User is already online");
                ClearBoxes();
                return;
            }

            //MessageBox.Show("1.I am online. Starting to set basic params");//
            _system.Settings.ChatsSettings.SetBasicThemes();

            _system.Settings.ChatsSettings.PossibleWallpapers =
                FilesAction.GetAllWallpaperNames(_system.Settings.ChatsSettings.PossibleWallpapers);

            //MessageBox.Show("2. Set basic wallpapers!");

            _system.Settings.ChatsSettings.Theme =
                TelegramLib.Enums.Settings.ChatSettings.ThemeType.Night;

            Application.Current.Resources["AppFont"] =
              new FontFamily(_system.Settings.ChatsSettings.FontName);

            await SetOnlineStatus();

            if (_system.Settings.GetChatSettings().Wallpaper is null)
                _system.Settings.GetChatSettings().Wallpaper =
                    new TelegramLib.UserSettings.SettingsTypes.SubSettings.ChatWallpaper();

            Application.Current.Resources["TempActiveTextColor"] =
                new SolidColorBrush(Color.FromRgb(_system.LoggedUser.MainColor.R,
                _system.LoggedUser.MainColor.G, _system.LoggedUser.MainColor.B));


            //MessageBox.Show("3. Eneded of Setting basic params! going to create main page");

            Window window = Window.GetWindow(this);

            if(window is not null && window is MainWindow main) main.SetMainPage(_system);
        }

        private async Task SetOnlineStatus()
        {
            _system.LoggedUser.IsOnline = true;
            await ApiService.SetUserOnlineStatus(_system.LoggedUser.Id, true);
        }

        private void ClearBoxes()
        {
            LoginBox.Text = string.Empty;
            PasswordBox.Text = string.Empty;
        }

        public async Task ShitTest()
        {
/*            const int usersAmount = 100;

            //Add 100 users
            for (int i = 0; i < usersAmount; i++)
            {
                string iS = i.ToString();

                await ApiService.AddNewUser(iS, iS, iS, iS, iS, DateTime.Now);

                int dbId = i + 2;

                await ApiService.AddUserBasicColor(dbId);
                await ApiService.AddUserSettings(dbId);
                await ApiService.AddSavedMessagesChat(dbId);
            }

            int addId = 1;
            //contact To EachOther
            for (int i = 2; i < usersAmount + 1; i++)
            {
                string temp = i.ToString();
                string prev = (i - 1).ToString();

                UserContactcs prevCont = new UserContactcs(-1, prev, prev, prev, DateTime.Now, string.Empty, prev, DateTime.Now, false, null, null, false);
                prevCont.ContactUserId = i;

                UserContactcs tempCont = new UserContactcs(-1, temp, temp, temp, DateTime.Now, string.Empty, temp, DateTime.Now, false, null, null, false);
                tempCont.ContactUserId = addId;


                await ApiService.AddContact(addId, prevCont);
                await ApiService.AddContact(i, tempCont);
            }
*/
            return;
            //Messages In chat
            for (int i = 0; i < _system.Contacts.Count; i++)
            {
                await ApiService.AddNewChat(1, _system.Contacts[i].ContactUserId);

                TelegramLib.MainClasses.UserChat? chat = await ApiService.GetChatByUserAndSenderId(_system.LoggedUser.Id, _system.Contacts[i].ContactUserId);

                for (int j = 0; j < 100; j++)
                {
                    await ApiService.AddMessage(new TelegramLib.MainClasses.Messages.TextMessage(-1, 1, DateTime.Now, "asd", false, -1, false, null, false), chat);
                }
            }

        }
    }
}
