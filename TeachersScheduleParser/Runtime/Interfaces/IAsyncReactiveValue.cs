using System;
using System.Threading.Tasks;

namespace TeachersScheduleParser.Runtime.Interfaces;

public interface IAsyncReactiveValue<out T>
{
    event Func<T, Task> ValueChangedAsync;
}