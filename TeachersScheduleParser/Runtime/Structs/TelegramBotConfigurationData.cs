using System;

using Newtonsoft.Json;

namespace TeachersScheduleParser.Runtime.Structs;

[Serializable]
public struct TelegramBotConfigurationData
{
    [JsonProperty] public readonly string BotAccessToken;

    [JsonProperty] public readonly string NullDataMessage;

    [JsonProperty] public readonly string StartupMessage;

    [JsonProperty] public readonly string DataUpdateMessage;

    [JsonProperty] public readonly string ClientRequestErrorMessage;

    [JsonProperty] public readonly string SubscriptionCanceledMessage;

    [JsonProperty] public readonly string SubscriptionErrorMessage;

    [JsonProperty] public readonly string ClientBannedMessage;

    [JsonProperty] public readonly string ErrorMessage;

    public TelegramBotConfigurationData(
        string botAccessToken,
        string nullDataMessage,
        string startupMessage,
        string errorMessage,
        string clientRequestErrorMessage, 
        string dataUpdateMessage,
        string subscriptionErrorMessage,
        string subscriptionCanceledMessage, 
        string clientBannedMessage)
    {
        BotAccessToken = botAccessToken;
        NullDataMessage = nullDataMessage;
        StartupMessage = startupMessage;
        DataUpdateMessage = dataUpdateMessage;
        ClientRequestErrorMessage = clientRequestErrorMessage;
        SubscriptionCanceledMessage = subscriptionCanceledMessage;
        SubscriptionErrorMessage = subscriptionErrorMessage;
        ClientBannedMessage = clientBannedMessage;
        ErrorMessage = errorMessage;
    }
}