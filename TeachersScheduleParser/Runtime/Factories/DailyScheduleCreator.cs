using System.Collections.Generic;
using System.Linq;

using DataReaders.Utils;

using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Factories
{
    public class DailyScheduleCreator
    {
        private const string WeekendKey = "СУББОТА";

        private readonly SubjectCreator _subjectCreator;

        public DailyScheduleCreator()
        {
            _subjectCreator = new SubjectCreator();
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

                        var subject = _subjectCreator.CreateSubject(dataRow, isDaily);

                        if (subjects.Any(x =>
                                x.SubjectName.Equals(subject.SubjectName) && x.SubjectTime.Equals(subject.SubjectTime)))
                        {
                            var index = subjects.IndexOf(subjects.First(x => x.SubjectName.Equals(subject.SubjectName)
                                                                 && x.SubjectTime.Equals(subject.SubjectTime)));

                            subjects[index] = RewriteSubjectGroup(subjects[index], subject.Group);
                            
                            continue;
                        }

                        subjects.Add(subject);
                    }
                }
            }

            return new DailySchedule(dateValue, subjects.ToArray());
        }

        private Subject RewriteSubjectGroup(Subject oldSubject, string newGroupValue)
        {
            var newGroupText = oldSubject.Group + " | " + newGroupValue;

            return new Subject(oldSubject.SubjectOrderNumber, oldSubject.SubjectTime, oldSubject.SubjectName,
                oldSubject.SubjectType, oldSubject.Cabinet, newGroupText);
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