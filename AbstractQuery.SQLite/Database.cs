using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace AbstractQuery.SQLite
{
	public class Database : IDatabase
	{
		private string _filePath;

		public Database(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException("SQLite database not found at " + filePath);

			_filePath = filePath;
		}

		public static void CreateDatabase(string filePath)
		{
			SQLiteConnection.CreateFile(filePath);
		}

		public IDbConnection GetConnection()
		{
			var connection = new SQLiteConnection("Data Source=" + _filePath + ";Version=3;");
			connection.Open();

			return connection;
		}

		public QueryResult ExecuteReader(Query query, IDbConnection connection, IDbTransaction transaction = null)
		{
			var mc = this.GetCommand(query, connection, transaction);
			var reader = mc.ExecuteReader();
			var result = new SQLiteQueryResult(reader);

			return result;
		}

		public int Execute(Query query, IDbConnection connection, IDbTransaction transaction = null)
		{
			var mc = this.GetCommand(query, connection, transaction);
			var result = mc.ExecuteNonQuery();

			return result;
		}

		private SQLiteCommand GetCommand(Query query, IDbConnection connection, IDbTransaction transaction)
		{
			var sqliteConnction = connection as SQLiteConnection;
			var sqliteTransaction = transaction as SQLiteTransaction;

			if (sqliteConnction == null)
				throw new InvalidOperationException("Connection can't be null.");

			var builder = new SQLiteQueryBuilder();
			var queryString = builder.GetQueryString(query, true);
			var parameters = builder.GetParameters();

			var mc = new SQLiteCommand(queryString, sqliteConnction, sqliteTransaction);
			foreach (var parameter in parameters)
				mc.Parameters.AddWithValue(parameter.Key, parameter.Value);

			return mc;
		}
	}
}
