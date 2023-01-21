using System;

using TeachersScheduleParser.Runtime.Enums;

namespace TeachersScheduleParser.Runtime.Structs
{
    [Serializable]
    public struct Subject
    {
        public int SubjectOrderNumber { get; }
        
        public string SubjectTime { get; }
        
        public string SubjectName { get; }
        
        public SubjectType SubjectType { get; }

        public string Cabinet { get; }
        
        public string Group { get; private set; }

        public Subject(int subjectOrderNumber, string subjectTime, string subjectName, SubjectType subjectType, string cabinet, string group)
        {
            SubjectOrderNumber = subjectOrderNumber;
            SubjectTime = subjectTime;
            SubjectName = subjectName;
            SubjectType = subjectType;
            Cabinet = cabinet;
            Group = group;
        }

        public override string ToString()
        {
            return $"{SubjectTime} | {SubjectOrderNumber}. {SubjectName} || {SubjectType} | {Group} | {Cabinet};";
        }
    }
}