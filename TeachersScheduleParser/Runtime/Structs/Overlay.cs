using System.Text;
using TeachersScheduleParser.Runtime.Enums;

namespace TeachersScheduleParser.Runtime.Structs;

public struct Overlay
{
    public readonly string Date;

    public readonly Subject[] LinkedSubjects;

    public Overlay(string date, Subject[] linkedSubjects)
    {
        Date = date;
        LinkedSubjects = linkedSubjects;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine(Date);

        foreach (var linkedSubject in LinkedSubjects)
        {
            stringBuilder.AppendLine(linkedSubject.ToString(PersonType.Moderator));
        }
        
        return stringBuilder.ToString();
    }
}