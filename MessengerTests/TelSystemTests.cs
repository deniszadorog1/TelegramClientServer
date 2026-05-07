
using TelegramLib.Enums.Chat;
using TelegramLib.Interfaces;
using TelegramLib.MainClasses;

namespace MessengerTests
{
    public class TelSystemTests
    {
        [Fact]
        public void IsFolderNameExist()
        {
            TelSystem system = new TelSystem();
            system.AddFolder(1, "Work", "icon", new List<User>(), new List<User>());

            bool existsUpper = system.IsFolderNameExists("Work");
            bool existsLower = system.IsFolderNameExists("work");

            Assert.True(existsUpper);
            Assert.False(existsLower);
        }

        [Fact]
        public void GetChatByMessageId()
        {
            TelSystem system = new TelSystem();
            SavedMessagesChat chat = new SavedMessagesChat() { Id = 999 };

            system.SavedMesesChat = chat;

            system.SavedMesesChat.Messages = new List<TelegramLib.MainClasses.Messages.Message>() { new TelegramLib.MainClasses.Messages.Message() { Id = 77 } };

            var result = system.GetChatByMessageId(77);

            Assert.NotNull(result);
            Assert.Equal(999, result.Id);
        }

        [Theory]
        [InlineData(AutoDeleteType.OneDay, "1d")]
        [InlineData(AutoDeleteType.OneWeek, "1w")]
        [InlineData(AutoDeleteType.OneYear, "1y")]
        [InlineData((AutoDeleteType)999, "ct")] 
        public void GetAutDelDurationInString_ReturnsCorrectShortCodes(AutoDeleteType type, string expected)
        {
            var system = new TelSystem();

            var result = system.GetAutDelDurationInString(type);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void AddFolder_NullName_ThrowsArgumentException()
        {
            var system = new TelSystem();

            Assert.Throws<ArgumentException>(() =>
                system.AddFolder(1, null, "icon", new List<User>(), new List<User>()));
        }

        [Fact]
        public void GetChatByMessageId_NonExistentId_ReturnsNull()
        {
            var system = new TelSystem();
            system.SavedMesesChat = new SavedMessagesChat { Messages = new List<TelegramLib.MainClasses.Messages.Message>() };

            var result = system.GetChatByMessageId(999999); 

            Assert.Null(result);
        }

    }
}