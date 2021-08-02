using DotNetCore.CAP;
using DotNetCore.CAP.MongoDB;
using DotNetCore.CAP.MongoDB.Plus;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CapOptionsExtensions
    {
        public static CapOptions UseMongoDBPlus(this CapOptions options)
        {
            return options.UseMongoDBPlus(_ => { });
        }

        public static CapOptions UseMongoDBPlus(this CapOptions options, string connectionString)
        {
            return options.UseMongoDBPlus(x => { x.DatabaseConnection = connectionString; });
        }

        public static CapOptions UseMongoDBPlus(this CapOptions options, Action<MongoDBOptions> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));

            options.RegisterExtension(new MongoDBPlusCapOptionsExtension(configure));

            return options;
        }
    }
}