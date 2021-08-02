using DotNetCore.CAP.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCore.CAP.Plus
{
    public interface IDataStoragePlus : IDataStorage
    {
        Task<IEnumerable<MediumMessage>> GetPublishedMessagesOfNeedResume();

        Task<IEnumerable<MediumMessage>> GetReceivedMessagesOfNeedResume();

        Task<long?> GetLastShouldResumePublishedMessageId();

        Task<long?> GetLastShouldResumeReceivedMessageId();

        Task InitResumeBoundary { get; }
    }
}