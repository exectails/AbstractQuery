using System.Data;

namespace AbstractQuery
{
	public interface IDatabase
	{
		IDbConnection GetConnection();

		QueryResult ExecuteReader(Query query, IDbConnection connection, IDbTransaction transaction = null);
		int Execute(Query query, IDbConnection connection, IDbTransaction transaction = null);
	}
}
