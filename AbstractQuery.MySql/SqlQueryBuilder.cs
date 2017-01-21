using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractQuery.MySql
{
	public class SqlQueryBuilder
	{
		private Dictionary<string, object> _parameters = new Dictionary<string, object>();
		private int _parameterCount;

		public SqlQueryBuilder()
		{
		}

		public IDictionary<string, object> GetParameters()
		{
			return _parameters;
		}

		public string GetQueryString(Query query)
		{
			return this.GetQueryString(query, false);
		}

		public string GetQueryString(Query query, bool parameterize)
		{
			_parameters.Clear();
			_parameterCount = 0;

			if (query.SelectElement != null)
				return this.GetSelectQueryString(query, parameterize);

			throw new InvalidOperationException("Unknown query type.");
		}

		private string GetSelectQueryString(Query query, bool parameterize)
		{
			var sb = new StringBuilder();

			var select = query.SelectElement;
			var froms = query.FromElements;
			var orderBys = query.OrderByElements;
			var innerJoins = query.InnerJoinElements;
			var limit = query.LimitElement;

			if (froms == null || !froms.Any())
				throw new InvalidOperationException("Expected 'From' element in query.");

			// SELECT
			sb.Append("SELECT ");
			{
				var i = 0;
				var count = select.FieldNames.Count;
				foreach (var fieldName in select.FieldNames)
				{
					var name = QuoteFieldName(fieldName);
					sb.Append(name);

					if (++i != count)
						sb.Append(", ");
					else
						sb.Append(" ");
				}
			}

			// FROM
			{
				var i = 0;
				var count = froms.Count;

				sb.Append("FROM ");

				foreach (var from in froms)
				{
					if (from.ShortName == null)
						sb.AppendFormat("`{0}`", from.TableName);
					else
						sb.AppendFormat("`{0}` AS `{1}`", from.TableName, from.ShortName);

					if (++i != count)
						sb.Append(", ");
					else
						sb.Append(" ");
				}
			}

			// INNER JOIN
			if (innerJoins != null && innerJoins.Any())
			{
				foreach (var innerJoin in innerJoins)
				{
					sb.AppendFormat("INNER JOIN `{0}` ", innerJoin.TableName);

					var name1 = QuoteFieldName(innerJoin.FieldName1);
					var name2 = QuoteFieldName(innerJoin.FieldName2);

					sb.AppendFormat("ON {0} = {1} ", name1, name2);
				}
			}

			// WHERE
			this.AppendWheres(sb, query, parameterize);

			// ORDER BY
			if (orderBys != null && orderBys.Any())
			{
				var i = 0;
				var count = orderBys.Count;

				sb.Append("ORDER BY ");

				foreach (var orderBy in orderBys)
				{
					var name = QuoteFieldName(orderBy.FieldName);

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

			// LIMIT
			if (limit != null)
			{
				sb.AppendFormat("LIMIT {0}, {1} ", limit.Start, limit.Count);
			}

			// END
			sb.Append(";");

			return sb.ToString();
		}

		private void AppendWheres(StringBuilder sb, Query query, bool parameterize)
		{
			if (query.WhereElements == null || !query.WhereElements.Any())
				return;

			var i = 0;
			var count = query.WhereElements.Count;

			sb.Append("WHERE ");

			foreach (var where in query.WhereElements)
			{
				var fieldName = QuoteFieldName(where.FieldName);
				sb.AppendFormat("{0} ", fieldName);

				var op = "=";
				switch (where.Comparison)
				{
					case Is.LowerThen: op = "<"; break;
					case Is.LowerEqualThen: op = "<="; break;
					case Is.GreaterThen: op = ">"; break;
					case Is.GreaterEqualThen: op = ">="; break;
					case Is.Equal: op = "="; break;
					case Is.NotEqual: op = "!="; break;
					case Is.Like: op = "LIKE"; break;
					case Is.NotLike: op = "NOT LIKE"; break;
				}

				sb.AppendFormat("{0} ", op);

				var value = where.Value;

				if (parameterize)
				{
					var parameterName = "@p" + _parameterCount;
					_parameters[parameterName] = value;
					_parameterCount++;

					sb.AppendFormat("{0} ", parameterName);
				}
				else if ((value is string) || !(value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal))
				{
					sb.AppendFormat("\"{0}\" ", where.Value);
				}
				else
				{
					sb.AppendFormat("{0} ", where.Value);
				}

				if (++i < count)
					sb.Append("AND ");
			}
		}

		private static string QuoteFieldName(string fieldName)
		{
			if (fieldName == "*")
				return fieldName;

			var index = fieldName.IndexOf('.');
			if (index == -1)
				return string.Format("`{0}`", fieldName);

			var tableName = fieldName.Substring(0, index);
			fieldName = fieldName.Remove(0, index + 1);

			return string.Format("`{0}`.`{1}`", tableName, fieldName);
		}
	}
}
