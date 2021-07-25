using System;
using Polly;
using Polly.Retry;

namespace WorkerConsumoAPIContagem.Resilience
{
    public static class RetryExtensions
    {
        public static AsyncRetryPolicy CreatePolicy(int numberOfRetries)
        {
            return Policy
                .Handle<Exception>()
                .RetryAsync(retryCount: numberOfRetries,
                    onRetry: (_, retryCount) =>
                    {
                        var previousBackgroundColor = Console.BackgroundColor;
                        var previousForegroundColor = Console.ForegroundColor;
                        
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                        
                        Console.Out.WriteLineAsync($" ***** Retentativa: {retryCount} **** ");
                        
                        Console.BackgroundColor = previousBackgroundColor;
                        Console.ForegroundColor = previousForegroundColor;
                    });
        }
    }
}