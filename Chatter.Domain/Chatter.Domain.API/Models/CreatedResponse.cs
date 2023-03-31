namespace Chatter.Domain.API.Models
{
    public class CreatedResponse
    {
        public CreatedResponse()
        {
        }

        public CreatedResponse(Guid id)
        {
            CreatedId = id;
        }

        public Guid CreatedId { get; set; }
    }
}
