using System;
using System.Text;

using Newtonsoft.Json;

using TeachersScheduleParser.Runtime.Enums;

namespace TeachersScheduleParser.Runtime.Structs
{
    [Serializable]
    public struct DailySchedule
    {
        [JsonProperty]
        public string Date { get; }

        [JsonProperty]
        public Subject[] Subjects { get; }

        public DailySchedule(string date, Subject[] subjects)
        {
            Date = date;
            Subjects = subjects;
        }

        public string ToString(PersonType personType)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(Date);

            foreach (var subject in Subjects)
            {
                stringBuilder.AppendLine(subject.ToString(personType));
            }

            return stringBuilder.ToString();
        }
    }
}