using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractQuery
{
	public abstract class SqlQueryBuilder
	{
		private Dictionary<string, object> _parameters = new Dictionary<string, object>();
		private int _parameterCount;

		/// <summary>
		/// Creates new instance.
		/// </summary>
		public SqlQueryBuilder()
		{
		}

		/// <summary>
		/// Returns generated parameters, to be passed on the command.
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, object> GetParameters()
		{
			return _parameters;
		}

		/// <summary>
		/// Generates query string from query.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public string GetQueryString(Query query)
		{
			return this.GetQueryString(query, false);
		}

		/// <summary>
		/// Generates query string from query.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameterize">Generate parameters?</param>
		/// <returns></returns>
		public string GetQueryString(Query query, bool parameterize)
		{
			_parameters.Clear();
			_parameterCount = 0;

			if (query.SelectElement != null)
				return this.GetSelectQueryString(query, parameterize);
			else if (query.InsertIntoElement != null)
				return this.GetInsertIntoQueryString(query, parameterize);
			else if (query.DeleteElement != null)
				return this.GetDeleteQueryString(query, parameterize);
			else if (query.UpdateElement != null)
				return this.GetUpdateQueryString(query, parameterize);
			else if (query.DropTableElement != null)
				return this.GetDropTableQueryString(query, parameterize);
			else if (query.CreateTableElement != null)
				return this.GetCreateTableQueryString(query, parameterize);

			throw new InvalidOperationException("Unknown query type.");
		}

		/// <summary>
		/// Generates SELECT query string.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameterize"></param>
		/// <returns></returns>
		private string GetSelectQueryString(Query query, bool parameterize)
		{
			var sb = new StringBuilder();

			var select = query.SelectElement;
			var froms = query.FromElements;
			var orderBys = query.OrderByElements;
			var innerJoins = query.InnerJoinElements;
			var limit = query.LimitElement;

			if (froms == null || !froms.Any())
				throw new InvalidOperationException("Expected 'From' elements in query.");

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
			this.AppendFroms(sb, froms, parameterize);

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

		/// <summary>
		/// Appends FROM element to string builder.
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="froms"></param>
		/// <param name="parameterize"></param>
		private void AppendFroms(StringBuilder sb, List<FromElement> froms, bool parameterize)
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

		/// <summary>
		/// Generates INSERT INTO query string.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameterize"></param>
		/// <returns></returns>
		private string GetInsertIntoQueryString(Query query, bool parameterize)
		{
			var sb = new StringBuilder();

			var insertInto = query.InsertIntoElement;
			var values = query.FieldValueElements;

			if (values == null || !values.Any())
				throw new InvalidOperationException("Expected values for insert.");

			var fieldNames = string.Join(", ", values.Select(a => QuoteFieldName(a.FieldName)));

			// INSERT INTO
			sb.AppendFormat("INSERT INTO `{0}` ", insertInto.TableName);

			sb.Append("(");
			{
				var i = 0;
				var count = values.Count;

				foreach (var value in values)
				{
					var fieldName = QuoteFieldName(value.FieldName);
					sb.Append(fieldName);

					if (++i != count)
						sb.Append(", ");
				}
			}
			sb.Append(") ");

			// VALUES
			sb.Append("VALUES (");
			{
				var i = 0;
				var count = values.Count;

				foreach (var value in values)
				{
					var fieldValue = this.PrepareValue(value.Value, parameterize);
					sb.Append(fieldValue);

					if (++i != count)
						sb.Append(", ");
				}
			}
			sb.Append(") ;");

			return sb.ToString();
		}

		/// <summary>
		/// Appends WHERE element to string builder.
		/// </summary>
		/// <param name="sb"></param>
		/// <param name="query"></param>
		/// <param name="parameterize"></param>
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
					case Is.Is: op = "IS"; break;
					case Is.IsNot: op = "IS NOT"; break;
				}

				sb.AppendFormat("{0} ", op);

				var value = this.PrepareValue(where.Value, parameterize);
				sb.AppendFormat("{0} ", value);

				if (++i < count)
					sb.Append("AND ");
			}
		}

		/// <summary>
		/// Wraps field name in proper quotes.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		protected static string QuoteFieldName(string fieldName)
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

		/// <summary>
		/// Returns ready to use value, quoted and/or turned into
		/// a parameter.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="parameterize"></param>
		/// <returns></returns>
		private string PrepareValue(object value, bool parameterize)
		{
			string result = null;

			if (parameterize)
			{
				var parameterName = "@p" + _parameterCount;
				_parameters[parameterName] = value;
				_parameterCount++;

				result = parameterName;
			}
			else if (value == null)
			{
				result = "NULL";
			}
			else if (value is bool)
			{
				result = value.ToString().ToUpper();
			}
			else if ((value is string) || !(value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal))
			{
				result = '"' + value.ToString() + '"';
			}
			else
			{
				result = value.ToString();
			}

			return result;
		}

		/// <summary>
		/// Generates DELETE query string.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameterize"></param>
		/// <returns></returns>
		private string GetDeleteQueryString(Query query, bool parameterize)
		{
			var froms = query.FromElements;

			if (froms == null || !froms.Any())
				throw new InvalidOperationException("Expected 'From' elements in query.");

			var sb = new StringBuilder();

			// DELETE
			sb.Append("DELETE ");

			// FROM
			this.AppendFroms(sb, froms, parameterize);

			// WHERE
			this.AppendWheres(sb, query, parameterize);

			sb.Append(";");

			return sb.ToString();
		}

		/// <summary>
		/// Generates UPDATE query string.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameterize"></param>
		/// <returns></returns>
		private string GetUpdateQueryString(Query query, bool parameterize)
		{
			var values = query.FieldValueElements;

			if (values == null || !values.Any())
				throw new InvalidOperationException("Expected 'Set' elements in query.");

			var sb = new StringBuilder();

			// UPDATE
			sb.AppendFormat("UPDATE `{0}` ", query.UpdateElement.TableName);

			// SET
			{
				sb.Append("SET ");

				var i = 0;
				var count = values.Count;

				foreach (var value in values)
				{
					var fieldName = QuoteFieldName(value.FieldName);
					var fieldValue = this.PrepareValue(value.Value, parameterize);

					sb.AppendFormat("{0} = {1}", fieldName, fieldValue);

					if (++i < count)
						sb.Append(", ");
					else
						sb.Append(" ");
				}
			}

			// WHERE
			this.AppendWheres(sb, query, parameterize);

			sb.Append(";");

			return sb.ToString();
		}

		/// <summary>
		/// Generates DROP TABLE query string.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameterize"></param>
		/// <returns></returns>
		private string GetDropTableQueryString(Query query, bool parameterize)
		{
			var dropTable = query.DropTableElement;
			var sb = new StringBuilder();

			// DROP TABLE
			sb.AppendFormat("DROP TABLE `{0}` ", dropTable.TableName);

			sb.Append(";");

			return sb.ToString();
		}

		/// <summary>
		/// Generates CREATE TABLE query string.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="parameterize"></param>
		/// <returns></returns>
		protected virtual string GetCreateTableQueryString(Query query, bool parameterize)
		{
			var createTable = query.CreateTableElement;
			var fieldDefinitions = query.FieldDefinitionElements;
			var sb = new StringBuilder();

			if (fieldDefinitions == null || !fieldDefinitions.Any())
				throw new InvalidOperationException("Expected 'Field' elements in query.");

			// CREATE TABLE
			if (!createTable.CheckExistence)
				sb.AppendFormat("CREATE TABLE `{0}` ", createTable.TableName);
			else
				sb.AppendFormat("CREATE TABLE IF NOT EXISTS `{0}` ", createTable.TableName);

			sb.Append("(");

			var primaryFields = fieldDefinitions.Where(a => (a.Options & FieldOptions.PrimaryKey) != 0);
			var primaryFieldsCount = primaryFields.Count();

			// Fields
			{
				var i = 0;
				var count = fieldDefinitions.Count;

				foreach (var field in fieldDefinitions)
				{
					var fieldName = QuoteFieldName(field.Name);
					var typeName = this.GetTypeString(field.Type, field.Length);

					sb.AppendFormat("{0} {1}", fieldName, typeName);

					if ((field.Options & FieldOptions.NotNull) != 0)
						sb.Append(" NOT NULL");

					if (primaryFieldsCount <= 1 && (field.Options & FieldOptions.PrimaryKey) != 0)
						sb.Append(" PRIMARY KEY");

					if ((field.Options & FieldOptions.AutoIncrement) != 0)
						sb.Append(" " + this.GetAutoIncrementString());

					if (++i < count)
						sb.Append(", ");
				}
			}

			// Keys
			if (primaryFieldsCount > 1)
			{
				sb.AppendFormat(", PRIMARY KEY ({0})", string.Join(", ", primaryFields.Select(a => QuoteFieldName(a.Name))));
			}

			sb.Append(") ;");

			return sb.ToString();
		}

		/// <summary>
		/// Returns type as a valid string for the query.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		protected abstract string GetTypeString(Type type, int length);

		/// <summary>
		/// Returns the string that indidcates auto increment fields in
		/// the query.
		/// </summary>
		/// <returns></returns>
		protected abstract string GetAutoIncrementString();
	}
}
