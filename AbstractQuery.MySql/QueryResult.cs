using MySql.Data.MySqlClient;

namespace AbstractQuery.MySql
{
	public class MySqlQueryResult : QueryResult
	{
		private MySqlDataReader _reader;

		public override bool Any
		{
			get { return _reader.HasRows; }
		}

		public MySqlQueryResult(MySqlDataReader reader)
		{
			_reader = reader;
		}

		public override void Dispose()
		{
			_reader.Dispose();
		}

		public override bool Read()
		{
			return _reader.Read();
		}

		public override int GetInt32(string fieldName)
		{
			return _reader.GetInt32(fieldName);
		}

		public override string GetString(string fieldName)
		{
			return _reader.GetString(fieldName);
		}
	}
}
