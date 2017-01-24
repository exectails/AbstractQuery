using System.Data.SQLite;

namespace AbstractQuery.SQLite
{
	public class SQLiteQueryResult : QueryResult
	{
		private SQLiteDataReader _reader;

		public override bool Any
		{
			get { return _reader.HasRows; }
		}

		public SQLiteQueryResult(SQLiteDataReader reader)
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
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetInt32(index);
		}

		public override string GetString(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetString(index);
		}
	}
}
