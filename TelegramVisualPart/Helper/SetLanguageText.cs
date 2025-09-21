using MaterialDesignThemes.Wpf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Xps.Packaging;
using TelegramLib.MainClasses;
using TelegramLib.Models;
using TelegramLib.UserSettings.SettingsTypes.SubSettings.PrivAnSecSubs;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Pages.Advanced;
using TelegramVisualPart.Pages.ChatActions;
using TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion;
using TelegramVisualPart.Pages.Contacts;
using TelegramVisualPart.Pages.MyProfile;
using TelegramVisualPart.Pages.MyProfile.SetInformation;
using TelegramVisualPart.Pages.Settings.ChatSettings;
using TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages;
using TelegramVisualPart.Pages.Settings.Folders;
using TelegramVisualPart.Pages.Settings.Language;
using TelegramVisualPart.Pages.Settings.NotifsAndSounds;
using TelegramVisualPart.Pages.Settings.PrivAndSecurity;
using TelegramVisualPart.Pages.Settings.PrivAndSecurity.ButsPages;
using TelegramVisualPart.Services;
using TelegramVisualPart.UserControls.ChatControls;

namespace TelegramVisualPart.Helper
{
    public static class SetLanguageText
    {
        //Set Main Page
        public static void SetMainChatPageParams(MainChatPage page)
        {
            HintAssist.SetHint(page.SarchBox, VisConstParamsJsonService.GetStringByName("SearcBoxHint"));

            page.SetChatBlock.Text = VisConstParamsJsonService.GetStringByName("SetChatBlock");
            page.LeftButtons.AllChats.ButText.Text = VisConstParamsJsonService.GetStringByName("AllChatsBut");
            page.LeftButtons.Personal.ButText.Text = VisConstParamsJsonService.GetStringByName("PersonalBut");
            page.LeftButtons.Edit.ButText.Text = VisConstParamsJsonService.GetStringByName("EditBut");

            page.SearchControl.ChatTab.Text = VisConstParamsJsonService.GetStringByName("ChatsTab");
            page.SearchControl.PhotosTab.Text = VisConstParamsJsonService.GetStringByName("PhotosTab");
            page.SearchControl.VideosTab.Text = VisConstParamsJsonService.GetStringByName("VideosTab");
            page.SearchControl.DownloadsTab.Text = VisConstParamsJsonService.GetStringByName("DownloadsTab");
            page.SearchControl.LinksTab.Text = VisConstParamsJsonService.GetStringByName("LinksTab");
            page.SearchControl.FrqContactsBlockName.Text = VisConstParamsJsonService.GetStringByName("FrqContactsBlockName");
            page.SearchControl.ShowFreqChats.Text = VisConstParamsJsonService.GetStringByName("ShowFreqChats");

            page.MyProfileDrawBut.ButName.Text = VisConstParamsJsonService.GetStringByName("MyProfileDrawBut");
            page.WalletDrawBut.ButName.Text = VisConstParamsJsonService.GetStringByName("WalletDrawBut");
            page.NewGroupDrawBut.ButName.Text = VisConstParamsJsonService.GetStringByName("NewGroupDrawBut");
            page.NewChannelDrawBut.ButName.Text = VisConstParamsJsonService.GetStringByName("NewChannelDrawBut");
            page.ContactsDrawBut.ButName.Text = VisConstParamsJsonService.GetStringByName("ContactsDrawBut");
            page.CallsDrawBut.ButName.Text = VisConstParamsJsonService.GetStringByName("CallsDrawBut");
            page.SettingsDrawBut.ButName.Text = VisConstParamsJsonService.GetStringByName("SettingsDrawBut");
        }

        public static void SetUserChat(TelegramVisualPart.UserControls.UserChat chat)
        {
            HintAssist.SetHint(chat.CommentTextBox, VisConstParamsJsonService.GetStringByName("SendTextHint"));

            chat.EmojisBoard.EmojiTab.Text = VisConstParamsJsonService.GetStringByName("EmBoardEmoji");
            chat.EmojisBoard.StickersTab.Text = VisConstParamsJsonService.GetStringByName("EmBoardSticker");
            chat.EmojisBoard.GIFsTab.Text = VisConstParamsJsonService.GetStringByName("EmBoardGIFs");

            HintAssist.SetHint(chat.EmojisBoard.EmojisPanel.SarchBox,
                VisConstParamsJsonService.GetStringByName("SearcBoxHint"));

            chat.UserChatMenu.ViewProfileBut.ButName.Text = VisConstParamsJsonService.GetStringByName("ViewProfBut");
            chat.UserChatMenu.SetWallpaperBut.ButName.Text = VisConstParamsJsonService.GetStringByName("SetWallpaperBut");
            chat.UserChatMenu.ExportHistoryBut.ButName.Text = VisConstParamsJsonService.GetStringByName("ExportHistoryBut");
            chat.UserChatMenu.ClearChatBut.ButName.Text = VisConstParamsJsonService.GetStringByName("ClearChatBut");
            chat.UserChatMenu.DeleteChatBut.ButName.Text = VisConstParamsJsonService.GetStringByName("DeleteChatBut");
        }

