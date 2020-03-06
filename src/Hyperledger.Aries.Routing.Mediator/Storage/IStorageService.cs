using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Decorators.Attachments;

namespace Hyperledger.Aries.Routing.Mediator.Storage
{
    public interface IStorageService
    {
        Task<string> StoreBackupAsync(string backupId, Attachment[] payload);
        Task<byte[]> RetrieveBackupAsync(string backupId);
        Task<IEnumerable<string>> ListBackupsAsync(string backupId);
        Task<byte[]> RetrieveBackupAsync(string backupId, DateTimeOffset date);
    }
}