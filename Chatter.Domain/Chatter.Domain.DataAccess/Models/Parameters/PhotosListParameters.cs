using Chatter.Domain.Common.Enums;

namespace Chatter.Domain.DataAccess.Models.Parameters
{
    public class PhotosListParameters
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public SortOrder SortOrder { get; set; }

        public PhotoSort SortBy { get; set; }

        public IList<Guid>? UsersIDs { get; set; }

    }
}
