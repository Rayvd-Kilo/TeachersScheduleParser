using System;
using System.Text;

namespace TeachersScheduleParser.Runtime.Structs
{
    [Serializable]
    public struct Schedule
    {
        public PersonData TeacherData { get; }

        public DailySchedule[] DailySchedules { get; }

        public Schedule(PersonData teacherData, DailySchedule[] dailySchedules)
        {
            TeacherData = teacherData;

            DailySchedules = dailySchedules;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(TeacherData);

            foreach (var dailySchedule in DailySchedules)
            {
                stringBuilder.AppendLine(dailySchedule.ToString());

                stringBuilder.AppendLine(string.Empty);
            }

            return stringBuilder.ToString();
        }
    }
}