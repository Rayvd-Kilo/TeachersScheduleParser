using System;
using System.Text;

namespace ExelParser.Runtime.Structs
{
    [Serializable]
    public struct DailySchedule
    {
        public string Date { get; }

        public Subject[] Subjects { get; }

        public DailySchedule(string date, Subject[] subjects)
        {
            Date = date;
            Subjects = subjects;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(Date);

            foreach (var subject in Subjects)
            {
                stringBuilder.AppendLine(subject.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}