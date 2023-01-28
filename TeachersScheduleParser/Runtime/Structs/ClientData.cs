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

    [JsonProperty]
    public readonly string LastMessage;

    public ClientData(long chatId, UpdateType updateType, string lastMessage)
    {
        ChatId = chatId;
        UpdateType = updateType;
        LastMessage = lastMessage;
    }
}