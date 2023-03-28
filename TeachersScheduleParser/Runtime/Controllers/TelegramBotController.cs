using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DataReaders.Readers;
using DataReaders.Readers.JSONReaders;
using DataReaders.Utils;

using Newtonsoft.Json;

using TeachersScheduleParser.Runtime.Enums;
using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;
using TeachersScheduleParser.Runtime.Utils;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

using File = System.IO.File;
using UpdateType = Telegram.Bot.Types.Enums.UpdateType;

namespace TeachersScheduleParser.Runtime.Controllers;

public class TelegramBotController : IAsyncStartable, IDisposable
{
    private readonly string _configurationPath = FilePathGetter.GetPath("BotConfiguration.json");
    
    private readonly string _versionDataPath = FilePathGetter.GetPath("BotVersionData.json");
    
    private readonly IDataContainerModel<Schedule[]> _schedulesContainerModel;
    
    private readonly IDataContainerModel<ClientData[]> _clientsDataContainerModel;
    
    private readonly IAsyncReactiveValue<ClientData> _reactiveClientData;

    private readonly IAsyncReactiveValue<Schedule[]> _reactiveValue;
    
    private readonly IAsyncResultHandler<Exception> _errorHandler;
    
    private readonly IAsyncResultHandler<Update> _updateHandler;
    
    private readonly IDataSaver<ClientData> _reportsSaver;

    private readonly ITelegramBotClient _botClient;

    private readonly CancellationTokenSource _cancellationTokenSource;
    
    private readonly TelegramBotConfigurationData _configurationData;
    
    private readonly VersionData _versionData;
    
    private Schedule[] _schedules = null!;

    public TelegramBotController(
        IDataContainerModel<Schedule[]> schedulesContainerModel,
        IAsyncReactiveValue<Schedule[]> reactiveValue,
        IDataContainerModel<ClientData[]> clientsDataContainerModel,
        IAsyncReactiveValue<ClientData> reactiveClientData,
        IAsyncResultHandler<Exception> errorHandler,
        IAsyncResultHandler<Update> updateHandler,
        IDataSaver<ClientData> reportsSaver)
    {
        _schedulesContainerModel = schedulesContainerModel;
        
        _reactiveValue = reactiveValue;
        
        _clientsDataContainerModel = clientsDataContainerModel;
        
        _reactiveClientData = reactiveClientData;
        
        _errorHandler = errorHandler;
        
        _updateHandler = updateHandler;
        
        _reportsSaver = reportsSaver;

        _cancellationTokenSource = new CancellationTokenSource();

        _configurationData = new JsonReader<TelegramBotConfigurationData>().
            ReadData(new FileStreamDataReader<TelegramBotConfigurationData>(), _configurationPath);
        
        _versionData = new JsonReader<VersionData>().ReadData(new FileStreamDataReader<VersionData>(), _versionDataPath);

        _botClient = new TelegramBotClient(_configurationData.BotAccessToken);
    }

    async Task IAsyncStartable.StartAsync(CancellationToken token)
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
        
        _reactiveValue.ValueChangedAsync += UpdateSchedulesAsync;
        _reactiveClientData.ValueChangedAsync += HandleClientDataUpdateAsync;

