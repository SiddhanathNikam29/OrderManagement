using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace OrderManagement.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private readonly Stopwatch _stopwatch;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Processing {RequestName} at {Timestamp}",
                requestName, DateTime.UtcNow);

            _stopwatch.Start();

            try
            {
                var response = await next();
                _stopwatch.Stop();

                _logger.LogInformation("Completed {RequestName} in {ElapsedMilliseconds}ms",
                    requestName, _stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                _stopwatch.Stop();
                _logger.LogError(ex, "Error processing {RequestName} after {ElapsedMilliseconds}ms",
                    requestName, _stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}