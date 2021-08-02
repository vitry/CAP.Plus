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
    public class PublishedMessageNeedToRetryProcessor : IProcessor
    {
        private readonly ILogger<PublishedMessageNeedToRetryProcessor> _logger;
        private readonly IMessageSender _messageSender;
        private readonly TimeSpan _waitingInterval;

        public PublishedMessageNeedToRetryProcessor(
            IOptions<PlusOptions> options,
            ILogger<PublishedMessageNeedToRetryProcessor> logger,
            IMessageSender messageSender)
        {
            _logger = logger;
            _messageSender = messageSender;
            _waitingInterval = TimeSpan.FromSeconds(options.Value.IdleInterval);
            _logger.LogInformation("Publishedmessage retry processor is started");
        }

        public async Task ProcessAsync(ProcessingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var storage = context.Provider.GetRequiredService<IDataStoragePlus>();

            bool isIdle = await ProcessPublishedAsync(storage, context);
            if (isIdle) await context.WaitAsync(_waitingInterval);
        }

        /// <summary>
        /// Published retry process
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="context"></param>
        /// <returns>Process is idle or not</returns>
        private async Task<bool> ProcessPublishedAsync(IDataStoragePlus connection, ProcessingContext context)
        {
            context.ThrowIfStopping();

            var messages = await GetSafelyAsync(connection.GetPublishedMessagesOfNeedRetry);

            Stopwatch sw = Stopwatch.StartNew();
            foreach (var message in messages)
            {
                //the message.Origin.Value maybe JObject
                await _messageSender.SendAsync(message);
            }
            sw.Stop();
            _logger.LogDebug($"Process {messages.Count()} pulished messages to retry, takes time {sw.ElapsedMilliseconds} ms");

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