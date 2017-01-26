using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQuery.MySql
{
	public class MySqlQueryBuilder : SqlQueryBuilder
	{
		protected override string GetTypeString(Type type, int length)
		{
			string result;
			var unsigned = false;

			if (type == typeof(bool))
			{
				result = "tinyint";
				length = 1;
			}
			else if (type == typeof(sbyte))
			{
				result = "tinyint";
			}
			else if (type == typeof(byte))
			{
				result = "tinyint";
				unsigned = true;
			}
			else if (type == typeof(short))
			{
				result = "smallint";
			}
			else if (type == typeof(ushort))
			{
				result = "smallint";
				unsigned = true;
			}
			else if (type == typeof(int))
			{
				result = "int";
			}
			else if (type == typeof(uint))
			{
				result = "int";
				unsigned = true;
			}
			else if (type == typeof(long))
			{
				result = "bigint";
			}
			else if (type == typeof(ulong))
			{
				result = "bigint";
				unsigned = true;
			}
			else if (type == typeof(float))
			{
				result = "float";
			}
			else if (type == typeof(double))
			{
				result = "double";
			}
			else if (type == typeof(string))
			{
				if (length == -1)
					result = "text";
				else
					result = "varchar";
			}
			else if (type == typeof(DateTime))
			{
				result = "datetime";
			}
			else
			{
				throw new ArgumentException("Unsupported type '" + type + "'.");
			}

			if (length > 0)
				result += "(" + length + ")";

			if (unsigned)
				result += " unsigned";

			return result;
		}

		protected override string GetAutoIncrementString()
		{
			return "AUTO_INCREMENT";
		}
	}
}