        if (_versionData.IsOutdated)
        {
            foreach (var clientData in _clientsDataContainerModel.GetData()!)
            {
                await _botClient.SendTextMessageAsync(clientData.ChatId, _versionData.PatchNoteMessage, cancellationToken: token);
            }

            var newVersionData = new VersionData(_versionData.Version, false, _versionData.PatchNoteMessage);

            var jSonData = JsonConvert.SerializeObject(newVersionData, Formatting.Indented);
            
            await File.WriteAllTextAsync(_versionDataPath, jSonData, token);
        }
    }

    void IDisposable.Dispose()
    {
        _cancellationTokenSource.Dispose();

        _reactiveValue.ValueChangedAsync -= UpdateSchedulesAsync;
        _reactiveClientData.ValueChangedAsync -= HandleClientDataUpdateAsync;
    }
    
    private async Task InitializeKeyboardAsync(long chatId, PersonType targetType, string message)
    {
        if (_schedules.Length == 0)
        {
            await _botClient.SendTextMessageAsync(chatId, _configurationData.NullDataMessage);
        }
        
        var rows = new List<KeyboardButton[]>();
        var cols = new List<KeyboardButton>();

        if (targetType.Equals(PersonType.None))
        {
            foreach (var registrationChoice in _configurationData.RegistrationChoices)
            {
                cols.Add(new KeyboardButton(registrationChoice.Value));
            }

            var markup = new ReplyKeyboardMarkup(cols.ToArray());

            await _botClient.SendTextMessageAsync(chatId, message, replyMarkup: markup);
            
            return;
        }

        var filteredData = _schedules.Where(x => x.PersonData.PersonType.Equals(targetType)).ToArray();
        
        for (var index = 1; index < filteredData.Length + 1; index++)
        {
            cols.Add(new KeyboardButton(filteredData[index - 1].PersonData.FullName));
            
            if (index % 3 != 0) continue;
            
            rows.Add(cols.ToArray());
            
            cols = new List<KeyboardButton>();
        }

        if (cols.Count > 0)
        {
            rows.Add(cols.ToArray());
        }

        var keyboardMarkup = new ReplyKeyboardMarkup(rows.ToArray());

        await _botClient.SendTextMessageAsync(chatId, message, 0,
            replyMarkup: keyboardMarkup);
    }
    
    private async Task HandleClientDataUpdateAsync(ClientData clientData)
    {
        switch (clientData.UpdateType)
        {
            case Enums.UpdateType.None:
                
                var validatedValue = await ValidateSubscriptionAsync(clientData, _configurationData.SubscriptionCanceledMessage);
                
                if (!validatedValue) return;
                
                await InitializeKeyboardAsync(clientData.ChatId, clientData.RequirePersonType, _configurationData.StartupMessage);
                
                break;
            
            case Enums.UpdateType.DataUpdateRequired:
                
                if (clientData.SubscriptionType != SubscriptionType.Banned)
                {
                    validatedValue = await ValidateSubscriptionAsync(clientData, string.Empty);
                
                    if (!validatedValue) return;
                }

                if (clientData.RequirePersonType.Equals(PersonType.None))
                {
                    if (_configurationData.RegistrationChoices.Any(x => x.Value.Equals(clientData.LastMessage)))
                    {
                        var targetPersonType = _configurationData.RegistrationChoices
                            .First(x => x.Value.Equals(clientData.LastMessage)).Markup;
                        
                        _clientsDataContainerModel.SaveData(new [] {new ClientData(clientData.ChatId, clientData.ProfileName, 
                            clientData.SubscriptionType, targetPersonType, clientData.UpdateType, clientData.LastMessage)});
                        
                        break;
                    }
                    
                    await SendWrongInputErrorAsync(clientData);
                
                    break;
                }

                await InitializeKeyboardAsync(clientData.ChatId, clientData.RequirePersonType, _configurationData.DataUpdateMessage);
                
                break;
            
            case Enums.UpdateType.ScheduleRequired:
                
                validatedValue = await ValidateSubscriptionAsync(clientData, _configurationData.SubscriptionErrorMessage);
                
                if (!validatedValue) return;
                
                await SendScheduleAsync(clientData);
                
                break;
            
            case Enums.UpdateType.ClientStartReport:
                
                if (clientData.SubscriptionType != SubscriptionType.Banned)
                {
                    validatedValue = await ValidateSubscriptionAsync(clientData, string.Empty);
                
                    if (!validatedValue) return;
                }

                await _botClient.SendTextMessageAsync(clientData.ChatId, _configurationData.ReportStartMessage);
                
                break;

            case Enums.UpdateType.ClientReportEnded:
                
                if (clientData.SubscriptionType != SubscriptionType.Banned)
                {
                    validatedValue = await ValidateSubscriptionAsync(clientData, string.Empty);
                
                    if (!validatedValue) return;
                }
                
                _reportsSaver.SaveData(clientData);

                await InitializeKeyboardAsync(clientData.ChatId, clientData.RequirePersonType, _configurationData.ReportEndMessage);
                
                break;
            
            case Enums.UpdateType.TypeChangeRequired:
                
                if (clientData.SubscriptionType != SubscriptionType.Banned)
                {
                    validatedValue = await ValidateSubscriptionAsync(clientData, string.Empty);
                
                    if (!validatedValue) return;
                }
                
                await InitializeKeyboardAsync(clientData.ChatId, clientData.RequirePersonType, _configurationData.DataUpdateMessage);
                
                break;
            
            default:
                
                validatedValue = await ValidateSubscriptionAsync(clientData, _configurationData.SubscriptionErrorMessage);
                
                if (!validatedValue) return;
                
                await SendWrongInputErrorAsync(clientData);
                
                break;
        }
    }

    private async Task UpdateSchedulesAsync(Schedule[] schedules)
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
                        {new ClientData(clientStatus.ChatId, clientStatus.ProfileName, clientStatus.SubscriptionType, 
                            clientStatus.RequirePersonType, Enums.UpdateType.DataUpdateRequired, clientStatus.LastMessage)});
                }
            }
            
            await SendOverlayScheduleAsync(_configurationData.ModeratorData);
        }
    }

    private async Task SendScheduleAsync(ClientData clientData)
    {
        if (!_schedules.Any(x => x.PersonData.FullName.Equals(clientData.LastMessage)))
        {
            await SendWrongInputErrorAsync(clientData);
            
            return;
        }
        
        await _botClient.SendTextMessageAsync(clientData.ChatId, _schedules.First
            (x => x.PersonData.FullName.Contains(clientData.LastMessage)).ToString(),
            cancellationToken: _cancellationTokenSource.Token);
    }

    private async Task SendOverlayScheduleAsync(ClientData clientData)
    {
        var overlayList = _schedules.GetOverlaySubjects();

        if (overlayList.Length > 0)
        {
            await _botClient.SendTextMessageAsync(clientData.ChatId, _configurationData.OverlayFound);
            
            foreach (var overlaySubject in overlayList)
            {
                await _botClient.SendTextMessageAsync(clientData.ChatId, overlaySubject.ToString(PersonType.Group),
                    cancellationToken: _cancellationTokenSource.Token);
            }
        }
    }

    private async Task SendWrongInputErrorAsync(ClientData clientData)
    {
        await InitializeKeyboardAsync(clientData.ChatId, clientData.RequirePersonType, _configurationData.ClientRequestErrorMessage);
    }

    private async Task<bool> ValidateSubscriptionAsync(ClientData clientData, string notValidatedMessage)
    {
        switch (clientData.SubscriptionType)
        {
            case SubscriptionType.Unsubscribed when notValidatedMessage.Equals(string.Empty):
                return false;
            case SubscriptionType.Unsubscribed:
                await _botClient.SendTextMessageAsync(clientData.ChatId, notValidatedMessage);
                return false;
            case SubscriptionType.Banned:
                await _botClient.SendTextMessageAsync(clientData.ChatId, _configurationData.ClientBannedMessage);
                return false;
        }

        return true;
    }
}