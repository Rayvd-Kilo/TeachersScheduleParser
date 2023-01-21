using TeachersScheduleParser.Runtime.Enums;

namespace TeachersScheduleParser.Runtime.Utils
{
    public static class SubjectTypeExtensions
    {
        public static SubjectType ConvertToSubject(this string value)
        {
            if (value.Contains("ЛК"))
            {
                return SubjectType.Lecture;
            }
            if (value.Contains("ЛБ"))
            {
                return SubjectType.LaboratoryWork;
            }
            if (value.Contains("ПР"))
            {
                return SubjectType.Practice;
            }

            return SubjectType.Other;
        }
    }
}