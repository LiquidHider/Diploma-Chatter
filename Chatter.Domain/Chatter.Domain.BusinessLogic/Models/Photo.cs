namespace Chatter.Domain.BusinessLogic.Models
{
    public class Photo
    {
        public Guid ID { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
}
