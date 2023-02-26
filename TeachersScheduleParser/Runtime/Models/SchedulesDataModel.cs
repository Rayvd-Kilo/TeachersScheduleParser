using System;
using System.IO;
using System.Threading.Tasks;

using DataReaders.Readers.Interfaces;
using DataReaders.Utils;

using Newtonsoft.Json;

using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Models;

public class SchedulesDataModel : IDataContainerModel<Schedule[]>, IAsyncReactiveValue<Schedule[]>
{
    private event Func<Schedule[], Task>? ValueChanged;

    event Func<Schedule[], Task>? IAsyncReactiveValue<Schedule[]>.ValueChangedAsync
    {
        add => ValueChanged += value;
        remove => ValueChanged -= value;
    }

    private readonly string _schedulesDataPath = FilePathGetter.GetPath("SchedulesData.json");

    private readonly IDataReader<Schedule[]> _dataReader;

    private readonly IFileReader<Schedule[]> _fileReader;
    
    private Schedule[]? _schedules;

    public SchedulesDataModel(
        IDataReader<Schedule[]> dataReader,
        IFileReader<Schedule[]> fileReader)
    {
        _dataReader = dataReader;
        _fileReader = fileReader;

        _schedules = ((IDataGetter<Schedule[]>) this).GetData();
    }

    void IDataSaver<Schedule[]>.SaveData(Schedule[] schedules)
    {
        _schedules = schedules;
        
        ValueChanged!.Invoke(_schedules);

        var jSonData = JsonConvert.SerializeObject(_schedules, Formatting.Indented);
        
        File.WriteAllText(_schedulesDataPath, jSonData);
    }

    Schedule[]? IDataGetter<Schedule[]>.GetData()
    {
        if (_schedules?.Length > 0) return _schedules;

        if (!File.Exists(_schedulesDataPath)) return null;
        
        _schedules = _dataReader.ReadData(_fileReader, _schedulesDataPath);

        return _schedules;
    }
}