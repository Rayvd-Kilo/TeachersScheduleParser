using System;

namespace TeachersScheduleParser.Runtime.Structs;

public struct MarkupValue<T,K> where T : Enum
{
    public readonly T Markup;

    public readonly K Value;

    public MarkupValue(T markup, K value)
    {
        Markup = markup;
        Value = value;
    }
}