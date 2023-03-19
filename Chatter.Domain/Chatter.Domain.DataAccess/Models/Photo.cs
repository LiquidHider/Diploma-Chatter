﻿namespace Chatter.Domain.DataAccess.Models
{
    internal class Photo
    {
        public Guid ID { get; set; }

        public string Url { get; set; }

        public bool IsMain { get; set; }

        public Guid UserID { get; set; }
    }
}
