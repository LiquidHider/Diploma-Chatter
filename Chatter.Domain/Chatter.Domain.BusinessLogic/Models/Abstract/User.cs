namespace Chatter.Domain.BusinessLogic.Models.Abstract
{
    internal abstract class User
    {
        public Guid ID { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Patronymic { get; set; }

        public string UserTag { get; set; }
    }
}
