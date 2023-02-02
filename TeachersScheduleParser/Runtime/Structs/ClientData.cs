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
    public readonly string ProfileName;

    [JsonProperty]
    public readonly SubscriptionType SubscriptionType;

    [JsonProperty]
    public readonly PersonType RequirePersonType;

    [JsonProperty]
    public readonly UpdateType UpdateType;

    [JsonProperty]
    public readonly string LastMessage;

    public ClientData(
        long chatId,
        string profileName,
        SubscriptionType subscriptionType, 
        PersonType requirePersonType,
        UpdateType updateType,
        string lastMessage)
    {
        ChatId = chatId;
        ProfileName = profileName;
        SubscriptionType = subscriptionType;
        UpdateType = updateType;
        LastMessage = lastMessage;
        RequirePersonType = requirePersonType;
    }
}