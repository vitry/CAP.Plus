using DotNetCore.CAP;
using DotNetCore.CAP.Plus;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CapOptionsExtensions
    {
        public static CapOptions UsePlus(this CapOptions options)
        {
            return options.UsePlus(_ => { });
        }

        public static CapOptions UsePlus(this CapOptions options, Action<PlusOptions> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            options.RegisterExtension(new PlusCapOptionsExtension(configure));
            return options;
        }
    }
}