        public static void SetContactInfo(ContactInfo info)
        {
            info.PageNameBlock.Text = VisConstParamsJsonService.GetStringByName("ContInfoPageName");

            info.MobileNumber.BottomText.Text = VisConstParamsJsonService.GetStringByName("MobileNumber");
            info.UserName.BottomText.Text = VisConstParamsJsonService.GetStringByName("UserName");
            info.Birthdate.BottomText.Text = VisConstParamsJsonService.GetStringByName("Birthdate");


            info.NotifsBlock.Text = VisConstParamsJsonService.GetStringByName("NotifsBlock");
            info.SendMesBlock.Text = VisConstParamsJsonService.GetStringByName("SendMessage");

            info.AmountOfPhotosTextBlock.Text = VisConstParamsJsonService.GetStringByName("AmountOfPhotosTextBlock");
            info.AmountOfVideosTextBlock.Text = VisConstParamsJsonService.GetStringByName("AmountOfVideosTextBlock");
            info.AmountOfGifsTextBlock.Text = VisConstParamsJsonService.GetStringByName("AmountOfGifsTextBlock");

            info.ShareContactBlock.Text = VisConstParamsJsonService.GetStringByName("ShareContactBlock");
            info.EditContactBlock.Text = VisConstParamsJsonService.GetStringByName("EditContactBlock");
            info.DeleteContactBlock.Text = VisConstParamsJsonService.GetStringByName("DeleteContactBlock");
            info.BlockContactBlock.Text = VisConstParamsJsonService.GetStringByName("BlockContactBlock");

            info.ContactMenu.AutoDelete.ButName.Text = VisConstParamsJsonService.GetStringByName("AutoDelete");
            info.ContactMenu.ShareContact.ButName.Text = VisConstParamsJsonService.GetStringByName("ShareContact");
            info.ContactMenu.EditContact.ButName.Text = VisConstParamsJsonService.GetStringByName("EditContact");
            info.ContactMenu.ExportHistory.ButName.Text = VisConstParamsJsonService.GetStringByName("ExportHistory");
            info.ContactMenu.AddToFolder.ButName.Text = VisConstParamsJsonService.GetStringByName("AddToFolder");
            info.ContactMenu.BlockUser.ButName.Text = VisConstParamsJsonService.GetStringByName("BlockUser");
            info.ContactMenu.DeleteContact.ButName.Text = VisConstParamsJsonService.GetStringByName("DeleteContact");
        }

        public static void SetMessDeletion(NewMessagesDeletion page)
        {
            page.PageNameBlock.Text = VisConstParamsJsonService.GetStringByName("MesDeletionPageName");

            page.InfoText.Text = VisConstParamsJsonService.GetStringByName("MesDelPgInfoText");
            page.InActiveFirstPart.Text = VisConstParamsJsonService.GetStringByName("MesDelPgInActiveFirstPart");
            page.SetDestructBut.Text = VisConstParamsJsonService.GetStringByName("MesDelPgSetDestructBut");
            page.InActiveEndPart.Text = VisConstParamsJsonService.GetStringByName("MesDelPgInActiveEndPart");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.SaveBut.Content = VisConstParamsJsonService.GetStringByName("SaveButName");
        }

