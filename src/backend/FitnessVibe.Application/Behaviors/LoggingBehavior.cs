using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FitnessVibe.Application.Behaviors
{
    /// <summary>
    /// Logging Behavior - the fitness tracker that monitors every workout.
    /// This logs performance metrics, execution times, and important events,
    /// just like how a fitness tracker monitors your heart rate, steps, and calories.
    /// </summary>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Starting execution of {RequestName}", requestName);

            try
            {
                var response = await next();
                
                stopwatch.Stop();
                
                _logger.LogInformation("Completed execution of {RequestName} in {ExecutionTime}ms", 
                    requestName, stopwatch.ElapsedMilliseconds);

                // Log performance warnings for slow operations (like flagging a workout that's taking too long)
                if (stopwatch.ElapsedMilliseconds > 3000) // 3 seconds
                {
                    _logger.LogWarning("Slow execution detected for {RequestName}: {ExecutionTime}ms", 
                        requestName, stopwatch.ElapsedMilliseconds);
                }

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                _logger.LogError(ex, "Failed execution of {RequestName} after {ExecutionTime}ms", 
                    requestName, stopwatch.ElapsedMilliseconds);
                
                throw;
            }
        }
    }
}
