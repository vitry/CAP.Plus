using DotNetCore.CAP.Persistence;
using DotNetCore.CAP.Plus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

namespace DotNetCore.CAP.MongoDB.Plus
{
    public class MongoDBPlusCapOptionsExtension : ICapOptionsExtension
    {
        private readonly Action<MongoDBOptions> _configure;

        public MongoDBPlusCapOptionsExtension(Action<MongoDBOptions> configure)
        {
            this._configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<CapStorageMarkerService>();

            services.AddSingleton<IDataStoragePlus, MongoDBPlusDataStorage>();
            services.AddSingleton<IDataStorage>(p => p.GetRequiredService<IDataStoragePlus>());
            services.AddSingleton<IStorageInitializer, MongoDBPlusStorageInitializer>();

            services.Configure(_configure);

            //Try to add IMongoClient if does not exists
            services.TryAddSingleton<IMongoClient>(x =>
            {
                var options = x.GetService<IOptions<MongoDBOptions>>().Value;
                return new MongoClient(options.DatabaseConnection);
            });
        }
    }
}