using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DataReaders.Readers;
using DataReaders.Readers.Interfaces;
using DataReaders.Readers.JSONReaders;
using DataReaders.Utils;

using TeachersScheduleParser.Runtime.Enums;
using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

using UpdateType = Telegram.Bot.Types.Enums.UpdateType;

namespace TeachersScheduleParser.Runtime.Controllers;

public class TelegramBotController : IInitializable, IDisposable
{
    private readonly string _configurationPath = FilePathGetter.GetPath("BotConfiguration.json");
    
    private readonly IDataContainerModel<Schedule[]> _schedulesContainerModel;
    
    private readonly IDataContainerModel<ClientData[]> _clientsDataContainerModel;
    
    private readonly IAsyncReactiveValue<ClientData> _reactiveClientData;

    private readonly IReactiveValue<Schedule[]> _reactiveValue;
    
    private readonly IAsyncResultHandler<Exception> _errorHandler;
    
    private readonly IAsyncResultHandler<Update> _updateHandler;
    
    private readonly ITelegramBotClient _botClient;

    private readonly CancellationTokenSource _cancellationTokenSource;
    
    private readonly TelegramBotConfigurationData _configurationData;
    
    private Schedule[] _schedules = null!;

    public TelegramBotController(
        IDataContainerModel<Schedule[]> schedulesContainerModel,
        IReactiveValue<Schedule[]> reactiveValue,
        IDataContainerModel<ClientData[]> clientsDataContainerModel,
        IAsyncReactiveValue<ClientData> reactiveClientData,
        IAsyncResultHandler<Exception> errorHandler,
        IAsyncResultHandler<Update> updateHandler)
    {
        _schedulesContainerModel = schedulesContainerModel;
        
        _reactiveValue = reactiveValue;
        
        _clientsDataContainerModel = clientsDataContainerModel;
        
        _reactiveClientData = reactiveClientData;
        
        _errorHandler = errorHandler;
        
        _updateHandler = updateHandler;

        IDataReader<TelegramBotConfigurationData> dataReader = new JsonReader<TelegramBotConfigurationData>();

        IFileReader<TelegramBotConfigurationData> fileReader = new FileStreamDataReader<TelegramBotConfigurationData>();
        
        _cancellationTokenSource = new CancellationTokenSource();

        _configurationData = dataReader.ReadData(fileReader, _configurationPath);

        _botClient = new TelegramBotClient(_configurationData.BotAccessToken);
    }

    void IInitializable.Initialize()
    {
        _botClient.StartReceiving(
            _updateHandler.HandleResultAsync,
            _errorHandler.HandleResultAsync,
            cancellationToken: _cancellationTokenSource.Token,
            receiverOptions: new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            });

        _schedules = _schedulesContainerModel.GetData() ?? Array.Empty<Schedule>();
        
        _reactiveValue.ValueChanged += UpdateSchedules;
        _reactiveClientData.ValueChangedAsync += HandleClientDataUpdate;
    }

    void IDisposable.Dispose()
    {
        _cancellationTokenSource.Dispose();

        _reactiveValue.ValueChanged -= UpdateSchedules;
        _reactiveClientData.ValueChangedAsync -= HandleClientDataUpdate;
    }
    
    private async Task InitializeKeyboard(long chatId, string message)
    {
        if (_schedules.Length == 0)
        {
            await _botClient.SendTextMessageAsync(chatId, _configurationData.NullDataMessage);
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

        await _botClient.SendTextMessageAsync(chatId, message, 0,
            replyMarkup: keyboardMarkup);
    }
    
    private async Task HandleClientDataUpdate(ClientData clientData)
    {
        switch (clientData.UpdateType)
        {
            case Enums.UpdateType.None:
                var validatedValue = await ValidateSubscriptionAsync(clientData, _configurationData.SubscriptionCanceledMessage);
                
                if (!validatedValue) return;
                
                await InitializeKeyboard(clientData.ChatId, _configurationData.StartupMessage);
                
                break;
            case Enums.UpdateType.DataUpdateRequired:
                validatedValue = await ValidateSubscriptionAsync(clientData, string.Empty);
                
                if (!validatedValue) return;

                await InitializeKeyboard(clientData.ChatId, _configurationData.DataUpdateMessage);
                break;
            case Enums.UpdateType.ScheduleRequired:
                validatedValue = await ValidateSubscriptionAsync(clientData, _configurationData.SubscriptionErrorMessage);
                
                if (!validatedValue) return;
                
                await SendScheduleAsync(clientData);
                break;
            default:
                validatedValue = await ValidateSubscriptionAsync(clientData, _configurationData.SubscriptionErrorMessage);
                
                if (!validatedValue) return;
                
                await SendWrongInputErrorAsync(clientData);
                break;
        }
    }

    private void UpdateSchedules(Schedule[] schedules)
    {
        _schedules = schedules;
        
        var data = _clientsDataContainerModel.GetData();

        if (data != null)
        {
            foreach (var clientStatus in data)
            {
                if (clientStatus.UpdateType != Enums.UpdateType.DataUpdateRequired)
                {
                    _clientsDataContainerModel.SaveData(new []
                        {new ClientData(clientStatus.ChatId, clientStatus.SubscriptionType,
                            Enums.UpdateType.DataUpdateRequired, clientStatus.LastMessage)});
                }
            }
        }
    }

    private async Task SendScheduleAsync(ClientData clientData)
    {
        if (!_schedules.Any(x => x.TeacherData.FullName.Equals(clientData.LastMessage)))
        {
            await SendWrongInputErrorAsync(clientData);
            
            return;
        }
        
        await _botClient.SendTextMessageAsync(clientData.ChatId, _schedules.First
            (x => x.TeacherData.FullName.Contains(clientData.LastMessage)).ToString(),
            cancellationToken: _cancellationTokenSource.Token);
    }

    private async Task SendWrongInputErrorAsync(ClientData clientData)
    {
        await InitializeKeyboard(clientData.ChatId, _configurationData.ClientRequestErrorMessage);
    }

    private async Task<bool> ValidateSubscriptionAsync(ClientData clientData, string notValidatedMessage)
    {
        if (clientData.SubscriptionType != SubscriptionType.Subscribed)
        {
            if (notValidatedMessage.Equals(string.Empty)) return false;
            
            await _botClient.SendTextMessageAsync(clientData.ChatId, notValidatedMessage);

            return false;
        }

        return true;
    }
}