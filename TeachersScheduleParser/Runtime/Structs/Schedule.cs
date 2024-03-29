using System;
using System.Text;

namespace TeachersScheduleParser.Runtime.Structs
{
    [Serializable]
    public struct Schedule
    {
        public PersonData PersonData { get; }

        public DailySchedule[] DailySchedules { get; }

        public Schedule(PersonData personData, DailySchedule[] dailySchedules)
        {
            PersonData = personData;

            DailySchedules = dailySchedules;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(PersonData.FullName);

            foreach (var dailySchedule in DailySchedules)
            {
                if (dailySchedule.Subjects.Length == 0) continue;
                
                stringBuilder.AppendLine(dailySchedule.ToString(PersonData.PersonType));

                stringBuilder.AppendLine(string.Empty);
            }

            return stringBuilder.ToString();
        }
    }
}