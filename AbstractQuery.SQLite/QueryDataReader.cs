using System;
using System.Data.SQLite;

namespace AbstractQuery.SQLite
{
	public class SQLiteQueryDataReader : QueryDataReader
	{
		private SQLiteDataReader _reader;

		public override bool Any
		{
			get { return _reader.HasRows; }
		}

		public SQLiteQueryDataReader(SQLiteDataReader reader)
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
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetBoolean(index);
		}

		public override sbyte GetSByte(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return (sbyte)_reader.GetByte(index);
		}

		public override byte GetByte(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetByte(index);
		}

		public override short GetInt16(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetInt16(index);
		}

		public override ushort GetUInt16(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return (ushort)_reader.GetInt16(index);
		}

		public override int GetInt32(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetInt32(index);
		}

		public override uint GetUInt32(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return (uint)_reader.GetInt32(index);
		}

		public override long GetInt64(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetInt64(index);
		}

		public override ulong GetUInt64(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return (ulong)_reader.GetInt64(index);
		}

		public override float GetFloat(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetFloat(index);
		}

		public override double GetDouble(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetDouble(index);
		}

		public override string GetString(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetString(index);
		}

		public override DateTime GetDateTime(string fieldName)
		{
			var index = _reader.GetOrdinal(fieldName);
			return _reader.GetDateTime(index);
		}
	}
}
