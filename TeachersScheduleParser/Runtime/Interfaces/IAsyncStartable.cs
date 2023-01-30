using System.Threading;
using System.Threading.Tasks;

namespace TeachersScheduleParser.Runtime.Interfaces;

public interface IAsyncStartable
{
    Task StartAsync(CancellationToken token);
}