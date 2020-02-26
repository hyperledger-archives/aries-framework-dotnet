using System.Threading.Tasks;
using Hyperledger.Aries.Decorators.Attachments;

namespace Hyperledger.Aries.Routing.Mediator.Storage
{
    public interface IStorageService
    {
        Task<string> SaveWallet(string key, Attachment[] payload);
        Task<byte[]> RetrieveWallet(string key);
    }
}