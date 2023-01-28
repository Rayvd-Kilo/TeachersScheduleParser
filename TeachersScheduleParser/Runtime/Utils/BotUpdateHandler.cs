using System;
using System.Threading;
using System.Threading.Tasks;

using TeachersScheduleParser.Runtime.Enums;
using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace TeachersScheduleParser.Runtime.Utils;

public class BotUpdateHandler : IAsyncResultHandler<Update>
{
    private readonly IDataContainerModel<ClientData[]> _clientsData;

    public BotUpdateHandler(IDataContainerModel<ClientData[]> clientsData)
    {
        _clientsData = clientsData;
    }

    Task IAsyncResultHandler<Update>.HandleResultAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message) return Task.CompletedTask;
        
        if (message.Text is not { } messageText) return Task.CompletedTask;

        var chatId = message.Chat.Id;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        if (IsCommand(messageText, chatId)) return Task.CompletedTask;
        
        _clientsData.SaveData(new[] {new ClientData(chatId, UpdateType.ScheduleRequired, messageText)});
        
        return Task.CompletedTask;
    }

    private bool IsCommand(string message, long chatId)
    {
        switch (message)
        {
            case "/start":
                _clientsData.SaveData(new []{new ClientData(chatId, UpdateType.ClientSubscribe, message)});

                return true;
            case "/update":
                _clientsData.SaveData(new []{new ClientData(chatId, UpdateType.DataUpdateRequired, message)});
            
                return true;
            case "/stop":
                _clientsData.SaveData(new []{new ClientData(chatId, UpdateType.ClientUnsubscribe, message)});
                
                return true;
        }

        if (message.Contains("/error"))
        {
            _clientsData.SaveData(new []{new ClientData(chatId, UpdateType.ClientEncounteredError, message)});
                
            return true;
        }

        return false;
    }
}