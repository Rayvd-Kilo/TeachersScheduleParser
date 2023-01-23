using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Utils
{
    public static class SubjectOrderToTimeConverter
    {
        public static int ConvertOrderNumberToMinutes(int orderNumber, TimeConverterInstruction converterInstruction)
        {
            var minutesValue = converterInstruction.FirstSubjectBeginTime;

            for (var i = 1; i < orderNumber; i++)
            {
                minutesValue += converterInstruction.SubjectDuration + converterInstruction.BreakTime[i - 1];
            }

            return minutesValue;
        }

        public static string ConvertToStringTime(this int minutesValue)
        {
            var hours = minutesValue / 60;

            var minutes = minutesValue % 60;

            var minutesToString = minutes.ToString().Length > 1 ? minutes.ToString() : "0" + minutes;

            return $@"{hours} : {minutesToString}";
        }
    }
}