using System.Data;

namespace AbstractQuery
{
	public interface IDatabase
	{
		IDbConnection GetConnection();
		QueryResult Execute(Query query, IDbConnection connection, IDbTransaction transaction = null);
	}
}
