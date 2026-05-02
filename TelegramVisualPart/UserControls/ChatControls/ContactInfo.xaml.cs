using MaterialDesignThemes.Wpf;
using System;
using System.CodeDom;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Xml.Linq;
using TelegramLib.MainClasses;
using TelegramLib.MainClasses.Messages;
using TelegramLib.Models;
using TelegramLib.UserSettings;
using TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs;
using TelegramVisualPart.Enums;
using TelegramVisualPart.Helper;
using TelegramVisualPart.Pages.Contacts;
using TelegramVisualPart.Pages.UserInfoContact.ActionsFolder;
using TelegramVisualPart.Pages.VisualPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls.ContactInfoControls;
using TelegramVisualPart.UserControls.ChatControls.UserContactControls;
using TelegramVisualPart.UserControls.ChatsControls;
using TelegramVisualPart.Windows;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace TelegramVisualPart.UserControls.ChatControls
{
    /// <summary>
    /// Логика взаимодействия для ContactInfo.xaml
    /// </summary>
    public partial class ContactInfo : UserControl
    {
        private TelegramLib.MainClasses.UserChat _chat;
        private TelSystem _system;

        public TelegramLib.MainClasses.UserContactcs _contact;

        public event Action SendMesPressed;

        public ContactInfo()
        {
            InitializeComponent();
            SetIconsSize();

            this.Visibility = Visibility.Hidden;
            SetMenuVisibility(Visibility.Hidden);
        }

        public void SetMenuVisibility(Visibility vis)
        {
            MenuButGrid.Visibility = vis;
        }

        public event Action LoadEnd;
        private bool _isSetMaxHeight;
        private bool _isUpdate;

        public async Task SetContactInfo(TelegramLib.MainClasses.UserChat chat,
            TelSystem system, TelegramLib.MainClasses.UserContactcs contact,
            bool isSetMaxHeight = true)
        {
            _system = system;
            _chat = chat;
            _contact = contact;
            _isSetMaxHeight = isSetMaxHeight;

            if(_chat is not null && _chat.Chatter is not null) await ApiService.UpdateChachedUserAndSettings(chat.Chatter.Id);

            if (!_isSetMaxHeight) MaxHeight = int.MaxValue;

            SetBasicRowHeight();
            await SetInfoVisibility();

            SignalRService.UpdateContactDel += UpdateContactParams;
            SignalRService.UpdateOnlineStatusDel += UpdateOnlineStatus;
            //SignalRService.SetContactPhoneNumberVisibilityDel += SetPhoneNumberVisAction;

            SignalRService.SetContactLastSeenVisStateDel += SetLastSeenState;
            SignalRService.SetPhoneNumVisByExpsDel += SetPhoneNumberVisByExps;
            SignalRService.UpdateBirthDateDel += UpdateBirthDate;
            SignalRService.UpdateContactPhotoDel += UpdateContactPhoto;
            SignalRService.UpdateContactBioDel += UpdateContactBio;

            SignalRService.MediaMessageReceived += ReceivedMedias;
            SignalRService.DeleteMessageByIdDel += RemovedMedia;
            SignalRService.RemoveManyMessagesDel += RemovedMedia;


            ContactMenu.UnblockUser += SetUnBlockAction;

            this.Visibility = Visibility.Visible;

            SetLanguageText.SetContactInfo(this);

            BlockButVisibility();
            SetStartToggleState();

            LoadEnd?.Invoke();
        }

        private const int _baseUpperInfoRowHeight = 55;
        public void SetBasicRowHeight()
        {
            BirthdatRow.Height = new GridLength(_baseUpperInfoRowHeight);
            // ToBeHiddenButs.Height = new GridLength(200);
            //BioRow.Height = new GridLength(_baseUpperInfoRowHeight);
            //AddContactRow.Height = new GridLength(_baseUpperInfoRowHeight);
        }

        public void ReceivedMedias(TelegramLib.MainClasses.User user, List<MediaAction> medias)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                await SetInfoVisibility();
            });
        }

        public void RemovedMedia(TelegramLib.MainClasses.User logged,
            TelegramLib.MainClasses.Messages.Message mes, bool isUpdate)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (_chat is not null && _chat.Chatter is not null && _chat.Chatter.Id == logged.Id)
                {
                    //_chat.RemoveMessagesByList(medias.Cast<TelegramLib.MainClasses.Messages.Message>().ToList());
                    TelegramLib.MainClasses.Messages.Message? pair =
                        await ApiService.GetPairOfMessage(mes);

                    if (pair is not null) _chat.RemoveMessageById(pair.Id);
                }
                await SetInfoVisibility();
            });
        }

        public void RemovedMedia(List<DateTime> dates, int contId)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (_chat is not null && _chat.Chatter is not null && _chat.Chatter.Id == contId)
                {
                    for (int i = 0; i < dates.Count; i++)
                    {
                        //_chat.Messages.Remove(_chat.GetMessageByDateTime(dates[i]));
                    }
                }
                await SetInfoVisibility();
            });
        }

        public void SetUnBlockAction()
        {
            SetBlockStatus();
        }

        public void SetStartToggleState()
        {
            NotificationToggle.IsChecked = _chat.GetNotificationStatus();
        }

        public async Task SetInfoVisibility()
        {
            SetBlocksVisibility();
            await SetUserParams();
        }
        public void BlockButVisibility()
        {
            if (_chat is TelegramLib.MainClasses.SavedMessagesChat)
            {
                BlockRow.Height = new GridLength(0);
                return;
            }
            bool isBlocked = _system.LoggedUser.IsUserIsBlockedById(_chat.Chatter.Id);
            BlockContactBlock.Text = isBlocked ? "Unblock contact" : "Block contact";
        }

        public void SetBlocksVisibility()
        {
            SetIsContactRemovedVis();

            SetMediasGridsVisibility();
        }

        public void SetInfoLinesVisibility()
        {
            if (/*Birthdate.UpperText.Text ==
                VisConstParamsJsonService.GetStringByName("CantSeeStuff") ||*/

                Birthdate.UpperText.Text ==
                VisConstParamsJsonService.GetStringByName("BirthDatNotSet"))
            {
                InfoRow.Height = new GridLength(
                    InfoRow.Height.Value -
                    BirthdatRow.Height.Value);

                MaxHeight -= BirthdatRow.Height.Value;
                BirthdatRow.Height = new GridLength(0);
            }
        }

        private const int _mediaRowHeight = 50;
        public void SetMediasGridsVisibility()
        {
            List<MediaAction> medias = _chat.GetMediaMessages();

            //is photo amount == 0
            if (FilesAction.GetImagesFromMediaAction(medias) == 0)
            {
                PhotosLine.Visibility = Visibility.Hidden;
                MaxHeight -= PhotoRow.Height.Value;
                PhotoRow.Height = new GridLength(0);
            }
            else if (PhotoRow.Height.Value == 0)
            {
                PhotosLine.Visibility = Visibility.Visible;
                MaxHeight += _mediaRowHeight;
                PhotoRow.Height = new GridLength(_mediaRowHeight);
            }


            //is videos amount == 0
            if (FilesAction.GetVideosAmount(medias) == 0)
            {
                VideosLine.Visibility = Visibility.Hidden;
                MaxHeight -= VideosRow.Height.Value;
                VideosRow.Height = new GridLength(0);
            }
            else if (VideosRow.Height.Value == 0)
            {
                VideosLine.Visibility = Visibility.Visible;
                MaxHeight += _mediaRowHeight;
                VideosRow.Height = new GridLength(_mediaRowHeight);
            }

            //is gifs amount == 0
            if (FilesAction.GetGifsAmount(medias) == 0)
            {
                GifLine.Visibility = Visibility.Hidden;
                MaxHeight -= GifRow.Height.Value;
                GifRow.Height = new GridLength(0);
            }
            else if (GifRow.Height.Value == 0)
            {
                GifLine.Visibility = Visibility.Visible;
                MaxHeight += _mediaRowHeight;
                GifRow.Height = new GridLength(_mediaRowHeight);
            }

            if (_chat.GetLinksAmount() == 0)
            {
                LinkLine.Visibility = Visibility.Hidden;
                MaxHeight -= LinkRow.Height.Value;
                LinkRow.Height = new GridLength(0);
            }
            else if (LinkRow.Height.Value == 0)
            {
                LinkLine.Visibility = Visibility.Visible;
                MaxHeight += _mediaRowHeight;
                LinkRow.Height = new GridLength(_mediaRowHeight);
            }

            if (GifLine.Visibility == Visibility.Hidden &&
                VideosLine.Visibility == Visibility.Hidden &&
                PhotosLine.Visibility == Visibility.Hidden &&
                LinkLine.Visibility == Visibility.Hidden)
            {
                BottomDivideLine.Visibility = Visibility.Hidden;
            }
            else BottomDivideLine.Visibility = Visibility.Visible;

            SetMediasRowVisibility();
            SetAddContactVisibility();
        }

        public void SetAddContactVisibility()
        {
            if (_chat is TelegramLib.MainClasses.SavedMessagesChat) return;

            bool isContact = _system.IsChatterIdIsContact(_chat.Chatter.Id);

            if (isContact)
            {
                AddContactRow.Height = new GridLength(0);
                //AddContactBlock.Visibility = Visibility.Hidden;
                //MaxHeight -= InfoRow.Height.Value - 260;
                InfoRow.Height = new GridLength(280);
                return;
            }
            AddContactRow.Height = new GridLength(50);
            //AddContactBlock.Visibility = Visibility.Visible;
            //MaxHeight += 330 - InfoRow.Height.Value;
            InfoRow.Height = new GridLength(330);
        }

        public void SetMediasRowVisibility()
        {
            MediasRow.Height = new GridLength(
                PhotoRow.Height.Value + VideosRow.Height.Value +
                GifRow.Height.Value + LinkRow.Height.Value + 10);

            MaxHeight += 5;

            if (BottomDivideLine.Visibility == Visibility.Hidden)
            {
                MaxHeight -= 10;
                DivRow.Height = new GridLength(0);
            }
            else DivRow.Height = new GridLength(10);

            if (MediasRow.Height.Value == 0) DivRow.Height = new GridLength(0);
        }

        public const int _hiddenParasHeight = 150;
        public void SetIsContactRemovedVis()
        {
            /*if (!_isSetMaxHeight) return;*/
            if (_contact is null)
            {
                //Hide lines
                ShareRow.Height = new GridLength(0);
                EditRow.Height = new GridLength(0);
                DeleteRow.Height = new GridLength(0);

                //Set page height
                ToBeHiddenButs.Height = new GridLength(50);

                MaxHeight -= _hiddenParasHeight;
            }
            else if (ToBeHiddenButs.Height.Value != 200) //lines are not hidden
            {
                ShareRow.Height = new GridLength(50);
                EditRow.Height = new GridLength(50);
                DeleteRow.Height = new GridLength(50);

                //Set page height
                ToBeHiddenButs.Height = new GridLength(200);

                MaxHeight += _hiddenParasHeight;
            }

            if (_chat is TelegramLib.MainClasses.SavedMessagesChat)
            {
                ShareRow.Height = new GridLength(50);
                ToBeHiddenButs.Height = new GridLength(75);
            }
        }

        public int GetHiddenParamsHeight()
        {
            return _hiddenParasHeight;
        }

        public void UpdateContactBio(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (_chat is null || _chat.Chatter is null) return;
                if (_chat.Chatter.Id == user.Id)
                {
                    await SetBioRow(user);
                }
            });
        }

        public void UpdateContactPhoto(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (user.Id == _system.LoggedUser.Id && _chat is not TelegramLib.MainClasses.SavedMessagesChat) return;

                await SignalRHelperService.SetContactPhoto(user,
                    _chat, ContactImgBrush, UserIcon);
            });
        }

        public void UpdateBirthDate(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (user.Id == _system.LoggedUser.Id) return;

                _system.UpdateUserBirthdate(user);
                await SignalRHelperService.SetBirthDate(user, _chat, Birthdate.UpperText);
            });
        }



        public void SetPhoneNumberVisByExps(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (user.Id == _system.LoggedUser.Id) return;
                await SetUserPhoneNumber(user);
            });
        }

        public async Task SetUserPhoneNumber(TelegramLib.MainClasses.User contactUser)
        {
            if (_chat is null || _chat.GetChatter() is null || _chat.GetChatter().Id != contactUser.Id) return;

            IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(contactUser, Enums.PrivacySettingType.PhoneNumber);

            await SignalRHelperService.SetPhoneNumber(contactUser, shareType, _chat, MobileNumber.UpperText);
        }

        public void SetLastSeenState(TelegramLib.MainClasses.User user)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (user.Id == _system.LoggedUser.Id) return;
                if (_chat.GetChatter().Id != user.Id) return;

                IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.LastSeen);

                await SignalRHelperService.SetLastSeenString(user, shareType, _chat, LastSeenOnline);
            });
        }

        public void SetPhoneNumberVisAction(bool isVis, TelegramLib.MainClasses.User updatedUser)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                await SetUserPhoneNumber(updatedUser);
            });
        }

        public void UpdateOnlineStatus(TelegramLib.MainClasses.User toUpdate)
        {
            Dispatcher.Invoke(() =>
            {
                if (_chat is null ||
               (_chat is not TelegramLib.MainClasses.SavedMessagesChat &&
               _chat.GetChatter().Id != toUpdate.Id)) return;

                HelperService.SetOnlineStatusInTextBox(LastSeenOnline, toUpdate.IsOnline, toUpdate.LastSeenOnline);
            });
        }

        private void UpdateContactParams(TelegramLib.MainClasses.User updated)
        {
            Dispatcher.InvokeAsync(async () =>
            {
                if (_contact is null || _contact.ContactUserId != updated.Id) return;

                //Username.Text = updated.Login;
                ContName.Text = updated.Name;
                ContSurname.Text = updated.Surname;

                await SetUserPhoneNumber(updated);
                //MobileNumber.UpperText.Text = updated.PhoneNumber;

                UserName.UpperText.Text = updated.Name;

                await SetBioRow(updated);

                Birthdate.UpperText.Text = updated.BirthDay is null ? VisConstParamsJsonService.GetStringByName("BirthdayNeverBeen") :
                $"{updated.BirthDay.Value.Day}.{updated.BirthDay.Value.Month}.{updated.BirthDay.Value.Year}";
            });
        }

        private const int _addInfoRowHeight = 55;
        private const int _baseInfoRowHeight = 280;

        public async Task SetBioRow(TelegramLib.MainClasses.User toUpdate,
            MainSettings settings = null)
        {
            if (toUpdate.BIO is null || toUpdate.BIO == string.Empty)
            {
                UpdateSizeWithBioRow(toUpdate);
                BioRow.Height = new GridLength(0);
                return;
            }
            else
            {
                UpdateSizeWithBioRow(toUpdate);
                BioRow.Height = new GridLength(_addInfoRowHeight);

                Bio.UpperText.Text = "Bio";
            }

            IsPrivacyException shareType =
                await SignalRHelperService.GetTypeByUser(toUpdate, Enums.PrivacySettingType.Bio, settings: settings);


            if (settings is null) settings = await SignalRHelperService.GetMainSettings(toUpdate, settings);

            bool isStop = await SignalRHelperService.IsAndSetStopPath(Enums.PrivacySettingType.Bio, toUpdate,
                settings: settings);

            if (shareType == IsPrivacyException.Share)
            {
                Bio.BottomText.Text = toUpdate.BIO;
                return;
            }

            if (settings.PrivacySettings.BioPrivacy.ShareType ==
                TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||
                shareType == IsPrivacyException.NeverShare)
            {
                Bio.BottomText.Text = VisConstParamsJsonService.GetStringByName("CantSeeStuff");
                return;
            }

            if (isStop)
            {
                Bio.BottomText.Text = VisConstParamsJsonService.GetStringByName("CantSeeStuff");
                return;
            }

            Bio.BottomText.Text = $"{toUpdate.BIO}";
        }

        public void UpdateSizeWithBioRow(TelegramLib.MainClasses.User user)
        {
            if (user.BIO is null || user.BIO == string.Empty)
            {
                if (BioRow.Height.Value == 0)
                {
                    InfoRow.Height = new GridLength(InfoRow.Height.Value);
                    return;
                }
                InfoRow.Height = new GridLength(InfoRow.Height.Value - _addInfoRowHeight);
            }
            else
            {
                if (BioRow.Height.Value != 0) return;
                InfoRow.Height = new GridLength(InfoRow.Height.Value + _addInfoRowHeight);
            }
        }

        TelegramLib.MainClasses.User? _chatterUser;
        private async Task SetUserParams()
        {
            _chatterUser =
                await GetChatterUser();
            if (_chatterUser is null) return;

            SetNameSurnameParams();

            MainSettings settings = await ApiService.GetSettingsByUserId(_chatterUser.Id);

            await SetOnlineStatus(settings);
            //SetLastSeenOnline();
            await SetMobilePhoneNumber(settings);
            await SetBirtDate(settings);

            await SetBioRow(_chat is TelegramLib.MainClasses.SavedMessagesChat ? _chatterUser : _chat.Chatter, settings: settings);

            UserName.SetUpperText(_chatterUser.Login);
            UserName.UpperText.Foreground = (SolidColorBrush)Application.Current.Resources["TempActiveTextColor"];
            UserName.SetUsernameActions();
            UserName.UpperText.PreviewMouseRightButtonDown +=
                UserNameUpperTextGrid_PreviewMouseRightButtonDown;

            SentObjsParams();

            await SetContactPhoto(settings);

            AddFoldersSubMenu();
        }

        public void UserNameUpperTextGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_chatterUser is null) return;
            MenuGrid.Children.Clear();

            LoginRowMenu menu = new LoginRowMenu(_chatterUser.Login);
            menu.TextCopied += () =>
            {
                MenuGrid.Children.Clear();
            };

            Point tempPos = e.GetPosition(this);

            Canvas.SetLeft(menu, tempPos.X);
            Canvas.SetTop(menu, tempPos.Y);

            MenuGrid.Children.Add(menu);
        }

        public void AddFoldersSubMenu()
        {
            ContactMenu.AddToFolder.MouseEnter += (sender, e) =>
            {
                Point relativePoint = e.GetPosition(this);
                Point point = new Point(relativePoint.X, relativePoint.Y - 15);

                UserContactMenu foldMenu = new UserContactMenu();
                foldMenu.SetTelSystemParam(_system, _chat);

                foldMenu.SetFoldersParams();

                ContactMenusCanvas.Children.Add(foldMenu);

                foldMenu.MouseLeave += (sender, e) =>
                {
                    ContactMenusCanvas.Children.Remove(foldMenu);
                };

                ContactMenusCanvas.MouseLeave += (sender, e) =>
                {
                    ContactMenusCanvas.Children.Remove(foldMenu);
                };

                ContactMenu.ClearThis += () =>
                {
                    ContactMenusCanvas.Children.Remove(foldMenu);
                };

                foldMenu.ClearThis += () =>
                {
                    ContactMenusCanvas.Children.Remove(foldMenu);
                };

                Canvas.SetLeft(foldMenu, Canvas.GetLeft(ContactMenu) - foldMenu.Width);
                Canvas.SetTop(foldMenu, Canvas.GetTop(ContactMenu) + ContactMenu.GetAddFolderButPos());
            };
        }

        public void AddMenuElement(UserChatMenu menu, Point cordPoint)
        {
            //MenusCan.Children.Add(menu);

            /*            Window window = Window.GetWindow(menu);
                        if (window is null ||
                            window is not MainWindow) throw new Exception("Its should be Main Window");

                        menu.SetWindow(window as MainWindow);*/

            Canvas.SetLeft(menu, cordPoint.X + 100);
            Canvas.SetTop(menu, cordPoint.Y);
        }



        public void SetNameSurnameParams()
        {

            if (_contact is null)
            {
                TelegramLib.MainClasses.User user = GetUserToSetParams();
                ContName.Text = user.Name;
                ContSurname.Text = user.Surname;
            }
            else
            {
                ContName.Text = _contact.Name;
                ContSurname.Text = _contact.Surname;
            }
        }

        private TelegramLib.MainClasses.User GetUserToSetParams()
        {
            return _chat is TelegramLib.MainClasses.SavedMessagesChat ? _system.LoggedUser : _chat.Chatter;
        }

        public string GetChatterName(int userId)
        {
            UserContactcs contact = _system.GetContactByUserId(_chat.GetChatter().Id);
            return contact is not null ? contact.Name : _chat.GetChatter().Name;
        }

        public async Task SetContactPhoto(MainSettings settings = null)
        {
            //TelegramLib.MainClasses.User? user = await GetChatterUser();
            if (_chatterUser is null /*|| _chat.GetChatter().Id != _chatterUser.Id*/) return;

            //Check for mask
            _chatterUser = _chat is TelegramLib.MainClasses.SavedMessagesChat ? _system.LoggedUser : _chat.GetChatter();

            await SignalRHelperService.SetContactPhoto(_chatterUser,
                _chat, ContactImgBrush, UserIcon, settings: settings);
        }

        public event Action UpdateAction;

        public async Task SetBirtDate(MainSettings settings)
        {
            TelegramLib.MainClasses.User? user = _chat is TelegramLib.MainClasses.SavedMessagesChat ? _system.LoggedUser : _chat.Chatter;//await GetChatterUser();
            if (user is null) return;

            //IsPrivacyException shareType = await SignalRHelperService.GetTypeByUser(user, Enums.PrivacySettingType.DateBirth);

            await SignalRHelperService.SetBirthDate(user, _chat, Birthdate.UpperText, settings: settings);

            SetInfoLinesVisibility();

            UpdateAction?.Invoke();
        }

        public async Task SetMobilePhoneNumber(MainSettings settings = null)
        {
            if (_chatterUser is null) return;

            IsPrivacyException shareType =
                await SignalRHelperService.GetTypeByUser(_chatterUser, Enums.PrivacySettingType.PhoneNumber, settings: settings);

            await SignalRHelperService.SetPhoneNumber(_chatterUser, shareType, _chat,
                MobileNumber.UpperText, settings: settings);

            //MobileNumber.SetBottomText("Mobile");
        }

        public async Task SetOnlineStatus(MainSettings settings)
        {
            //if (_chat is null) return;
            /*TelegramLib.MainClasses.User? user =
                await GetChatterUser();*/ //await ApiService.GetUserById(_chat.GetChatter().Id);
            if (_chatterUser is null) return;

            IsPrivacyException shareType =
                await SignalRHelperService.GetTypeByUser(_chatterUser, Enums.PrivacySettingType.LastSeen, settings: settings);

            await SignalRHelperService.SetLastSeenString(_chatterUser, shareType, _chat, LastSeenOnline, settings: settings);
        }

        public async Task<TelegramLib.MainClasses.User?> GetChatterUser()
        {
            if (_chat is null) return null;
            TelegramLib.MainClasses.User user = _chat is TelegramLib.MainClasses.SavedMessagesChat ? _system.LoggedUser :
                await ApiService.GetUserById(_chat.GetChatter().Id);
            return user;
        }

        private void SetOfflineStatus()
        {
            LastSeenOnline.Text = _chat.GetLastSeen();
            LastSeenOnline.Foreground = new SolidColorBrush(Colors.Gray);
        }

        public void SentObjsParams()
        {
            List<MediaAction> medias = _chat.GetMediaMessages();

            SetTextForTextBlock(AmountOfPhotosTextBlock,
                FilesAction.GetImagesFromMediaAction(medias),
                VisConstParamsJsonService.GetStringByName("AmountOfPhotosTextBlock"));

            SetTextForTextBlock(AmountOfVideosTextBlock,
                FilesAction.GetVideosAmount(medias),
                VisConstParamsJsonService.GetStringByName("AmountOfVideosTextBlock"));

            SetTextForTextBlock(AmountOfGifsTextBlock,
                FilesAction.GetGifsAmount(medias),
                VisConstParamsJsonService.GetStringByName("AmountOfGifsTextBlock"));

            SetTextForTextBlock(AmountOfLinksTextBlock,
                _chat.GetLinksAmount(), "Amount of Links: ");
        }

        private void SetTextForTextBlock(TextBlock block, int amount, string baseString)
        {
            string amountStr = amount > 0 ? amount.ToString() + " " : string.Empty;
            block.Text = $"{amountStr}{baseString}";
        }

        private void SetIconsSize()
        {
            SetIconSize(InfoIcon);
            SetIconSize(BellIcon);

            SetIconSize(ImageIcon);
            SetIconSize(VideoIcon);
            //SetIconSize(FileIcon);
            //SetIconSize(LinkIcon);
            SetIconSize(GifIcon);

            SetIconSize(SendIcon);
            SetIconSize(PenIcon);
            SetIconSize(CanIcon);
            SetIconSize(HandIcon);

            ContactMenu.Margin = new Thickness(
                0,
                UpperRow.Height.Value + 10,
                0,
                0
                );
        }

        private const int _iconWidth = 30;
        private const int _iconHeight = 30;
        private void SetIconSize(PackIcon icon)
        {
            icon.Width = _iconWidth;
            icon.Height = _iconHeight;
        }

        private void But_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            if (sender is Grid grid)
            {
                SetBgToGrid(grid);
            }
            else
            {
                Grid el = GetGrid(sender as FrameworkElement);
                if (el is not null) SetBgToGrid(el);
            }
        }

        private void But_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid)
            {
                ClearGridBg(grid);
            }
            else
            {
                Grid el = GetGrid(sender as FrameworkElement);
                if (el is not null) ClearGridBg(el);
            }

            Cursor = null;
        }

        public Grid? GetGrid(FrameworkElement el)
        {
            return
                el == SendIcon || el == ShareContactBlock ? ShareLine :
                el == PenIcon || el == EditContactBlock ? EditContactLine :
                el == CanIcon || el == DeleteContactBlock ? DeleteLine :
                el == HandIcon || el == BlockContactBlock ? BlockLine :

                el == ImageIcon || el == AmountOfPhotosTextBlock ? PhotosLine :
                el == VideoIcon || el == AmountOfVideosTextBlock ? VideosLine :
                el == GifIcon || el == AmountOfGifsTextBlock ? GifLine :
                el == SendMesBlock ? SendMessageBut : null;

        }

        public void SetBgToGrid(Grid grid)
        {
            Cursor = Cursors.Hand;
            grid.Background =
            (SolidColorBrush)Application.Current.Resources["DarkThemeMouseEnterBut"];
        }

        public void ClearGridBg(Grid grid)
        {
            Cursor = null;
            grid.Background = Brushes.Transparent;
        }

        private void SendMessageBut_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //Set User chat 
            if (_chat is null) return;

            int chatterId = _chat is TelegramLib.MainClasses.SavedMessagesChat ?
                _system.LoggedUser.Id : _chat.GetChatter().Id;

            ((MainWindow)Window.GetWindow(this))
                .SetOtherChatByUserId(chatterId);

            SendMesPressed?.Invoke();
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void CloseButGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            CloseBut.Foreground = Brushes.White;
        }

        private void CloseButGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            CloseBut.Foreground = Brushes.Gray;
        }

        private void CloseButGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).ClearSecFrame();
        }

        private void MenuButGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            MoreInfoBut.Foreground = Brushes.White;
        }

        private void MenuButGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
            MoreInfoBut.Foreground = Brushes.Gray;
        }

        private void BlockLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetBlockStatus();
        }

        public async void SetBlockStatus()
        {
            if (_chat is TelegramLib.MainClasses.SavedMessagesChat) return;

            bool isBlocked = _system.LoggedUser.IsUserIsBlockedById(_chat.Chatter.Id);

            if (isBlocked)//unblock action
            {
                // unblock in db
                await ApiService.RemoveBlockedContact(_system.LoggedUser.Id, _chat.Chatter.Id);

                //unblock in system
                _system.LoggedUser.UnblockUserById(_chat.Chatter.Id);
                await ((MainWindow)Window.GetWindow(this)).SetBlockedUserVisParams(false, _chat.Chatter);
            }
            else
            {
                //Set to block page
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(
                    new Pages.UserInfoContact.ActionsFolder.BlockContact(_system, _chat.GetChatter()));
            }
            ((MainWindow)Window.GetWindow(this)).SetFramesAfterBlockingContact();
        }

        private void DeleteLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(
                new Pages.UserInfoContact.ActionsFolder.DeleteContact(_contact, _system));
        }

        private void EditContactLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame
                (new Pages.UserInfoContact.ActionsFolder.
                EditUserContact(_system.LoggedUser, _contact, _system));
        }

        private void ShareLine_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(
                new Pages.UserInfoContact.ActionsFolder.ShareContact(_system, _contact));
        }

        private void Line_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_chat is null) return;
            if (sender is FrameworkElement element)
            {
                ((MainWindow)Window.GetWindow(this)).SetThirdFrame(new Pages.UserInfoContact.
                    SentObjectsUserInfo.SentItemsUserContact(
                    ((MainWindow)Window.GetWindow(this)).GetSystem(),
                    GetItemType(element.Name), _chat));
            }
        }

        private Enums.SentItemsTypes GetItemType(string name)
        {
            return name == PhotosLine.Name.ToString() || name == ImageIcon.Name.ToString() || name == AmountOfPhotosTextBlock.Name.ToString() ? Enums.SentItemsTypes.Photos :
                name == VideosLine.Name.ToString() || name == VideoIcon.Name.ToString() || name == AmountOfVideosTextBlock.Name.ToString() ? Enums.SentItemsTypes.Video :
                name == GifLine.Name.ToString() || name == GifIcon.Name.ToString() || name == AmountOfGifsTextBlock.Name.ToString() ? Enums.SentItemsTypes.GIFs :
                name == LinkLine.Name.ToString() ? Enums.SentItemsTypes.SharedLinks :
                Enums.SentItemsTypes.Photos;
        }

        private bool _isMenuOpen = false;

        private void MenuButGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMenuOpen = !_isMenuOpen;

            if (_chat is not null &&
                _chat is TelegramLib.MainClasses.SavedMessagesChat)
            {
                ContactMenu.RemoveParamsIfIsSavedMessagesChat();
            }

            if (_isMenuOpen)
            {
                ContactMenu.Visibility = Visibility.Visible;

                ContactMenu.SetTelSystemParam(_system, _chat);
                ContactMenu.UpdateParamsIsChatterIsNotContact();

                //Set 
                SetMenuPosition(e.GetPosition(this));
            }
            else
            {
                ContactMenu.Visibility = Visibility.Hidden;
                RemoveSubMenus();
            }
        }

        public void RemoveSubMenus()
        {
            List<UIElement> menus = new List<UIElement>();

            for (int i = 0; i < ContactMenusCanvas.Children.Count; i++)
            {
                if (ContactMenusCanvas.Children[i] != ContactMenu)
                {
                    menus.Add(ContactMenusCanvas.Children[i]);
                }
            }

            foreach (var el in menus)
            {
                ContactMenusCanvas.Children.Remove(el);
            }
        }

        public void SetMenuPosition(Point relativePoint)
        {
            Point point = new Point(relativePoint.X, relativePoint.Y);

            Canvas.SetLeft(ContactMenu, point.X - ContactMenu.Width / 1.3);
            Canvas.SetTop(ContactMenu, point.Y - 50);
        }

        private async void NotificationToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_chat is null) return;
            await ApiService.ChangeNotificationState(_chat.Id, true);
            _chat.ChangeNotificationStatus(true);
        }

        private async void NotificationToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_chat is null) return;
            await ApiService.ChangeNotificationState(_chat.Id, false);
            _chat.ChangeNotificationStatus(false);
        }

        private async void UserIcon_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TelegramLib.MainClasses.User user =
                _chat is TelegramLib.MainClasses.SavedMessagesChat ?
                _system.LoggedUser : _chat.GetChatter();

            if (!await IsCanShowMediaWindow(user)) return;
            
            //Set window here
            MediaWindow mediaWindow = new MediaWindow(
                user, (MainWindow)Window.GetWindow(this),
                Enums.MediaShow.MediaShowType.OtherUserImages, _system);

            //Is exist
            if (((MainWindow)Window.GetWindow(this))
                .IsMediaWindowIsExistByUserId(user.Id)) return;

            mediaWindow.Show();
        }

        public async Task<bool> IsCanShowMediaWindow(TelegramLib.MainClasses.User user)
        {
            if (user.ImageMask is not null) return true;

            if (_chat is not SavedMessagesChat && _chat.GetChatter() is not null)
            {
                MainSettings settings = await SignalRHelperService.GetMainSettings(_chat.GetChatter(), null); 
                    //await ApiService.GetSettingsByUserId(_chat.GetChatter().Id);

                ProfilePhotosSub sub = settings.PrivacySettings.ProfPhotoPrivacy;

                if (sub.ShareType == TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Nobody ||

                  (sub.ShareType == TelegramLib.Enums.Settings.PrivacyAndSecurity.ShareWith.Contacts &&
                   !_system.Contacts.Any(x => x.Id == _chat.GetChatter().Id)))
                {
                    return false;
                }

                if (await ApiService.IsUserIsBlocked(_chat.GetChatter().Id, _system.LoggedUser.Id)) return false;
            }

            return true;
        } 

        private void UserIcon_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserIcon_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = null;
        }

        public void UpdateParams(UserContactcs contact)
        {
            ContName.Text = contact.Name;
            ContSurname.Text = contact.Surname;
        }

        public void ContactRemovedAction()
        {
            ContName.Text = _chat.Chatter.Name;
            ContSurname.Text = _chat.Chatter.Surname;

            //Set hide action params
        }

        private void AddContactGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid) SetBgToGrid(grid);
        }

        private void AddContactGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Grid grid) ClearGridBg(grid);
        }

        private void AddContactGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_chat is TelegramLib.MainClasses.SavedMessagesChat) return;

            //Set Add Contact Page
            EditUserContact contact =
                new EditUserContact(_chat.Chatter, _system);
            ((MainWindow)Window.GetWindow(this)).SetThirdFrame(contact);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MenuGrid.Children.Count > 0)
            {
                MenuGrid.Children.Clear();
                e.Handled = true;
                return;
            }
        }
    }
}
