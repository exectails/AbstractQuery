using System;
using System.Linq;
using System.Text;

namespace AbstractQuery.MySql
{
	public class SqlQueryBuilder
	{
		public SqlQueryBuilder()
		{
		}

		public string GetQueryString(Query query)
		{
			if (query.SelectElement != null)
				return this.GetSelectQueryString(query);

			throw new InvalidOperationException("Unknown query type.");
		}

		private string GetSelectQueryString(Query query)
		{
			var sb = new StringBuilder();

			var select = query.SelectElement;
			var from = query.FromElement;
			var orderBys = query.OrderByElements;
			var innerJoins = query.InnerJoinElements;

			if (from == null)
				throw new InvalidOperationException("Expected 'From' element in query.");

			// SELECT
			sb.Append("SELECT ");
			{
				var i = 0;
				var count = select.FieldNames.Count;
				foreach (var fieldName in select.FieldNames)
				{
					var name = fieldName;

					if (name != "*")
					{
						name = '`' + name + '`';
					}

					sb.Append(name);
					if (++i != count)
						sb.Append(", ");
					else
						sb.Append(" ");
				}
			}

			// FROM
			if (from.ShortName == null)
				sb.AppendFormat("FROM `{0}` ", from.TableName);
			else
				sb.AppendFormat("FROM `{0}` AS `{1}` ", from.TableName, from.ShortName);

			// INNER JOIN
			if (innerJoins != null && innerJoins.Any())
			{
				foreach (var innerJoin in innerJoins)
				{
					sb.AppendFormat("INNER JOIN `{0}` ", innerJoin.TableName);

					var name1 = '`' + innerJoin.FieldName1 + '`';
					var name2 = '`' + innerJoin.FieldName2 + '`';

					sb.AppendFormat("ON {0} = {1} ", name1, name2);
				}
			}

			// WHERE
			this.AppendWheres(sb, query);

			// ORDER BY
			if (orderBys != null && orderBys.Any())
			{
				var i = 0;
				var count = orderBys.Count;

				sb.Append("ORDER BY ");

				foreach (var orderBy in orderBys)
				{
					var name = orderBy.FieldName;

					if (name != "*")
						name = '`' + name + '`';

					sb.Append(name);
					if (orderBy.Direction == OrderDirection.Ascending)
						sb.Append(" ASC");
					else
						sb.Append(" DESC");

					if (++i != count)
						sb.Append(", ");
					else
						sb.Append(" ");
				}
			}

			// END
			sb.Append(";");

			return sb.ToString();
		}

		private void AppendWheres(StringBuilder sb, Query query)
		{
			if (query.WhereElements == null || !query.WhereElements.Any())
				return;

			var i = 0;
			var count = query.WhereElements.Count;

			sb.Append("WHERE ");

			foreach (var where in query.WhereElements)
			{
				sb.AppendFormat("`{0}` ", where.FieldName);

				var op = "=";
				switch (where.Comparison)
				{
					case Is.LowerThen: op = "<"; break;
					case Is.LowerEqualThen: op = "<="; break;
					case Is.GreaterThen: op = ">"; break;
					case Is.GreaterEqualThen: op = "<="; break;
					case Is.Equal: op = "="; break;
					case Is.Like: op = "LIKE"; break;
				}

				sb.AppendFormat("{0} ", op);

				var value = where.Value;
				if ((value is string) || !(value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal))
					sb.AppendFormat("\"{0}\" ", where.Value);
				else
					sb.AppendFormat("{0} ", where.Value);

				if (++i < count)
					sb.Append("AND ");
			}
		}
	}
}
