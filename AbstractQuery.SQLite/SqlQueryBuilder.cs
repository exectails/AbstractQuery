using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractQuery.SQLite
{
	public class SQLiteQueryBuilder : SqlQueryBuilder
	{
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
