using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chatter.Domain.DataAccess.Utilities
{
    internal abstract class SQLQueryHelper
    {
        public string Where(params string[] queryParts)
        {
            var builder = new StringBuilder();
            builder.AppendLine("WHERE ");
            builder.AppendLine(queryParts.First());
            foreach (var part in queryParts.Where(x => x != queryParts.First()))
            {
                builder.AppendLine($"AND {part}");
            }

            return builder.ToString();
        }

        public string OrderBy(string order,params string[] columns)
        {
            var builder = new StringBuilder();
            builder.AppendLine("ORDER BY ");
            foreach (var part in columns.Where(x => x != columns.Last()))
            {
                builder.AppendLine($"{part},");
            }
            builder.AppendLine(columns.Last());
            builder.AppendLine(order);
            return builder.ToString();
        }
    }
}
