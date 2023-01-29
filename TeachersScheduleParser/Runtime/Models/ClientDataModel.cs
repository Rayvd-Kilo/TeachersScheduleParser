using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DataReaders.Readers;
using DataReaders.Readers.Interfaces;
using DataReaders.Readers.JSONReaders;
using DataReaders.Utils;

using Newtonsoft.Json;

using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Models;

public class ClientDataModel : IDataContainerModel<ClientData[]>, IAsyncReactiveValue<ClientData>
{ 
    public event Func<ClientData, Task>? ValueChangedAsync;
    
    private readonly string _chatsIDsPath = FilePathGetter.GetPath("ChatsIDs.json");
    
    private readonly IDataReader<ClientData[]> _clientDataReader;
    
    private readonly IFileReader<ClientData[]> _clientDataFileReader;
    
    private List<ClientData> _clientStatusList;

    public ClientDataModel()
    {
        _clientStatusList = new List<ClientData>();
        
        _clientDataReader = new JsonReader<ClientData[]>();

        _clientDataFileReader = new FileStreamDataReader<ClientData[]>();
    }

    void IDataSaver<ClientData[]>.SaveData(ClientData[] data)
    {
        foreach (var value in data)
        {
            if (_clientStatusList.Any(x => x.ChatId.Equals(value.ChatId)))
            {
                HandleValueUpdate(value);
                
                continue;
            }
            
            ValueChangedAsync?.Invoke(value);
            
            _clientStatusList.Add(value);
        }

        var jSonData = JsonConvert.SerializeObject(_clientStatusList);
        
        File.WriteAllText(_chatsIDsPath, jSonData);
    }

    ClientData[]? IDataGetter<ClientData[]>.GetData()
    {
        if (_clientStatusList.Count > 0) return _clientStatusList.ToArray();

        if (!File.Exists(_chatsIDsPath)) return null;
        
        _clientStatusList = _clientDataReader.ReadData(_clientDataFileReader, _chatsIDsPath).ToList();

        return _clientStatusList.ToArray();
    }
    
    private void HandleValueUpdate(ClientData value)
    {
        var equalData = _clientStatusList.First(x => x.ChatId.Equals(value.ChatId));

        _clientStatusList[_clientStatusList.IndexOf(equalData)] = value;
        
        ValueChangedAsync?.Invoke(value);
    }
}