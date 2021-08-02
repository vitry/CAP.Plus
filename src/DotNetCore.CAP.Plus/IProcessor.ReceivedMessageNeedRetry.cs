using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.CAP.Plus
{
    public class ReceivedMessageNeedToRetryProcessor : IProcessor
    {
        private readonly ILogger<ReceivedMessageNeedToRetryProcessor> _logger;
        private readonly ISubscribeDispatcher _subscribeDispatcher;
        private readonly TimeSpan _waitingInterval;

        public ReceivedMessageNeedToRetryProcessor(
            IOptions<PlusOptions> options,
            ILogger<ReceivedMessageNeedToRetryProcessor> logger,
            ISubscribeDispatcher subscribeDispatcher)
        {
            _logger = logger;
            _subscribeDispatcher = subscribeDispatcher;
            _waitingInterval = TimeSpan.FromSeconds(options.Value.IdleInterval);
            _logger.LogInformation("Receivedmessage retry processor is started");
        }

        public async Task ProcessAsync(ProcessingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var storage = context.Provider.GetRequiredService<IDataStoragePlus>();

            bool isIdle = await ProcessReceivedAsync(storage, context);
            if (isIdle) await context.WaitAsync(_waitingInterval);
        }

        /// <summary>
        /// Received retry process
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="context"></param>
        /// <returns>Process is idle or not</returns>
        private async Task<bool> ProcessReceivedAsync(IDataStoragePlus connection, ProcessingContext context)
        {
            context.ThrowIfStopping();

            var messages = await GetSafelyAsync(connection.GetReceivedMessagesOfNeedRetry);

            Stopwatch sw = Stopwatch.StartNew();
            foreach (var message in messages)
            {
                await _subscribeDispatcher.DispatchAsync(message);
            }
            sw.Stop();
            _logger.LogDebug($"Process {messages.Count()} received messages to retry, takes time {sw.ElapsedMilliseconds} ms");

            return messages.Count() == 0;
        }

        private async Task<IEnumerable<T>> GetSafelyAsync<T>(Func<Task<IEnumerable<T>>> getMessagesAsync)
        {
            try
            {
                return await getMessagesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(1, ex, "Get messages of type '{messageType}' failed. Retrying...", typeof(T).Name);

                return Enumerable.Empty<T>();
            }
        }
    }
}