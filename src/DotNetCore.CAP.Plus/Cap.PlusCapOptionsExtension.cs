using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace DotNetCore.CAP.Plus
{
    public class PlusCapOptionsExtension : ICapOptionsExtension
    {
        private readonly Action<PlusOptions> _configure;

        public PlusCapOptionsExtension(Action<PlusOptions> configure)
        {
            this._configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<CapStorageMarkerService>();
            services.Configure(_configure);

            services.Replace(ServiceDescriptor.Singleton<IConsumerRegister, ConsumerRegister>());
            services.Replace(ServiceDescriptor.Singleton<ISubscribeInvoker, SubscribeInvoker>());

            // Rebulid processing server
            services.RemoveAll<IProcessingServer>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, ConsumerRegister>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, ResumeProcessingServer>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IProcessingServer, CapPlusProcessingServer>());

            // Queue's message processor
            services.TryAddSingleton<MessageNeedToResumeProcessor>();
            services.TryAddSingleton<PublishedMessageNeedToRetryProcessor>();
            services.TryAddSingleton<ReceivedMessageNeedToRetryProcessor>();

            services.AddSingleton<IRetryTimer, RetryTimer>();
        }
    }
}