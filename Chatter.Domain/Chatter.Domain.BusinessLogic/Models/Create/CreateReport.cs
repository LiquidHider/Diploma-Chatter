namespace Chatter.Domain.BusinessLogic.Models.Create
{
    public class CreateReport
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public Guid ReportedUserID { get; set; }
    }
}