        public static void SetSelfDestTimer(SelfDestructTimer page)
        {
            page.PageNameBlock.Text = VisConstParamsJsonService.GetStringByName("SelfDestPageName");

            page.OffRadioBut.ButName.Text = VisConstParamsJsonService.GetStringByName("SelfDestOffRadioBut");
            page.OneDayBut.ButName.Text = VisConstParamsJsonService.GetStringByName("SelfDestOneDayBut");
            page.OneWeekBut.ButName.Text = VisConstParamsJsonService.GetStringByName("SelfDestOneWeekBut");
            page.OneMonthBut.ButName.Text = VisConstParamsJsonService.GetStringByName("SelfDestOneMonthBut");
            page.CustomTimeBut.ButName.Text = VisConstParamsJsonService.GetStringByName("SelfDestCustomTimeBut");

            page.InActiveInfoText.Text = VisConstParamsJsonService.GetStringByName("SelfDestInfoInActiveText");
            page.SetAutoDeleteToContact.Text = VisConstParamsJsonService.GetStringByName("SelfDestActiveText");
        }

        public static void SetCustomTimer(SetCustomTime page)
        {
            page.PageNameBlock.Text = VisConstParamsJsonService.GetStringByName("SetCustTimerPageNameBlock");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.SaveBut.Content = VisConstParamsJsonService.GetStringByName("SaveButName");
        }

        public static void SetToChooseChat(ToChooseChats page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("PageNameToChooseChat");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.ApplyBut.Content = VisConstParamsJsonService.GetStringByName("ApplyButName");
        }

        public static void SetChatWallpaper(SetChatWallpaper page)
        {
            page.PageNameBlock.Text = VisConstParamsJsonService.GetStringByName("SetChatWallPageName");

            page.CloseBut.TextBlock.Text = VisConstParamsJsonService.GetStringByName("CloseButName");
            page.ChooseFileBlock.Text = VisConstParamsJsonService.GetStringByName("SetChatWallFileBlock");
        }

        public static void SetWallpaperPreview(WallpaperPreview page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("WallperPreviewPageName");

            page.TestDate.Text = VisConstParamsJsonService.GetStringByName("TestPrevDate");

            page.FirstMessageTextBlock.Text = VisConstParamsJsonService.GetStringByName("FirstTestMessageTextBlock");
            page.SecondMessageTextBlock.Text = VisConstParamsJsonService.GetStringByName("SecondTestMessageTextBlock");

            page.IsBlurCheckBox.Content = VisConstParamsJsonService.GetStringByName("IsBlurCheckBox");

            page.Cancel.TextBlock.Text = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.Apply.TextBlock.Text = VisConstParamsJsonService.GetStringByName("ApplyButName");
        }

        public static void ClearChatHistory(ClearChatHistory page)
        {
            page.FirstInfoText.Text = VisConstParamsJsonService.GetStringByName("FirstInfoTextClChatHist");
            page.SecondInfoText.Text = VisConstParamsJsonService.GetStringByName("SecondInfoTextClChatHist");
            page.CheckBoxText.Text = VisConstParamsJsonService.GetStringByName("CheckBoxTextClChatHist");
            page.EnAutoDelete.Text = VisConstParamsJsonService.GetStringByName("EnAutoDeleteClChatHist");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.DeleteBut.Content = VisConstParamsJsonService.GetStringByName("DeleteButName");
        }

        public static void SetDeleteChat(DeleteChat page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("DeleteChatPageName");
            page.FirstInfoText.Text = VisConstParamsJsonService.GetStringByName("DeleteChatFirstInfoText");
            page.QuestText.Text = VisConstParamsJsonService.GetStringByName("QuestText");

            page.SecInfoText.Text = VisConstParamsJsonService.GetStringByName("DeleteChatSecInfoText");

            page.CheckBoxText.Text = VisConstParamsJsonService.GetStringByName("DeleteChatCheckBoxText");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.DeleteBut.Content = VisConstParamsJsonService.GetStringByName("DeleteButName");
        }

        public static void SetLoggedUserProfile(LoggedUserProfile page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("LogUseProfPageName");
            page.phNumberText.Text = VisConstParamsJsonService.GetStringByName("LogUseProfMobile");
            page.LoginText.Text = VisConstParamsJsonService.GetStringByName("LogUseProfUserName");
        }

