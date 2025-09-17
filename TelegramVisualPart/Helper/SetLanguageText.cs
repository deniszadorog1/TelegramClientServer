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
using TelegramLib.MainClasses;
using TelegramLib.Models;
using TelegramVisualPart.Pages;
using TelegramVisualPart.Pages.ChatActions;
using TelegramVisualPart.Pages.ChatActions.MessageAutoDeletion;
using TelegramVisualPart.Pages.Contacts;
using TelegramVisualPart.Pages.MyProfile;
using TelegramVisualPart.Pages.MyProfile.SetInformation;
using TelegramVisualPart.Pages.Settings.ChatSettings.ChatSetPages;
using TelegramVisualPart.Pages.Settings.NotifsAndSounds;
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

    }
}
