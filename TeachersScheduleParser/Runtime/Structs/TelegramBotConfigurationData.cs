using System;

using Newtonsoft.Json;

namespace TeachersScheduleParser.Runtime.Structs;

[Serializable]
public struct TelegramBotConfigurationData
{
    [JsonProperty] public readonly string BotAccessToken;

    [JsonProperty] public readonly string NullDataMessage;

    [JsonProperty] public readonly string StartupMessage;

    [JsonProperty] public readonly string ErrorMessage;

    public TelegramBotConfigurationData(
        string botAccessToken,
        string nullDataMessage,
        string startupMessage,
        string errorMessage)
    {
        BotAccessToken = botAccessToken;
        NullDataMessage = nullDataMessage;
        StartupMessage = startupMessage;
        ErrorMessage = errorMessage;
    }
}