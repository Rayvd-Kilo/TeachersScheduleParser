using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;

namespace TeachersScheduleParser.Runtime.Interfaces;

public interface IAsyncResultHandler<in T>
{
    Task HandleResultAsync(ITelegramBotClient botClient, T update, CancellationToken cancellationToken);
}