        public static void SetMyProfileSettings(MyProfileSettings page)
        {
            HintAssist.SetHint(page.BioTextBox,
                VisConstParamsJsonService.GetStringByName("Bio"));

            page.PageName.Text = VisConstParamsJsonService.GetStringByName("PageNameProfSet");
            page.InfoText.Text = VisConstParamsJsonService.GetStringByName("ProfSetInfoText");

            page.Name.ButName.Text = VisConstParamsJsonService.GetStringByName("ProfSetName");
            //page.Name.AdditionalText.Text = VisConstParamsJsonService.GetStringByName("ProfSetAdditName");

            page.PhoneNumber.ButName.Text = VisConstParamsJsonService.GetStringByName("ProfSetPhoneNumber");
            //page.PhoneNumber.AdditionalText.Text = VisConstParamsJsonService.GetStringByName("ProfSetAdditPhoneNumber");

            page.Username.ButName.Text = VisConstParamsJsonService.GetStringByName("ProfSetUsername");
            //page.Username.AdditionalText.Text = VisConstParamsJsonService.GetStringByName("ProfSetAdditUsername");

            page.SecondInfoText.Text = VisConstParamsJsonService.GetStringByName("ProfSetSecInfoText");

            page.PersonalChannelBut.ButName.Text = VisConstParamsJsonService.GetStringByName("ProfSetPersonalChannel");
            //page.PersonalChannelBut.AdditionalText.Text = VisConstParamsJsonService.GetStringByName("ProfSetAdditPersonalChannel");

            page.BirthdayBut.ButName.Text = VisConstParamsJsonService.GetStringByName("ProfSetBirthdayBut");
            //page.BirthdayBut.AdditionalText.Text = VisConstParamsJsonService.GetStringByName("ProfSetAdditBirthdayBut");
        }

        public static void SetNameSurname(SetNameSurname page)
        {
            HintAssist.SetHint(page.FirstNameBox,
                VisConstParamsJsonService.GetStringByName("NameSuranmeFirstNmHint"));

            HintAssist.SetHint(page.LastNameBox,
                VisConstParamsJsonService.GetStringByName("NameSuranmeFirstLastNmHint"));

            page.PageName.Text = VisConstParamsJsonService.GetStringByName("NameSuranmePageName");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.SaveBut.Content = VisConstParamsJsonService.GetStringByName("SaveButName");

        }

        public static void SetPhoneNumber(SetPhoneNumber page)
        {
            page.TextInfo.Text = VisConstParamsJsonService.GetStringByName("PhoneNumberText");
            page.Ok.Content = VisConstParamsJsonService.GetStringByName("OkButName");
        }

        public static void SetUsername(SetUsername page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("SetUsernamePageName");

            page.FirstTextInfo.Text = VisConstParamsJsonService.GetStringByName("SetUsernameFirstTextInfo");
            page.SecTextInfo.Text = VisConstParamsJsonService.GetStringByName("SetUsernameSecTextInfo");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.SaveBut.Content = VisConstParamsJsonService.GetStringByName("SaveButName");
        }

        public static void SetBirthDate(SetBirthDate page)
        {
            page.RemoveBut.Content = VisConstParamsJsonService.GetStringByName("RemoveButName");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.SaveBut.Content = VisConstParamsJsonService.GetStringByName("SaveButName");
        }

        public static void SetUserContacts(MainContacts page)
        {
            page.ContactsBlock.Text = VisConstParamsJsonService.GetStringByName("UsContsPageName");

            page.AddContactBut.Content = VisConstParamsJsonService.GetStringByName("UsAddContactBut");
            page.CloseBut.Content = VisConstParamsJsonService.GetStringByName("CloseButName");
        }

        public static void SetAddContact(AddContact page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("AddContactPageName");

            HintAssist.SetHint(page.NameBox,
                VisConstParamsJsonService.GetStringByName("AddContFirstNameHint"));

            HintAssist.SetHint(page.LastnameBox,
                VisConstParamsJsonService.GetStringByName("AddContLastNameHint"));

            HintAssist.SetHint(page.PhoneBox,
                VisConstParamsJsonService.GetStringByName("AddContPhoneNumHint"));

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.CreateBut.Content = VisConstParamsJsonService.GetStringByName("CreateButName");
        }

