using Chatter.Domain.Common.Enums;
using Chatter.Domain.DataAccess.Models;

namespace Chatter.Domain.DataAccess.Interfaces
{
    internal interface IChatMessageRepository
    {
        Task<ChatMessage> GetAsync(Guid id, CancellationToken cancellationToken);

        Task CreateAsync(ChatMessage item, CancellationToken cancellationToken);

        Task UpdateAsync(ChatMessage item, CancellationToken cancellationToken);

        Task<DeletionStatus> DeleteAsync(ChatMessage item, CancellationToken cancellationToken);

        Task<IList<ChatMessage>> GetAllASync(CancellationToken cancellationToken);
    }
}
