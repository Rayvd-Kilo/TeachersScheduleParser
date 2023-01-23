using System.IO;

using DataReaders.Readers.Interfaces;
using DataReaders.Utils;

using Newtonsoft.Json;

using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Services;

public class SchedulesDataService : IDataContainerService<Schedule[]>
{
    private readonly string _schedulesDataPath = FilePathGetter.GetPath("SchedulesData.json");

    private readonly IDataReader<Schedule[]> _dataReader;

    private readonly IFileReader<Schedule[]> _fileReader;
    
    private Schedule[]? _schedules;

    public SchedulesDataService(
        IDataReader<Schedule[]> dataReader,
        IFileReader<Schedule[]> fileReader)
    {
        _dataReader = dataReader;
        _fileReader = fileReader;

        _schedules = GetData();
    }

    public void SaveData(Schedule[] schedules)
    {
        _schedules = schedules;

        var jSonData = JsonConvert.SerializeObject(_schedules);
        
        File.WriteAllText(_schedulesDataPath, jSonData);
    }

    public Schedule[]? GetData()
    {
        if (_schedules?.Length > 0) return _schedules;

        if (!File.Exists(_schedulesDataPath)) return null;
        
        _schedules = _dataReader.ReadData(_fileReader, _schedulesDataPath);

        return _schedules;
    }
}