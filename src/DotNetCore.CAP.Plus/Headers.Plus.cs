namespace DotNetCore.CAP.Messages
{
    /// <summary>
    /// Headers use in cap plus
    /// </summary>
    public static class PlusHeaders
    {
        /// <summary>
        /// Exception Stack Trace
        /// </summary>
        public const string STACKTRACE = "cap-exception-stack";

        /// <summary>
        /// Message Tags. More tags are divided by ",".
        /// </summary>
        public const string TAGS = "cap-msg-tags";
    }
}