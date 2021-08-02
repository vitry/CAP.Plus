using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.CAP.Plus
{
    public class MessageNeedToResumeProcessor : IProcessor
    {
        private readonly ILogger<MessageNeedToResumeProcessor> _logger;
        private readonly ISubscribeDispatcher _subscribeDispatcher;
        private readonly IMessageSender _messageSender;

        public MessageNeedToResumeProcessor(
            ILogger<MessageNeedToResumeProcessor> logger,
            ISubscribeDispatcher subscribeDispatcher,
            IMessageSender messageSender)
        {
            this._logger = logger;
            this._subscribeDispatcher = subscribeDispatcher;
            this._messageSender = messageSender;
            _logger.LogInformation("Message resume processor is started");
        }

        public async Task ProcessAsync(ProcessingContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var storage = context.Provider.GetRequiredService<IDataStoragePlus>();
            await Task.WhenAll(ProcessPublishedAsync(storage, context), ProcessReceivedAsync(storage, context));
        }

        private async Task ProcessPublishedAsync(IDataStoragePlus connection, ProcessingContext context)
        {
            context.ThrowIfStopping();

            bool isOver = false;
            while (!isOver)
            {
                var messages = await GetSafelyAsync(connection.GetPublishedMessagesOfNeedResume);
                isOver = messages.Count() == 0;

                Stopwatch sw = Stopwatch.StartNew();
                foreach (var message in messages)
                {
                    //the message.Origin.Value maybe JObject
                    await _messageSender.SendAsync(message);
                }
                sw.Stop();
                _logger.LogDebug($"Process {messages.Count()} pulished messages to resume, takes time {sw.ElapsedMilliseconds} ms");
            }
        }

        private async Task ProcessReceivedAsync(IDataStoragePlus connection, ProcessingContext context)
        {
            context.ThrowIfStopping();

            bool isOver = false;
            while (!isOver)
            {
                var messages = await GetSafelyAsync(connection.GetReceivedMessagesOfNeedResume);
                isOver = messages.Count() == 0;

                Stopwatch sw = Stopwatch.StartNew();
                foreach (var message in messages)
                {
                    await _subscribeDispatcher.DispatchAsync(message);
                }
                sw.Stop();
                _logger.LogDebug($"Process {messages.Count()} received messages to resume, takes time {sw.ElapsedMilliseconds} ms");
            }
        }

        private async Task<IEnumerable<T>> GetSafelyAsync<T>(Func<Task<IEnumerable<T>>> getMessagesAsync)
        {
            try
            {
                return await getMessagesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(1, ex, "Get messages of type '{messageType}' failed. Resume failed...", typeof(T).Name);

                return Enumerable.Empty<T>();
            }
        }
    }
}