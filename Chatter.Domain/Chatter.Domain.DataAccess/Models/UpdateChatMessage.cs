namespace Chatter.Domain.DataAccess.Models
{
    internal class UpdateChatMessage
    {
        public Guid ID { get; set; }
        public string? Body { get; set; }
        public bool IsEdited { get; private set; } = true;
    }
}
