using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DataReaders.Readers;
using DataReaders.Readers.Interfaces;
using DataReaders.Readers.JSONReaders;
using DataReaders.Utils;

using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeachersScheduleParser.Runtime.Controllers;

public class TelegramBotController : IInitializable
{
    private readonly string _configurationPath = FilePathGetter.GetPath("BotConfiguration.json");
    
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly ITelegramBotClient _botClient;

    private readonly TelegramBotConfigurationData _configurationData;

    private Schedule[] _schedules = null!;
    
    public TelegramBotController()
    {
        IDataReader<TelegramBotConfigurationData> dataReader = new JsonReader<TelegramBotConfigurationData>();

        IFileReader<TelegramBotConfigurationData> fileReader = new FileStreamDataReader<TelegramBotConfigurationData>();
        
        _cancellationTokenSource = new CancellationTokenSource();

        _configurationData = dataReader.ReadData(fileReader, _configurationPath);

        _botClient = new TelegramBotClient(_configurationData.BotAccessToken);
    }
    
    public void Initialize()
    {
        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            cancellationToken: _cancellationTokenSource.Token,
            receiverOptions: new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            });
    }

    public void SetSchedules(Schedule[] schedules)
    {
        _schedules = schedules;
    }

    private async Task InitializeKeyboard(Update update)
    {
        if (_schedules.Length == 0)
        {
            await _botClient.SendTextMessageAsync(update.Message!.Chat.Id, _configurationData.NullDataMessage);
        }
        
        var rows = new List<KeyboardButton[]>();
        var cols = new List<KeyboardButton>();
        
        for (var index = 1; index < _schedules.Length; index++)
        {
            cols.Add(new KeyboardButton(_schedules[index - 1].TeacherData.FullName));
            
            if (index % 3 != 0) continue;
            
            rows.Add(cols.ToArray());
            
            cols = new List<KeyboardButton>();
        }

        var keyboardMarkup = new ReplyKeyboardMarkup(rows.ToArray());

        await _botClient.SendTextMessageAsync(update.Message!.Chat.Id, _configurationData.StartupMessage, 0,
           replyMarkup: keyboardMarkup);
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message) return;
        
        if (message.Text is not { } messageText) return;

        var chatId = message.Chat.Id;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
        
        if (messageText.Contains("start"))
        {
            await InitializeKeyboard(update);
            
            return;
        }

        if (_schedules.Any(x => x.TeacherData.FullName.Contains(messageText)))
        {
            await botClient.SendTextMessageAsync(chatId, _schedules.First
                    (x => x.TeacherData.FullName.Contains(messageText)).ToString(), cancellationToken: cancellationToken);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}