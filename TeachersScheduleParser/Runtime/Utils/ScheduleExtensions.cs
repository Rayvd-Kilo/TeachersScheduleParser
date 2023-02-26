using System.Collections.Generic;
using System.Linq;

using DataReaders.Readers.RegexReaders;

using TeachersScheduleParser.Runtime.Enums;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser.Runtime.Utils;

public static class ScheduleExtensions
{
    private const string Pattern = "([А-Я]*\\-[0-9]{2})";

    private static readonly string[] IgnoreCabinets = {"ФОК"};
    
    public static Subject[] GetOverlaySubjects(this Schedule schedule)
    {
        var overlaySubjectsList = new List<Subject>();
        
        var regexReader = new BaseRegexReader(Pattern);

        foreach (var dailySchedule in schedule.DailySchedules)
        {
            foreach (var subject in dailySchedule.Subjects)
            {
                var matches = regexReader.GetMatches(subject.Group);

                var firstMatch = matches.First().ToString().Remove(0,3);

                if (matches.Any(x => !x.ToString().Remove(0, 3).Equals(firstMatch)))
                {
                    overlaySubjectsList.Add(subject);
                }
            }
        }

        return overlaySubjectsList.ToArray();
    }

    public static Subject[] GetOverlaySubjects(this Schedule[] schedules)
    {
        var overlaySubjectsList = new List<Subject>();

        var filteredSchedules = schedules.Where(x => x.PersonData.PersonType
            .Equals(PersonType.Teacher)).ToArray();

        foreach (var schedule in filteredSchedules)
        {
            overlaySubjectsList.AddRange(schedule.GetOverlaySubjects());

            foreach (var dailySchedule in schedule.DailySchedules)
            {
                foreach (var subject in dailySchedule.Subjects)
                {
                    if (filteredSchedules.Where(x => x.PersonData.FullName != schedule.PersonData.FullName).Any(x =>
                        {
                            var daily = x.DailySchedules.FirstOrDefault(z => z.Date.Equals(dailySchedule.Date));

                            var dailySubject = daily.Subjects.FirstOrDefault(z =>
                                z.SubjectOrderNumber.Equals(subject.SubjectOrderNumber));

                            if (string.IsNullOrEmpty(dailySubject.Cabinet))
                            {
                                return false;
                            }

                            var isOverlay = dailySubject.Cabinet.Equals(subject.Cabinet) &&
                                            !IgnoreCabinets.Contains(subject.Cabinet);

                            if (isOverlay)
                            {
                                if (dailySubject.TeacherName.Contains("\n"))
                                {
                                    isOverlay = false;
                                }
                            }

                            return isOverlay;
                        }))
                    {
                        overlaySubjectsList.Add(subject);
                    }
                }
            }
        }

        return overlaySubjectsList.ToArray();
    }
}