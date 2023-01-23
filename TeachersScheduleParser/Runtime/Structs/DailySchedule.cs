using System;
using System.Text;

using Newtonsoft.Json;

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

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(Date);

            foreach (var subject in Subjects)
            {
                stringBuilder.AppendLine(subject.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}