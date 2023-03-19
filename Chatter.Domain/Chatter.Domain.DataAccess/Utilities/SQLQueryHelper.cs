using Dapper;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using static Dapper.SqlMapper;

namespace Chatter.Domain.DataAccess.Utilities
{
    internal abstract class SQLQueryHelper
    {
        public string CreateQueryUpdateParameters<TModel>(TModel model, DynamicParameters parameters)
        {
            PropertyInfo[] modelProperties = model.GetType().GetProperties();
            var builder = new StringBuilder();
            foreach (PropertyInfo modelProperty in modelProperties)
            {
                var propertyValue = modelProperty.GetValue(model);
                if (propertyValue != null && modelProperty.Name.ToUpper() != "ID") 
                {
                    var updateQueryPart = $"{modelProperty.Name} = @{modelProperty.Name}";
                    builder.AppendLine(updateQueryPart);
                    parameters.Add($"@{modelProperty.Name}", propertyValue);
                }
            }
            return builder.ToString();
        }

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
