namespace Chatter.Domain.BusinessLogic.Models.Update
{
    public class UpdateMessage
    {
        public Guid ID { get; set; }

        public string? Body { get; set; }

        public bool? IsRead { get; set; }

        public bool IsEdited { get; set; }
    }
}
