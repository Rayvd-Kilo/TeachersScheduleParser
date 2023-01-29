using System;
using System.Linq;
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

        var storedData = _clientsData.GetData()!.FirstOrDefault(x => x.ChatId.Equals(chatId));

        var subscriptionType = storedData.SubscriptionType;

        var userName = message.From!.Username ?? string.Empty;

        if (storedData.Equals(default))
        {
            subscriptionType = SubscriptionType.Unsubscribed;
        }

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        if (IsCommand(messageText, chatId, userName, subscriptionType)) return Task.CompletedTask;

        if (subscriptionType == SubscriptionType.Unsubscribed)
        {
            _clientsData.SaveData(new[] {new ClientData(chatId, userName, subscriptionType , UpdateType.ScheduleRequired, messageText)});
            
            return Task.CompletedTask;
        }
        
        _clientsData.SaveData(new[] {new ClientData(chatId, userName, subscriptionType , UpdateType.ScheduleRequired, messageText)});
        
        return Task.CompletedTask;
    }

    private bool IsCommand(string message, long chatId, string username, SubscriptionType subscriptionType)
    {
        switch (message)
        {
            case "/start":
                if (!ValidateBannedClient(subscriptionType, chatId, username, message)) return true;
                
                _clientsData.SaveData(new []{new ClientData(chatId, username, SubscriptionType.Subscribed, UpdateType.None, message)});

                return true;
            case "/update":
                if (!ValidateBannedClient(subscriptionType, chatId, username, message)) return true;
                
                _clientsData.SaveData(new []{new ClientData(chatId, username, subscriptionType, UpdateType.DataUpdateRequired, message)});
            
                return true;
            case "/stop":
                if (!ValidateBannedClient(subscriptionType, chatId, username, message)) return true;
                
                _clientsData.SaveData(new []{new ClientData(chatId, username, SubscriptionType.Unsubscribed, UpdateType.None, message)});
                
                return true;
            case "/error":
                if (!ValidateBannedClient(subscriptionType, chatId, username, message)) return true;
                
                _clientsData.SaveData(new []{new ClientData(chatId, username, subscriptionType, UpdateType.ClientEncounteredError, message)});
                
                return true;
        }

        return false;
    }

    private bool ValidateBannedClient(SubscriptionType subscriptionType, long chatId, string username, string message)
    {
        if (subscriptionType == SubscriptionType.Banned)
        {
            _clientsData.SaveData(new []{new ClientData(chatId, username, subscriptionType, UpdateType.None, message)});

            return false;
        }

        return true;
    }
}