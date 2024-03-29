using System.Collections.Generic;
using System.Linq;

using DataReaders.Readers.RegexReaders;
using DataReaders.Utils;

using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Creators
{
    public class DailyScheduleCreator
    {
        private const string WeekendKey = "СУББОТА";

        private const string GroupRegexPattern = "([А-Я]*\\-[0-9]{2}\\-+[А-Я]*(?:\\-+[0-9]){0,1})";

        private readonly SubjectCreator _subjectCreator;

        private readonly BaseRegexReader _regexReader;

        public DailyScheduleCreator()
        {
            _regexReader = new BaseRegexReader(GroupRegexPattern);

            _subjectCreator = new SubjectCreator(_regexReader);
        }

        public DailySchedule CreateDailySchedule(PersonData targetPerson,
            DailyScheduleInformation[] dailySchedulesInformation, string dateValue)
        {
            var dailyCollection = dailySchedulesInformation.Where(x => x.Date.DataValue.Equals(dateValue))
                .Where(x => HasTargetPerson(targetPerson, x.ScheduleData.Data)).ToArray();

            var subjects = new List<Subject>();

            for (int i = 0; i < dailyCollection.Length; i++)
            {
                for (int j = 0; j < dailyCollection[i].ScheduleData.Data.GetLength(0); j++)
                {
                    var dataRow = MatrixToArrayConverter<string>.GetRow(dailyCollection[i].ScheduleData.Data, j);

                    if (dataRow.Any(x => x.Contains(targetPerson.FullName)))
                    {
                        var isDaily = !dateValue.Contains(WeekendKey);

                        var subject = _subjectCreator.CreateSubject(dataRow, j + 1, isDaily, dateValue);

                        if (subject.SubjectName.Equals(string.Empty))
                        {
                            continue;
                        }

                        if (subjects.Any(x =>
                                x.SubjectName.Equals(subject.SubjectName) && x.SubjectTime.Equals(subject.SubjectTime)))
                        {
                            var index = subjects.IndexOf(subjects.First(x => x.SubjectName.Equals(subject.SubjectName)
                                                                 && x.SubjectTime.Equals(subject.SubjectTime)));

                            subjects[index] = RewriteSubjectGroup(subjects[index], subject.Group, dateValue);
                            
                            continue;
                        }

                        subjects.Add(subject);
                    }
                }
            }

            return new DailySchedule(dateValue, subjects.OrderBy(x => x.SubjectOrderNumber).ToArray());
        }

        private Subject RewriteSubjectGroup(Subject oldSubject, string newGroupValue, string dateValue)
        {
            var newGroupText = _regexReader.GetMatch(oldSubject.Group).Value + "," + _regexReader.GetMatch(newGroupValue).Value;

            return new Subject(oldSubject.SubjectOrderNumber, oldSubject.SubjectTime, oldSubject.SubjectName,
                oldSubject.SubjectType, oldSubject.Cabinet, oldSubject.TeacherName, newGroupText, dateValue);
        }

        private bool HasTargetPerson(PersonData personData, string[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j].Contains(personData.FullName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}