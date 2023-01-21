using System;

using Newtonsoft.Json;

namespace TeachersScheduleParser.Runtime.Structs
{
    [Serializable]
    public struct TimeConverterInstruction
    {
        [JsonProperty]
        public readonly int FirstSubjectBeginTime;
            
        [JsonProperty]
        public readonly int SubjectDuration;

        [JsonProperty]
        public readonly int[] BreakTime;

        public TimeConverterInstruction(int firstSubjectBeginTime, int subjectDuration, int[] breakTime)
        {
            FirstSubjectBeginTime = firstSubjectBeginTime;
            SubjectDuration = subjectDuration;
            BreakTime = breakTime;
        }
    }
}