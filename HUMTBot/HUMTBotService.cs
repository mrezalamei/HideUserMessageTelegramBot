using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace HUMTBot
{
    partial class HUMTBotService : ServiceBase
    {
        private readonly TelegramBotClient _telegramBotClient = new TelegramBotClient("6067006322:AAH1faW3FtkHHaHwzQSt0wdVUBK6TQJ40w0");
        private Thread _startBotThread = null;
        private List<BotCommand> _botCommands;
        public HUMTBotService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _startBotThread = new Thread(() =>
            {
                StartBot().Wait();
            });
            _startBotThread.Start();
        }

        protected override void OnStop()
        {
            _startBotThread = null;
        }

        private async Task StartBot()
        {
            await CheckTelegramToken();
            _botCommands = GetBotCommands();
            _telegramBotClient.StartReceiving(UpdateHandler, PollingErrorHandler, new ReceiverOptions()
            {

            });
        }

        private Task PollingErrorHandler(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            return Task.CompletedTask;
        }

        private List<BotCommand> GetBotCommands()
        {
            var commands = new List<BotCommand>()
            {
                new BotCommand()
                {
                    Command = HUMTBotCommand.RemoveIdsFromCaption,
                    Description = "You are able to remove usernames of a caption The remaining text of the caption would be remained untouched"
                }
            };
            return commands;
        }
        private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            string clearCaption = null;
            if (update.Message.Type == MessageType.Text)
            {
                var messageText = update.Message.Text;
                var messageIsCommand = _botCommands.Any(x => x.Command == messageText);
                if (messageIsCommand)
                {
                    BotCommandHandler(update, ref clearCaption);
                }
            }
            var chatId = update.Message.Chat.Id;
            if (update.Message.ReplyToMessage != null)
            {
                await MessageHandler(update.Message.ReplyToMessage, chatId, clearCaption ?? update.Message.Text);
                return;
            }
            await MessageHandler(update.Message, chatId);
        }

        private void BotCommandHandler(Update update, ref string clearCaption)
        {
            switch (update.Message.Text)
            {
                case var text when text == HUMTBotCommand.RemoveIdsFromCaption:
                    clearCaption = ClearCommandHandler(update);
                    break;
            }
        }

        private string ClearCommandHandler(Update update)
        {
            if (update.Message.ReplyToMessage == null || string.IsNullOrWhiteSpace(update.Message.ReplyToMessage.Caption))
                _telegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Please Reply A message with a caption").Wait();
            var clearCaption = update.Message.ReplyToMessage.Caption;
            clearCaption = Regex.Replace(clearCaption, "(?<![\\w@])@([\\w@]+(?:[.!][\\w@]+)*)", String.Empty);
            clearCaption = Regex.Replace(clearCaption, "(https?:\\/\\/)?(www[.])?(telegram|t)\\.me\\/([a-zA-Z0-9_-]*)\\/?$", String.Empty);
            return clearCaption;
        }

        private async Task MessageHandler(Message message, long chatId, string caption = null)
        {
            switch (message.Type)
            {
                case MessageType.Text:
                    await SendTextMessage(message, chatId);
                    break;
                case MessageType.Photo:
                    await SendPhotoMessage(message, chatId, caption);
                    break;
                case MessageType.Audio:
                    await SendAudioMessage(message, chatId);
                    break;
                case MessageType.Video:
                    await SendVideoMessage(message, chatId, caption);
                    break;
                case MessageType.Voice:
                    await SendVoiceMessage(message, chatId, caption);
                    break;
                case MessageType.Document:
                    await SendDocumentMessage(message, chatId, caption);
                    break;
                case MessageType.Sticker:
                    await SendStickerMessage(message, chatId);
                    break;
                case MessageType.Location:
                    await SendLocationMessage(message, chatId);
                    break;
                case MessageType.Contact:
                    await SendContactMessage(message, chatId);
                    break;
                case MessageType.Venue:
                    await SendVenueMessage(message, chatId);
                    break;
                case MessageType.VideoNote:
                    await SendVideoNoteMessage(message, chatId);
                    break;
                case MessageType.Poll:
                    await SendPollMessage(message, chatId);
                    break;
            }
        }

        private async Task CheckTelegramToken()
        {
            var isValidToken = await _telegramBotClient.TestApiAsync();
            if (!isValidToken)
                this.Stop();
        }

        private async Task SendPollMessage(Message message, long chatId)
        {
            var messagePoll = message.Poll;
            await _telegramBotClient.SendPollAsync(chatId, messagePoll.Question,
                messagePoll.Options.Select(x => x.Text).ToList(), messagePoll.IsAnonymous,
                messagePoll.Type.ToLower() == "quiz".ToLower() ? PollType.Quiz : PollType.Regular,
                messagePoll.AllowsMultipleAnswers, messagePoll.CorrectOptionId, messagePoll.Explanation);
        }

        private async Task SendVideoNoteMessage(Message message, long chatId)
        {
            var messageVideoNote = message.VideoNote;
            await _telegramBotClient.SendVideoNoteAsync(chatId, messageVideoNote.FileId, messageVideoNote.Duration,
                messageVideoNote.Length, messageVideoNote.Thumb.FileId);
        }

        private async Task SendVenueMessage(Message message, long chatId)
        {
            var messageVenue = message.Venue;
            await _telegramBotClient.SendVenueAsync(chatId, messageVenue.Location.Latitude,
                messageVenue.Location.Longitude, messageVenue.Title, messageVenue.Address,
                messageVenue.FoursquareId, messageVenue.FoursquareType, messageVenue.GooglePlaceId,
                messageVenue.GooglePlaceType);
        }

        private async Task SendContactMessage(Message message, long chatId)
        {
            var messageContact = message.Contact;
            await _telegramBotClient.SendContactAsync(chatId, messageContact.PhoneNumber, messageContact.FirstName,
                messageContact.LastName, messageContact.Vcard);
        }

        private async Task SendLocationMessage(Message message, long chatId)
        {
            var messageLocation = message.Location;
            await _telegramBotClient.SendLocationAsync(chatId, messageLocation.Latitude, messageLocation.Longitude,
                messageLocation.LivePeriod, messageLocation.Heading, messageLocation.ProximityAlertRadius);
        }

        private async Task SendStickerMessage(Message message, long chatId)
        {
            await _telegramBotClient.SendStickerAsync(chatId, message.Sticker.FileId);
        }

        private async Task SendDocumentMessage(Message message, long chatId, string caption = null)
        {
            var messageDocument = message.Document;
            await _telegramBotClient.SendDocumentAsync(chatId, messageDocument.FileId,
                thumb: messageDocument.Thumb.FileId, caption ?? message.Caption);
        }

        private async Task SendVoiceMessage(Message message, long chatId, string caption = null)
        {
            var messageVoice = message.Voice;
            await _telegramBotClient.SendVoiceAsync(chatId, messageVoice.FileId, caption ?? message.Caption);
        }

        private async Task SendVideoMessage(Message message, long chatId, string caption = null)
        {
            var messageVideo = message.Video;
            await _telegramBotClient.SendVideoAsync(chatId, messageVideo.FileId, duration: messageVideo.Duration,
                width: messageVideo.Width, height: messageVideo.Height, thumb: messageVideo.Thumb.FileId,
                caption ?? message.Caption);
        }

        private async Task SendAudioMessage(Message message, long chatId)
        {
            var messageAudio = message.Audio;
            await _telegramBotClient.SendAudioAsync(chatId, messageAudio.FileId, message.Caption,
                duration: messageAudio.Duration, performer: messageAudio.Performer, title: messageAudio.Title);
        }

        private async Task SendTextMessage(Message message, long chatId)
        {
            await _telegramBotClient.SendTextMessageAsync(chatId, message.Text);
        }

        private async Task SendPhotoMessage(Message message, long chatId, string caption = null)
        {
            var photo = message.Photo.First();
            await _telegramBotClient.SendPhotoAsync(chatId, photo.FileId, caption ?? message.Caption);
        }
    }
}
