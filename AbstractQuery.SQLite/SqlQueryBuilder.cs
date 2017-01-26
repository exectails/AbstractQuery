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
			throw new NotImplementedException();
		}

		protected override string GetAutoIncrementString()
		{
			throw new NotImplementedException();
		}
	}
}
