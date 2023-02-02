using TeachersScheduleParser.Runtime.Enums;

namespace TeachersScheduleParser.Runtime.Structs
{
    public struct PersonData
    {
        public readonly string FullName;

        public readonly PersonType PersonType;

        public PersonData(string fullName, PersonType personType)
        {
            FullName = fullName;
            PersonType = personType;
        }
    }
}