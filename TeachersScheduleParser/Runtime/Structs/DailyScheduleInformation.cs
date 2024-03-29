using DataReaders.ValueTypes;

namespace TeachersScheduleParser.Runtime.Structs
{
    public struct DailyScheduleInformation
    {
        public readonly DataInMatrix<string> Date;

        public readonly MatrixData<string> ScheduleData;

        public DailyScheduleInformation(DataInMatrix<string> date, MatrixData<string> scheduleData)
        {
            Date = date;
            ScheduleData = scheduleData;
        }
    }
}