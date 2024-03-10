using System;
using Newtonsoft.Json;
using TeachersScheduleParser.Runtime.Enums;

namespace TeachersScheduleParser.Runtime.Structs
{
    [Serializable]
    public struct PersonData
    {
        [JsonProperty]
        public string FullName { get; }

        [JsonProperty]
        public PersonType PersonType { get; }

        public PersonData(string fullName, PersonType personType)
        {
            FullName = fullName;
            PersonType = personType;
        }
    }
}