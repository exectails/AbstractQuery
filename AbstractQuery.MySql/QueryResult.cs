using MySql.Data.MySqlClient;
using System;

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

		public override bool GetBoolean(string fieldName)
		{
			return _reader.GetBoolean(fieldName);
		}

		public override sbyte GetSByte(string fieldName)
		{
			return _reader.GetSByte(fieldName);
		}

		public override byte GetByte(string fieldName)
		{
			return _reader.GetByte(fieldName);
		}

		public override short GetInt16(string fieldName)
		{
			return _reader.GetInt16(fieldName);
		}

		public override ushort GetUInt16(string fieldName)
		{
			return _reader.GetUInt16(fieldName);
		}

		public override int GetInt32(string fieldName)
		{
			return _reader.GetInt32(fieldName);
		}

		public override uint GetUInt32(string fieldName)
		{
			return _reader.GetUInt32(fieldName);
		}

		public override long GetInt64(string fieldName)
		{
			return _reader.GetInt64(fieldName);
		}

		public override ulong GetUInt64(string fieldName)
		{
			return _reader.GetUInt64(fieldName);
		}

		public override float GetFloat(string fieldName)
		{
			return _reader.GetFloat(fieldName);
		}

		public override double GetDouble(string fieldName)
		{
			return _reader.GetDouble(fieldName);
		}

		public override string GetString(string fieldName)
		{
			return _reader.GetString(fieldName);
		}

		public override DateTime GetDateTime(string fieldName)
		{
			return _reader.GetDateTime(fieldName);
		}
	}
}
