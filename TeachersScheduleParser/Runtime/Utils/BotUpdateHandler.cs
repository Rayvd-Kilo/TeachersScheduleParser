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

        var storedData = (_clientsData.GetData() ?? Array.Empty<ClientData>()).FirstOrDefault(x => x.ChatId.Equals(chatId));

        var subscriptionType = storedData.SubscriptionType;

        var targetPersonType = storedData.RequirePersonType;

        var userName = message.From!.Username ?? message.From.FirstName + " " + message.From.LastName;

        if (storedData.Equals(default))
        {
            subscriptionType = SubscriptionType.Unsubscribed;
            
            targetPersonType = PersonType.None;
        }

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        if (IsCommand(messageText, chatId, userName, subscriptionType, targetPersonType)) return Task.CompletedTask;

        if (subscriptionType == SubscriptionType.Unsubscribed)
        {
            _clientsData.SaveData(new[] {new ClientData(chatId, userName, subscriptionType,
                targetPersonType, UpdateType.ScheduleRequired, messageText)});
            
            return Task.CompletedTask;
        }
        
        if (storedData.UpdateType.Equals(UpdateType.ClientStartReport))
        {
            _clientsData.SaveData(new[] {new ClientData(chatId, userName, subscriptionType,
                targetPersonType, UpdateType.ClientReportEnded, messageText)});

            return Task.CompletedTask;
        }

        if (targetPersonType == PersonType.None)
        {
            _clientsData.SaveData(new[] {new ClientData(chatId, userName, subscriptionType,
                targetPersonType, UpdateType.DataUpdateRequired, messageText)});
            
            return Task.CompletedTask;
        }
        
        _clientsData.SaveData(new[] {new ClientData(chatId, userName, subscriptionType,
            targetPersonType, UpdateType.ScheduleRequired, messageText)});
        
        return Task.CompletedTask;
    }

    private bool IsCommand(string message, long chatId, string username, SubscriptionType subscriptionType, PersonType personType)
    {
        switch (message)
        {
            case "/start":
                if (!ValidateBannedClient(subscriptionType, chatId, username, message)) return true;
                
                _clientsData.SaveData(new []{new ClientData(chatId, username,
                    SubscriptionType.Subscribed, PersonType.None, UpdateType.None, message)});

                return true;
            case "/update":
                if (!ValidateBannedClient(subscriptionType, chatId, username, message)) return true;
                
                _clientsData.SaveData(new []{new ClientData(chatId, username,
                    subscriptionType, personType, UpdateType.DataUpdateRequired, message)});
            
                return true;
            case "/stop":
                if (!ValidateBannedClient(subscriptionType, chatId, username, message)) return true;
                
                _clientsData.SaveData(new []{new ClientData(chatId, username,
                    SubscriptionType.Unsubscribed, personType, UpdateType.None, message)});
                
                return true;
            case "/report":
                if (!ValidateBannedClient(subscriptionType, chatId, username, message)) return true;
                
                _clientsData.SaveData(new []{new ClientData(chatId, username,
                    subscriptionType, personType, UpdateType.ClientStartReport, message)});

                return true;
            
            case "/changelisttype":
                if (!ValidateBannedClient(subscriptionType, chatId, username, message)) return true;
                
                _clientsData.SaveData(new []{new ClientData(chatId, username,
                    subscriptionType, PersonType.None, UpdateType.TypeChangeRequired, message)});

                return true;
        }

        return false;
    }

    private bool ValidateBannedClient(SubscriptionType subscriptionType, long chatId, string username, string message)
    {
        if (subscriptionType == SubscriptionType.Banned)
        {
            _clientsData.SaveData(new []{new ClientData(chatId, username,
                subscriptionType, PersonType.None, UpdateType.None, message)});

            return false;
        }

        return true;
    }
}