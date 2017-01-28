using System;

namespace AbstractQuery
{
	/// <summary>
	/// Query result reader.
	/// </summary>
	public abstract class QueryResult : IDisposable
	{
		/// <summary>
		/// Returns whether there are any results to read.
		/// </summary>
		public abstract bool Any { get; }

		/// <summary>
		/// Disposes resource.
		/// </summary>
		public virtual void Dispose()
		{
		}

		/// <summary>
		/// Reads the next dataset and prepares its values to be read.
		/// </summary>
		/// <returns></returns>
		public abstract bool Read();

		/// <summary>
		/// Returns the given field's value as a bool.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract bool GetBoolean(string fieldName);

		/// <summary>
		/// Returns the given field's value as a signed byte.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract sbyte GetSByte(string fieldName);

		/// <summary>
		/// Returns the given field's value as a byte.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract byte GetByte(string fieldName);

		/// <summary>
		/// Returns the given field's value as a short int.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract short GetInt16(string fieldName);

		/// <summary>
		/// Returns the given field's value as a short unsigned int.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract ushort GetUInt16(string fieldName);

		/// <summary>
		/// Returns the given field's value as an int.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract int GetInt32(string fieldName);

		/// <summary>
		/// Returns the given field's value as an unsigned int.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract uint GetUInt32(string fieldName);

		/// <summary>
		/// Returns the given field's value as a long int.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract long GetInt64(string fieldName);

		/// <summary>
		/// Returns the given field's value as a long unsigned int.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract ulong GetUInt64(string fieldName);

		/// <summary>
		/// Returns the given field's value as a float.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract float GetFloat(string fieldName);

		/// <summary>
		/// Returns the given field's value as a double.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract double GetDouble(string fieldName);

		/// <summary>
		/// Returns the given field's value as a string.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract string GetString(string fieldName);

		/// <summary>
		/// Returns the given field's value as a DateTime.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public abstract DateTime GetDateTime(string fieldName);
	}
}