        public static void SetNotsAndSounds(NotAndSoundSettings page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("NotAndSoundPageName");

            page.GlobSubMenu.Text = VisConstParamsJsonService.GetStringByName("NotAndSoundGlobSubMenu");
            page.NotForChatbMenu.Text = VisConstParamsJsonService.GetStringByName("NotForChatbMenuSubMenu");
            page.EventsMenu.Text = VisConstParamsJsonService.GetStringByName("EventsMenuSubMenu");
            page.ScreenLocMenu.Text = VisConstParamsJsonService.GetStringByName("ScreenLocMenuSubMenu");
            page.MessAmountMenu.Text = VisConstParamsJsonService.GetStringByName("MessAmountMenuSubMenu");


            page.DeskTopNotifs.TextBlock.Text = VisConstParamsJsonService.GetStringByName("NotSoundDesktopNotif");
            page.FlashBarIcon.TextBlock.Text = VisConstParamsJsonService.GetStringByName("NotSoundFlashBarIconFlashBarIcon");
            page.AllowSound.TextBlock.Text = VisConstParamsJsonService.GetStringByName("NotSoundAllowSound");
            page.PrivateChat.TextBlock.Text = VisConstParamsJsonService.GetStringByName("NotSoundPrivateChat");
            page.PinnedMessages.TextBlock.Text = VisConstParamsJsonService.GetStringByName("NotSoundPinnedMessages");
        }

        public static void SetChatSetPage(MainChatSetPage page)
        {
            page.ChatNameBlock.Text = VisConstParamsJsonService.GetStringByName("ChatSetPageName");

            page.ThemesTextBlock.Text = VisConstParamsJsonService.GetStringByName("ChatSetThemesTextBlock");

            page.Classic.CardName.Text = VisConstParamsJsonService.GetStringByName("ChatSetThemesClassicCard");
            page.Day.CardName.Text = VisConstParamsJsonService.GetStringByName("ChatSetThemesDayCard");
            page.Tinted.CardName.Text = VisConstParamsJsonService.GetStringByName("ChatSetThemesTintedCard");
            page.Night.CardName.Text = VisConstParamsJsonService.GetStringByName("ChatSetThemesNightCard");

            page.ThemeSettingsBlock.Text = VisConstParamsJsonService.GetStringByName("ChatSetThemeSettingsBlock");

            page.AutoNightBut.ButName.Text = VisConstParamsJsonService.GetStringByName("ChatSetAutoNightBut");
            page.FontFamalyBut.ButName.Text = VisConstParamsJsonService.GetStringByName("ChatSetFontFamalyBut");

            page.ChatWalpsBlock.Text = VisConstParamsJsonService.GetStringByName("ChatWalpsBlock");

            page.ChooseWallpaperFromGalery.Text = VisConstParamsJsonService.GetStringByName("ChatSetChooseWallpaperFromGalery");
            page.ChooseWallpaperFromFile.Text = VisConstParamsJsonService.GetStringByName("ChatSetChooseWallpaperFromFile");

            page.SendEnterRadio.Content = VisConstParamsJsonService.GetStringByName("ChatSetSendEnterRadio");
            page.SendCtrlEnterRadio.Content = VisConstParamsJsonService.GetStringByName("ChatSetSendCtrlEnterRadio");
        }

        public static void SetChatSetPalette(ChatSetPalette page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("ChatSetPalettePageName");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.SaveBut.Content = VisConstParamsJsonService.GetStringByName("SaveButName");
        }

        public static void SetFontFamily(ChooseFontFamily page)
        {
            HintAssist.SetHint(page.SearchBox,
                VisConstParamsJsonService.GetStringByName("AddContFirstNameHint"));

            page.PageNameBlock.Text = VisConstParamsJsonService.GetStringByName("FontFamPageNameBlock");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.SaveBut.Content = VisConstParamsJsonService.GetStringByName("SaveButName");
        }

