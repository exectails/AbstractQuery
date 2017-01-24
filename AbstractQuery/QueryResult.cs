using System;

namespace AbstractQuery
{
	public abstract class QueryResult : IDisposable
	{
		public abstract bool Any { get; }

		public virtual void Dispose()
		{
		}

		public abstract bool GetBoolean(string fieldName);
		public abstract sbyte GetSByte(string fieldName);
		public abstract byte GetByte(string fieldName);
		public abstract short GetInt16(string fieldName);
		public abstract ushort GetUInt16(string fieldName);
		public abstract int GetInt32(string fieldName);
		public abstract uint GetUInt32(string fieldName);
		public abstract long GetInt64(string fieldName);
		public abstract ulong GetUInt64(string fieldName);
		public abstract float GetFloat(string fieldName);
		public abstract double GetDouble(string fieldName);
		public abstract string GetString(string fieldName);
		public abstract DateTime GetDateTime(string fieldName);

		public abstract bool Read();
	}
}
