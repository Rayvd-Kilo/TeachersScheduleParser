using System;
using System.Threading;
using System.Threading.Tasks;

using TeachersScheduleParser.Runtime.Enums;
using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Models;
using TeachersScheduleParser.Runtime.Structs;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeachersScheduleParser.Runtime.Utils;

public class BotUpdateHandler : IAsyncResultHandler<Update>
{
    private readonly ClientDataModel _clientsData;

    public BotUpdateHandler(ClientDataModel clientsData)
    {
        _clientsData = clientsData;
    }
    
    public Task HandleResultAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message) return Task.CompletedTask;
        
        if (message.Text is not { } messageText) return Task.CompletedTask;

        var chatId = message.Chat.Id;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
        
        if (messageText.Contains("start"))
        {
            _clientsData.SaveData(new ClientData(chatId, UpdateType.ClientSubscribe));

            return Task.CompletedTask;
        }

        _clientsData.SaveData(new ClientData(chatId, UpdateType.ScheduleRequired));
        
        return Task.CompletedTask;
    }
}