        public static void SetAdvancedPage(AdvancedPage page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("AdvPagePageName");

            page.DataStorageText.Text = VisConstParamsJsonService.GetStringByName("AdvPageDataStorageBlock");
            page.MediaDownloadText.Text = VisConstParamsJsonService.GetStringByName("AdvPageMediaDownloadBlock");
            page.TitleBarText.Text = VisConstParamsJsonService.GetStringByName("AdvPageTitleBarBlock");
            page.SysIntegration.Text = VisConstParamsJsonService.GetStringByName("AdvPageSysIntegrationBlock");
            page.VersionsText.Text = VisConstParamsJsonService.GetStringByName("AdvPageVersionsTextBlock");

            page.DownloadPathBut.ButName.Text = VisConstParamsJsonService.GetStringByName("AdvPageDownloadPathButButName");
            page.DownloadPathBut.TempStatusBut.Text = VisConstParamsJsonService.GetStringByName("AdvPageDownloadPathButTempStatusBut");

            page.Downloads.ButName.Text = VisConstParamsJsonService.GetStringByName("AdvPageDownloadsButName");

            page.IsAskDownloadPath.TextBlock.Text = VisConstParamsJsonService.GetStringByName("AdvPageIsAskDownloadPathTextBlock");

            page.PrivateChatsBut.ButName.Text = VisConstParamsJsonService.GetStringByName("AdvPagePrivateChatsButButName");

            page.PrivateChatsBut.ButName.Text = VisConstParamsJsonService.GetStringByName("AdvPagePrivateChatsButButName");

            page.ShowChatNameBox.Content = VisConstParamsJsonService.GetStringByName("AdvPageShowChatNameBox");
            page.UnreadCountBox.Content = VisConstParamsJsonService.GetStringByName("AdvPageUnreadCountBox");
            page.WindowFrame.Content = VisConstParamsJsonService.GetStringByName("AdvPageWindowFrame");

            page.TrayIconBox.Content = VisConstParamsJsonService.GetStringByName("AdvPageTrayIconBox");
            page.TaskBarBox.Content = VisConstParamsJsonService.GetStringByName("AdvPageTaskBarBox");
            page.CloseToTaskBarBox.Content = VisConstParamsJsonService.GetStringByName("AdvPageCloseToTaskBarBox");
            page.AtStartLaunchTelegramBox.Content = VisConstParamsJsonService.GetStringByName("AdvPageAtStartLaunchTelegramBox");
            page.PalceInSendTo.Content = VisConstParamsJsonService.GetStringByName("AdvPagePalceInSendTo");


            page.VersionBut.FirstTextBlock.Text = VisConstParamsJsonService.GetStringByName("AdvPageVersionButFirstTextBlock");
            page.VersionBut.SecondTextBlock.Text = VisConstParamsJsonService.GetStringByName("AdvPageSecondTextBlock");

            page.InstalBetaBut.TextBlock.Text = VisConstParamsJsonService.GetStringByName("AdvPageInstalBetaButTextBlock");
            page.CheckForUpdates.TextBlock.Text = VisConstParamsJsonService.GetStringByName("AdvPageCheckForUpdatesTextBlock");

        }

        public static void SetFoldersPage(FoldersPage page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("FoldersPageName");

            page.InfoText.Text = VisConstParamsJsonService.GetStringByName("FoldersPageInfoText");

            page.FoldersBlock.Text = VisConstParamsJsonService.GetStringByName("FoldersPageFoldersBlock");

            page.TestThing.FolderName.Text = VisConstParamsJsonService.GetStringByName("FoldersPageTestThingFolderName");
            page.TestThing.AmountOfChats.Text = VisConstParamsJsonService.GetStringByName("FoldersPageTestThingAmountOfChats");

            page.CreateNewFolderBut.NewFolderText.Text = VisConstParamsJsonService.GetStringByName("FoldersPageCreateNewFolderBut");

            page.FolderBlock.Text = VisConstParamsJsonService.GetStringByName("FoldersPageFolderBlock");

            page.LeftTabs.Content = VisConstParamsJsonService.GetStringByName("FoldersPageLeftTabs");
            page.ShitTabs.Content = VisConstParamsJsonService.GetStringByName("FoldersPageShitTabs");

            page.Kchau.Text = VisConstParamsJsonService.GetStringByName("FoldersPageKchau");
        }

        public static void SetFolderAction(FolderAction page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("FolActionPageName");

            HintAssist.SetHint(page.FolderNameBox,
                 VisConstParamsJsonService.GetStringByName("FolActionFolderNameBoxHint"));

            page.WhoCanUseBlock.Text = VisConstParamsJsonService.GetStringByName("FolActionWhoCanUseBlock");

            page.CreateNewFolderBut.NewFolderText.Text = VisConstParamsJsonService.GetStringByName("FolActionCreateNewFolderBut");

            page.InfoText.Text = VisConstParamsJsonService.GetStringByName("FolActionInfoText");
            page.ExcludedChatsBlock.Text = VisConstParamsJsonService.GetStringByName("FolActionExcludedChatsBlock");

            page.ChatToExcludeBut.NewFolderText.Text = VisConstParamsJsonService.GetStringByName("FolActionChatToExcludeBut");

            page.SecondInfoText.Text = VisConstParamsJsonService.GetStringByName("FolActionSecondInfoText");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.CreateBut.Content = VisConstParamsJsonService.GetStringByName("CreateButName");
        }

