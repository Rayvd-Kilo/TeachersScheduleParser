using System;

using Newtonsoft.Json;

using TeachersScheduleParser.Runtime.Enums;

namespace TeachersScheduleParser.Runtime.Structs;

[Serializable]
public struct ClientData
{
    [JsonProperty]
    public readonly long ChatId;

    [JsonProperty]
    public readonly UpdateType UpdateType;

    public ClientData(long chatId, UpdateType updateType)
    {
        ChatId = chatId;
        UpdateType = updateType;
    }
}