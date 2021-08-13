using System.Collections.Generic;
using System.Linq;

namespace DotNetCore.CAP.With
{
    public static class Plus
    {
        public static IDictionary<string, string> Tag(string tags)
        {
            return new Dictionary<string, string> { { Messages.PlusHeaders.TAGS, tags } };
        }
    }
}
