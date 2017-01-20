using System;

namespace AbstractQuery
{
	public abstract class QueryResult : IDisposable
	{
		public abstract bool Any { get; }

		public virtual void Dispose()
		{
		}

		public abstract int GetInt32(string fieldName);
		public abstract string GetString(string fieldName);

		public abstract bool Read();
	}
}
