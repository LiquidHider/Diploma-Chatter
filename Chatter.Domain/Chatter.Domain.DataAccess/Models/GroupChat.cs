using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatter.Domain.DataAccess.Models
{
    internal class GroupChat
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
