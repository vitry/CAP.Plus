using System.Collections.Generic;
using System.Linq;

namespace DotNetCore.CAP
{
    /// <summary>
    /// CapPlusHeader support add tags
    /// </summary>
    public class CapPlusHeader : CapHeader
    {
        public CapPlusHeader(IDictionary<string, string> dictionary) : base(dictionary)
        {
        }

        private string _consumeTags = "";
        public string ConsumeTag => _consumeTags;

        public void AddTags(string tags)
        {
            //排除空白字符串
            var t = tags?.Split(',', ';', System.StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim()) ?? new string[] { };
            var tagsString = string.Join(',', t);
            _consumeTags = tagsString;
        }
    }
}