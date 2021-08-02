using System;

namespace DotNetCore.CAP.Plus
{
    public interface IRetryTimer
    {
        DateTime GetNextRetryTime(DateTime now, int retriedTimes);
    }
}