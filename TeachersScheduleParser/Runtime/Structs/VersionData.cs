using Newtonsoft.Json;

namespace TeachersScheduleParser.Runtime.Structs;

public struct VersionData
{
    [JsonProperty] public readonly string Version;

    [JsonProperty] public readonly bool IsOutdated;

    [JsonProperty] public readonly string PatchNoteMessage;

    public VersionData(string version, bool isOutdated, string patchNoteMessage)
    {
        Version = version;
        IsOutdated = isOutdated;
        PatchNoteMessage = patchNoteMessage;
    }
}