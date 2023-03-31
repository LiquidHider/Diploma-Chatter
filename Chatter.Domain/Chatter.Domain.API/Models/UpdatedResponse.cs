namespace Chatter.Domain.API.Models
{
    public class UpdatedResponse
    {
        public UpdatedResponse()
        {
        }

        public UpdatedResponse(Guid id)
        {
            UpdatedId = id;
        }

        public Guid UpdatedId { get; set; }
    }
}
