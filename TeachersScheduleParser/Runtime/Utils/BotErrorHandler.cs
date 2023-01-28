using System;
using System.Threading;
using System.Threading.Tasks;

using TeachersScheduleParser.Runtime.Interfaces;

using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace TeachersScheduleParser.Runtime.Utils;

public class BotErrorHandler : IAsyncResultHandler<Exception>
{
    Task IAsyncResultHandler<Exception>.HandleResultAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}