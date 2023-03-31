namespace Chatter.Domain.API.Models
{
    public class RemovedResponse
    {
        public RemovedResponse()
        {
        }

        public RemovedResponse(Guid id)
        {
            RemovedId = id;
        }

        public Guid RemovedId { get; set; }
    }
}
