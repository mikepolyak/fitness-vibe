using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace FitnessVibe.Application.Behaviors
{
    /// <summary>
    /// Performance Monitoring Behavior - the fitness performance analyst.
    /// This tracks execution times, identifies bottlenecks, and monitors system health,
    /// just like how a fitness coach monitors your workout performance and recovery metrics.
    /// </summary>
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
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
            
            // Track memory usage before execution
            var initialMemory = GC.GetTotalMemory(false);
            
            _logger.LogDebug("Starting performance monitoring for {RequestName}", requestName);

            try
            {
                var response = await next();
                
                stopwatch.Stop();
                var executionTime = stopwatch.ElapsedMilliseconds;
                
                // Track memory usage after execution
                var finalMemory = GC.GetTotalMemory(false);
                var memoryUsed = finalMemory - initialMemory;
                
                // Log performance metrics
                LogPerformanceMetrics(requestName, executionTime, memoryUsed, request);
                
                // Check for performance issues and log warnings
                CheckPerformanceThresholds(requestName, executionTime, memoryUsed);
                
                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var executionTime = stopwatch.ElapsedMilliseconds;
                
                _logger.LogError(ex, "Performance monitoring detected failure in {RequestName} after {ExecutionTime}ms", 
                    requestName, executionTime);
                
                throw;
            }
        }

        /// <summary>
        /// Logs detailed performance metrics for analysis.
        /// Like recording your workout stats for performance analysis.
        /// </summary>
        private void LogPerformanceMetrics(string requestName, long executionTime, long memoryUsed, TRequest request)
        {
            var performanceData = new
            {
                RequestType = requestName,
                ExecutionTimeMs = executionTime,
                MemoryUsedBytes = memoryUsed,
                Timestamp = DateTime.UtcNow,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };

            _logger.LogInformation("Performance: {PerformanceData}", 
                JsonSerializer.Serialize(performanceData));

            // For commands, log additional context
            if (requestName.Contains("Command"))
            {
                _logger.LogDebug("Command performance - {RequestName}: {ExecutionTime}ms, Memory: {MemoryUsed} bytes",
                    requestName, executionTime, memoryUsed);
            }
            
            // For queries, focus on response time
            if (requestName.Contains("Query"))
            {
                _logger.LogDebug("Query performance - {RequestName}: {ExecutionTime}ms", 
                    requestName, executionTime);
            }
        }

        /// <summary>
        /// Checks performance against thresholds and logs warnings.
        /// Like a fitness coach flagging when your performance drops below expectations.
        /// </summary>
        private void CheckPerformanceThresholds(string requestName, long executionTime, long memoryUsed)
        {
            // Define performance thresholds based on request type
            var (timeThreshold, memoryThreshold) = GetPerformanceThresholds(requestName);
            
            // Check execution time threshold
            if (executionTime > timeThreshold)
            {
                var severity = executionTime > timeThreshold * 2 ? "CRITICAL" : "WARNING";
                _logger.LogWarning("PERFORMANCE {Severity}: {RequestName} took {ExecutionTime}ms (threshold: {Threshold}ms)",
                    severity, requestName, executionTime, timeThreshold);
            }
            
            // Check memory usage threshold
            if (memoryUsed > memoryThreshold)
            {
                _logger.LogWarning("MEMORY WARNING: {RequestName} used {MemoryUsed} bytes (threshold: {Threshold} bytes)",
                    requestName, memoryUsed, memoryThreshold);
            }
            
            // Check for potential memory leaks
            if (memoryUsed > 10 * 1024 * 1024) // 10MB
            {
                _logger.LogError("MEMORY ALERT: {RequestName} used excessive memory: {MemoryUsed} bytes",
                    requestName, memoryUsed);
            }
        }

        /// <summary>
        /// Gets performance thresholds based on request type.
        /// Different operations have different performance expectations.
        /// </summary>
        private (long timeThreshold, long memoryThreshold) GetPerformanceThresholds(string requestName)
        {
            // Default thresholds
            long timeThreshold = 1000; // 1 second
            long memoryThreshold = 1024 * 1024; // 1MB

            // Adjust thresholds based on request type
            if (requestName.Contains("Query"))
            {
                timeThreshold = requestName switch
                {
                    var name when name.Contains("Live") => 500, // Live queries should be faster
                    var name when name.Contains("Search") => 2000, // Search can take longer
                    var name when name.Contains("Report") => 5000, // Reports can take even longer
                    _ => 1000
                };
            }
            else if (requestName.Contains("Command"))
            {
                timeThreshold = requestName switch
                {
                    var name when name.Contains("Start") || name.Contains("Complete") => 2000, // Activity operations
                    var name when name.Contains("Register") || name.Contains("Login") => 1500, // Auth operations
                    var name when name.Contains("Upload") || name.Contains("Process") => 10000, // File operations
                    _ => 2000
                };
                
                // Commands typically use more memory
                memoryThreshold = 2 * 1024 * 1024; // 2MB
            }

            return (timeThreshold, memoryThreshold);
        }
    }
}
