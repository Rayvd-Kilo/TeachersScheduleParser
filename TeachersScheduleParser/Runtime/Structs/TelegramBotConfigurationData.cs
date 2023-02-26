using System;

using Newtonsoft.Json;

using TeachersScheduleParser.Runtime.Enums;

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

    [JsonProperty] public readonly string ReportStartMessage;

    [JsonProperty] public readonly string ReportEndMessage;

    [JsonProperty] public readonly string OverlayFound;

    [JsonProperty] public readonly MarkupValue<PersonType, string>[] RegistrationChoices;

    [JsonProperty] public readonly ClientData ModeratorData;

    public TelegramBotConfigurationData(
        string botAccessToken,
        string nullDataMessage,
        string startupMessage,
        string errorMessage,
        string clientRequestErrorMessage, 
        string dataUpdateMessage,
        string subscriptionErrorMessage,
        string subscriptionCanceledMessage, 
        string clientBannedMessage,
        string reportStartMessage,
        string reportEndMessage,
        MarkupValue<PersonType, string>[] registrationChoices,
        ClientData moderatorData, string overlayFound)
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
        ReportStartMessage = reportStartMessage;
        ReportEndMessage = reportEndMessage;
        RegistrationChoices = registrationChoices;
        ModeratorData = moderatorData;
        OverlayFound = overlayFound;
    }
}