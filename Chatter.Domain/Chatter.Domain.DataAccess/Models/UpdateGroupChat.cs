namespace Chatter.Domain.DataAccess.Models
{
    internal class UpdateGroupChat
    {
        public Guid ID { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }
}
