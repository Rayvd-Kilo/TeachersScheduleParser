using System;

namespace TeachersScheduleParser.Runtime.Interfaces;

public interface IReactiveValue<out T>
{
    event Action<T> ValueChanged;
}