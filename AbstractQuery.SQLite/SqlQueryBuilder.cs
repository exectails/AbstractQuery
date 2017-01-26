using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQuery.SQLite
{
	public class SQLiteQueryBuilder : SqlQueryBuilder
	{
		protected override string GetCreateTableQueryString(Query query, bool parameterize)
		{
			var createTable = query.CreateTableElement;
			var fieldDefinitions = query.FieldDefinitionElements;
			var keyDefinitions = query.KeyDefinitionElements;
			var sb = new StringBuilder();

			if (fieldDefinitions == null || !fieldDefinitions.Any())
				throw new InvalidOperationException("Expected 'Field' elements in query.");

			// CREATE TABLE
			if (!createTable.CheckExistence)
				sb.AppendFormat("CREATE TABLE `{0}` ", createTable.TableName);
			else
				sb.AppendFormat("CREATE TABLE IF NOT EXISTS `{0}` ", createTable.TableName);

			sb.Append("(");

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

					if (keyDefinitions != null)
					{
						var key = keyDefinitions.FirstOrDefault(a => a.FieldName == field.Name && a.Primary == true);
						if (key != null)
						{
							sb.Append(" PRIMARY KEY");

							if ((field.Options & FieldOptions.AutoIncrement) != 0)
								sb.Append(" " + this.GetAutoIncrementString());
						}
					}

					if (++i < count)
						sb.Append(", ");
				}
			}

			sb.Append(") ;");

			return sb.ToString();
		}

		protected override string GetTypeString(Type type, int length)
		{
			if (
				type == typeof(bool) ||
				type == typeof(sbyte) || type == typeof(byte) || type == typeof(short) || type == typeof(ushort) ||
				type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong)
			)
			{
				return "integer";
			}
			else if (type == typeof(float) || type == typeof(double))
			{
				return "real";
			}
			else if (type == typeof(string) || type == typeof(DateTime))
			{
				return "text";
			}

			throw new ArgumentException("Unsupported type '" + type + "'.");
		}

		protected override string GetAutoIncrementString()
		{
			return "AUTOINCREMENT";
		}
	}
}
