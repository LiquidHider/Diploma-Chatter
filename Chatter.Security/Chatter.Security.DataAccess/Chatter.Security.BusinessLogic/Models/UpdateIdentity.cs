﻿namespace Chatter.Security.Core.Models
{
    public class UpdateIdentity
    {
        public Guid ID { get; set; }

        public string? Email { get; set; }

        public string? UserTag { get; set; }

        public string? PasswordHash { get; set; }

        public string? PasswordKey { get; set; }
    }
}
