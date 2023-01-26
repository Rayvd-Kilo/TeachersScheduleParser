namespace TeachersScheduleParser.Runtime.Interfaces;

public interface IDataGetter<out T>
{
    T? GetData();
}