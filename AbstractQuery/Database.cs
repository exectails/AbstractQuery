using System.Data;

namespace AbstractQuery
{
	/// <summary>
	/// Represents a database.
	/// </summary>
	public interface IDatabase
	{
		/// <summary>
		/// Returns a connection that can be used to execute queries
		/// on the database.
		/// </summary>
		/// <remarks>
		/// The connection needs to be disposed from the caller.
		/// </remarks>
		/// <returns></returns>
		IDbConnection GetConnection();

		/// <summary>
		/// Executes query and returns an object with the means to read the
		/// information returned.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="connection"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		QueryDataReader ExecuteReader(Query query, IDbConnection connection, IDbTransaction transaction = null);

		/// <summary>
		/// Executes query and returns the number of affected datasets.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="connection"></param>
		/// <param name="transaction"></param>
		/// <returns></returns>
		int Execute(Query query, IDbConnection connection, IDbTransaction transaction = null);
	}
}
