using Microsoft.Extensions.Options;
using System;

namespace DotNetCore.CAP.Plus
{
    public class RetryTimer : IRetryTimer
    {
        private readonly IOptions<PlusOptions> _options;
        private readonly IOptions<CapOptions> _capOptions;

        public RetryTimer(
            IOptions<PlusOptions> options,
            IOptions<CapOptions> capOptions)
        {
            this._options = options;
            this._capOptions = capOptions;
        }

        public DateTime GetNextRetryTime(DateTime now, int retriedTimes)
        {
            int[] intervals = _options.Value.FailRetryIntervals;
            int immediatelyTimes = _options.Value.RetryImmediatelyTimes;
            int totalRetryTimes = _capOptions.Value.FailedRetryCount; 

            if (retriedTimes < immediatelyTimes
                || retriedTimes >= totalRetryTimes)
                return now;

            int intervalTimes = intervals.Length;
            int currentInterval = retriedTimes - immediatelyTimes;
            if (currentInterval < intervalTimes)
                return now.AddSeconds(intervals[currentInterval]);

            int secondsOfOutOfIntervals = _capOptions.Value.FailedRetryInterval;
            return now.AddSeconds(secondsOfOutOfIntervals);
        }
    }
}