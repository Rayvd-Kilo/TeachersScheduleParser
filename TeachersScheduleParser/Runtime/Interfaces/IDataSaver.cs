namespace TeachersScheduleParser.Runtime.Interfaces;

public interface IDataSaver<in T>
{
    void SaveData(T data);
}