        public static void SetFolderChatAction(FoldersChatAction page)
        {
            HintAssist.SetHint(page.ChatSearchBox,
                 VisConstParamsJsonService.GetStringByName("FolChatActChatSearchBoxHint"));

            page.ContactsChats.TypeName.Text = VisConstParamsJsonService.GetStringByName("FolChatActContactsChats");
            page.NoneContactsChats.TypeName.Text = VisConstParamsJsonService.GetStringByName("FolChatActNoneContactsChats");
            page.GroupsChats.TypeName.Text = VisConstParamsJsonService.GetStringByName("FolChatActGroupsChats");
            page.ChannelsChats.TypeName.Text = VisConstParamsJsonService.GetStringByName("FolChatActChannelsChats");
            page.BotsChats.TypeName.Text = VisConstParamsJsonService.GetStringByName("FolChatActBotsChats");

            page.ChatsTextBlock.Text = VisConstParamsJsonService.GetStringByName("FolChatChatsTextBlock");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.SaveBut.Content = VisConstParamsJsonService.GetStringByName("SaveButName");

        }

        public static void SetLanguagePage(LanguagePage page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("LangPagePageName");

            HintAssist.SetHint(page.SarchBox,
                 VisConstParamsJsonService.GetStringByName("LangPageSarchBoxHint"));

            page.OkBut.Content = VisConstParamsJsonService.GetStringByName("OkButName");
        }

        public static void SetPrivAndSecurity(PrivacyAndSecurity page)
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecPageName");

            page.SecBlock.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecSecBlock");

            page.LocalPasscode.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecLocalPasscodeNamePart");
            page.LocalPasscode.EnumPart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecLocalPasscodeEnumPart");

            page.BlockedUsers.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecBlockedUsersNamePart");
            page.BlockedUsers.EnumPart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecBlockedUsersEnumPart");

            page.PrivBlock.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecPrivBlock");
            
            page.PhoneNumber.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecPhoneNumberNamePart");
            page.PhoneNumber.EnumPart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecPhoneNumberEnumPart");

            page.LastSeen.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecLastSeenNamePart");
            page.LastSeen.EnumPart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecLastSeenEnumPart");

            page.ProfilePhotos.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecProfilePhotosNamePart");
            page.ProfilePhotos.EnumPart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecProfilePhotosEnumPart");

            page.ForwardedMessages.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecForwardedMessagesNamePart");
            page.ForwardedMessages.EnumPart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecForwardedMessagesEnumPart");

            page.Messages.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecMessagesNamePart");
            page.Messages.EnumPart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecMessagesEnumPart");

            page.DateOfBirth.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecDateOfBirthNamePart");
            page.DateOfBirth.EnumPart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecDateOfBirthEnumPart");

            page.BioBut.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecBioButNamePart");
            page.BioBut.EnumPart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecBioButEnumPart");

            page.BotsWebBlock.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecBotsWebBlock");

            page.ClearPayments.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecClearPaymentsNamePart");

            page.DeleteAway.NamePart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecDeleteAwayNamePart");
            page.DeleteAway.EnumPart.Text = VisConstParamsJsonService.GetStringByName("PrivAndSecDeleteAwayEnumPart");
        }

        public static void SetAccountDeletion(PrivacyDeleteAccount page) 
        {
            page.PageName.Text = VisConstParamsJsonService.GetStringByName("DeleteAccPageName");
            page.InfoText.Text = VisConstParamsJsonService.GetStringByName("DeleteAccInfoText");

            page.OneMonthRadio.Content = VisConstParamsJsonService.GetStringByName("DeleteOneMonthRadio");
            page.ThreeMonthRadio.Content = VisConstParamsJsonService.GetStringByName("DeleteThreeMonthRadio");
            page.SixRadio.Content = VisConstParamsJsonService.GetStringByName("DeleteSixMonthRadio");
            page.TwelveMonthRadio.Content = VisConstParamsJsonService.GetStringByName("DeleteTwelveMonthRadio");
            page.EighteenMonthRadio.Content = VisConstParamsJsonService.GetStringByName("DeleteEighteenMonthRadio");
            page.TwentyfourMonthRadio.Content = VisConstParamsJsonService.GetStringByName("DeleteTwentyfourMonthRadio");

            page.CancelBut.Content = VisConstParamsJsonService.GetStringByName("CancelButName");
            page.SaveBut.Content = VisConstParamsJsonService.GetStringByName("SaveButName");
        }

    }
}
