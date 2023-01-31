using System;

using Newtonsoft.Json;

namespace TeachersScheduleParser.Runtime.Structs;

[Serializable]
public struct ClientReports
{
    [JsonProperty]
    public readonly ClientData ClientData;

    [JsonProperty]
    public readonly string[] Reports;

    public ClientReports(ClientData clientData, string[] reports)
    {
        ClientData = clientData;
        Reports = reports;
    }
}