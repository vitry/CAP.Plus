namespace DotNetCore.CAP.Plus
{
    public class PlusOptions
    {
        public const int DEFAULT_RESUME_FETCH_COUNT = 1000;
        public const int DEFAULT_RETRY_FETCH_COUNT = 1000;
        public const int DEFAULT_RETRY_IMMEDIATELY_TIMES = 3;
        public const int DEFAULT_IDLE_INTERVAL = 10;

        public int IdleInterval { get; set; } = DEFAULT_IDLE_INTERVAL;

        public int ResumePreFetchCount { get; set; } = DEFAULT_RESUME_FETCH_COUNT;

        public int RetryPreFetchCount { get; set; } = DEFAULT_RETRY_FETCH_COUNT;

        public int RetryImmediatelyTimes { get; set; } = DEFAULT_RETRY_IMMEDIATELY_TIMES;

        private int[] _failRetryIntervals;

        public int[] FailRetryIntervals
        {
            get
            {
                if (_failRetryIntervals == null || _failRetryIntervals.Length == 0)
                {
                    int[] defaultIntervals = new int[] { 10, 20, 30, 40, 50 };
                    _failRetryIntervals = defaultIntervals;
                }
                return _failRetryIntervals;
            }
            set { _failRetryIntervals = value; }
        }
    }
}