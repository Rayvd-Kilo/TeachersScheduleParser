using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Interfaces;

public interface IDataGetter<out T>
{
    Schedule[]? GetData();
}