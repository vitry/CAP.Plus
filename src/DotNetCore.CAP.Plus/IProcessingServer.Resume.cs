using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCore.CAP.Plus
{
    public class ResumeProcessingServer : IProcessingServer
    {
        private readonly ILogger<ResumeProcessingServer> _logger;
        private readonly IServiceProvider _provider;
        private readonly CancellationTokenSource _cts;

        private ProcessingContext _context;
        private Task _compositeTask;
        private bool _disposed;

        public ResumeProcessingServer(
            ILogger<ResumeProcessingServer> logger,
            IServiceProvider provider)
        {
            this._logger = logger;
            this._provider = provider;
            this._cts = new CancellationTokenSource();
        }

        public void Start()
        {
            _logger.ServerStarting();
            _context = new ProcessingContext(_provider, _cts.Token);

            var processor = _provider.GetRequiredService<MessageNeedToResumeProcessor>();
            _compositeTask = processor.ProcessAsync(_context);
        }

        public void Pulse()
        {
            _logger.LogTrace("Pulsing the processor.");
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                _disposed = true;

                _logger.ServerShuttingDown();
                _cts.Cancel();

                _compositeTask?.Wait((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
            }
            catch (AggregateException ex)
            {
                var innerEx = ex.InnerExceptions[0];
                if (!(innerEx is OperationCanceledException))
                {
                    _logger.ExpectedOperationCanceledException(innerEx);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An exception was occurred when disposing.");
            }
            finally
            {
                _logger.LogInformation("### CAP shutdown!");
            }
        }
    }
}