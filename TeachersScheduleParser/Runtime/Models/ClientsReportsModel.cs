using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DataReaders.Readers;
using DataReaders.Readers.Interfaces;
using DataReaders.Readers.JSONReaders;
using DataReaders.Utils;

using Newtonsoft.Json;

using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Models;

public class ClientsReportsModel : IDataSaver<ClientData>, IDataGetter<ClientReports[]>
{
    private readonly string _clientReportsPath = FilePathGetter.GetPath("Reports.json");
    
    private readonly IDataReader<ClientReports[]> _clientReportsReader;
    
    private readonly IFileReader<ClientReports[]> _clientReportsFileReader;
    
    private List<ClientReports> _clientReportsList;

    public ClientsReportsModel()
    {
        _clientReportsReader = new JsonReader<ClientReports[]>();
        
        _clientReportsFileReader = new FileStreamDataReader<ClientReports[]>();

        _clientReportsList = GetData()?.ToList() ?? new List<ClientReports>();
    }
    
    public void SaveData(ClientData data)
    {
        if (_clientReportsList.Any(x => x.ClientData.ChatId.Equals(data.ChatId)))
        {
            HandleValueUpdate(data);
        }
        else
        {
            _clientReportsList.Add(new ClientReports(data, new []{data.LastMessage}));
        }
        
        var jSonData = JsonConvert.SerializeObject(_clientReportsList, Formatting.Indented);
        
        File.WriteAllText(_clientReportsPath, jSonData);
    }

    public ClientReports[]? GetData()
    {
        if (_clientReportsList?.Count > 0) return _clientReportsList.ToArray();

        if (!File.Exists(_clientReportsPath)) return null;
        
        _clientReportsList = _clientReportsReader.ReadData(_clientReportsFileReader, _clientReportsPath).ToList();

        return _clientReportsList.ToArray();
    }
    
    private void HandleValueUpdate(ClientData data)
    {
        var equalData = _clientReportsList.First(x => x.ClientData.ChatId.Equals(data.ChatId));

        var reports = _clientReportsList[_clientReportsList.IndexOf(equalData)].Reports.ToList()
                      ?? throw new ArgumentNullException(nameof(data));

        reports.Add(data.LastMessage);

        _clientReportsList[_clientReportsList.IndexOf(equalData)] = new ClientReports(data, reports.ToArray());
    }
}