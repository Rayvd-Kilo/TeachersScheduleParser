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
        
        public string TeacherName { get; }
        
        public string Group { get; private set; }
        
        public string Date { get; private set; }

        public Subject(int subjectOrderNumber, string subjectTime, string subjectName, SubjectType subjectType,
            string cabinet, string teacherName, string group, string date)
        {
            SubjectOrderNumber = subjectOrderNumber;
            SubjectTime = subjectTime;
            SubjectName = subjectName;
            SubjectType = subjectType;
            Cabinet = cabinet;
            TeacherName = teacherName;
            Group = group;
            Date = date;
        }

        public string ToString(PersonType personType)
        {
            switch (personType)
            {
                case PersonType.None:
                    break;
                case PersonType.Group:
                    return $" {SubjectOrderNumber}. {SubjectName}; \n" +
                           $" Преподаватель: {TeacherName}; \n" +
                           $" Время начала: {SubjectTime}; \n" +
                           $" Кабинет: {Cabinet};";
                case PersonType.Teacher:
                    return ToString();
                case PersonType.Moderator:
                    return $" {SubjectOrderNumber}. {SubjectName}; \n" +
                           $" Преподаватель: {TeacherName}; \n" +
                           $" Группы: {Group}; \n" +
                           $" Кабинет: {Cabinet};";
                default:
                    throw new ArgumentOutOfRangeException(nameof(personType), personType, null);
            }

            return ToString();
        }

        public override string ToString()
        {
            return $"{SubjectOrderNumber}. {SubjectName}; \n" +
                   $" Группы: {Group}; \n" +
                   $" Кабинет: {Cabinet};";
        }
    }
}