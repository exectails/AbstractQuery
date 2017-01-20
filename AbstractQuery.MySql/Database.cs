using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace AbstractQuery.MySql
{
	public class Database : IDatabase
	{
		public string ConnectionString { get; private set; }

		public Database(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentException("Connection string can't be null or empty.");

			this.ConnectionString = connectionString;
		}

		public IDbConnection GetConnection()
		{
			var result = new MySqlConnection(this.ConnectionString);
			result.Open();

			return result;
		}

		public QueryResult Execute(Query query, IDbConnection connection, IDbTransaction transaction = null)
		{
			var mySqlConnction = connection as MySqlConnection;
			var mySqlTransaction = transaction as MySqlTransaction;

			if (mySqlConnction == null)
				throw new InvalidOperationException("Connection can't be null.");

			var queryString = new SqlQueryBuilder().GetQueryString(query);
			var mc = new MySqlCommand(queryString, mySqlConnction, mySqlTransaction);

			var reader = mc.ExecuteReader();
			var result = new MySqlQueryResult(reader);

			return result;
		}
	}
}
