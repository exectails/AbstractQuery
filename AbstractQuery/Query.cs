using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractQuery
{
	/// <summary>
	/// Wrapper around potential query elements.
	/// </summary>
	public class Query
	{
		public SelectElement SelectElement { get; set; }
		public List<FromElement> FromElements { get; set; }
		public List<WhereElement> WhereElements { get; set; }
		public List<OrderByElement> OrderByElements { get; set; }
		public List<InnerJoinElement> InnerJoinElements { get; set; }
		public LimitElement LimitElement { get; set; }

		public TableNameElement InsertIntoElement { get; set; }
		public List<FieldValueElement> FieldValueElements { get; set; }

		public DeleteElement DeleteElement { get; set; }

		public TableNameElement UpdateElement { get; set; }
		public TableNameElement DropTableElement { get; set; }

		public CreateTableElement CreateTableElement { get; set; }
		public List<FieldDefinitionElement> FieldDefinitionElements { get; set; }

		protected Query()
		{
		}

		/// <summary>
		/// Creates new Query and adds a Select element.
		/// </summary>
		/// <param name="fieldNames"></param>
		/// <returns></returns>
		public static Query Select(params string[] fieldNames)
		{
			var query = new Query();

			query.SelectElement = new SelectElement(fieldNames);

			return query;
		}

		/// <summary>
		/// Creates new Query and adds an InsertInto element.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static Query InsertInto(string tableName)
		{
			var query = new Query();

			query.InsertIntoElement = new TableNameElement(tableName);

			return query;
		}

		/// <summary>
		/// Creates new Query and adds Update element.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static Query Update(string tableName)
		{
			var query = new Query();

			query.UpdateElement = new TableNameElement(tableName);

			return query;
		}

		/// <summary>
		/// Creates new Query and adds Delete element.
		/// </summary>
		/// <returns></returns>
		public static Query Delete()
		{
			var query = new Query();

			query.DeleteElement = new DeleteElement();

			return query;
		}

		/// <summary>
		/// Creates new Query and adds CreateTable element.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static Query CreateTable(string tableName)
		{
			return CreateTable(tableName, false);
		}

		/// <summary>
		/// Creates new Query and adds CreateTable element.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="checkExistence"></param>
		/// <returns></returns>
		public static Query CreateTable(string tableName, bool checkExistence)
		{
			var query = new Query();

			query.CreateTableElement = new CreateTableElement(tableName, checkExistence);

			return query;
		}

		/// <summary>
		/// Adds Field element to Query.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public Query Field<T>(string name)
		{
			return this.Field<T>(name, -1, false, default(T), FieldOptions.None);
		}

		/// <summary>
		/// Adds Field element to Query.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public Query Field<T>(string name, int length)
		{
			return this.Field<T>(name, length, false, default(T), FieldOptions.None);
		}

		/// <summary>
		/// Adds Field element to Query.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Query Field<T>(string name, FieldOptions options)
		{
			return this.Field<T>(name, -1, false, default(T), options);
		}

		/// <summary>
		/// Adds Field element to Query.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="def"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Query Field<T>(string name, T def, FieldOptions options)
		{
			return this.Field<T>(name, -1, true, def, options);
		}

		/// <summary>
		/// Adds Field element to Query.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="length"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Query Field<T>(string name, int length, FieldOptions options)
		{
			return this.Field<T>(name, length, false, default(T), options);
		}

		/// <summary>
		/// Adds Field element to Query.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="length"></param>
		/// <param name="hasDefault"></param>
		/// <param name="def"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public Query Field<T>(string name, int length, bool hasDefault, T def, FieldOptions options)
		{
			var element = new FieldDefinitionElement(name, typeof(T), length, hasDefault, def, options);

			if (this.FieldDefinitionElements == null)
				this.FieldDefinitionElements = new List<FieldDefinitionElement>();

			if ((element.Options & FieldOptions.AutoIncrement) != 0 && (element.Options & FieldOptions.PrimaryKey) == 0)
				throw new InvalidOperationException("Only the PrimaryKey can have the AutoIncrement option.");

			this.FieldDefinitionElements.Add(element);

			var primaryFieldsCount = this.FieldDefinitionElements.Count(a => (a.Options & FieldOptions.PrimaryKey) != 0);
			var autoIncrementFieldsCount = this.FieldDefinitionElements.Count(a => (a.Options & FieldOptions.AutoIncrement) != 0);
			if (primaryFieldsCount > 1 && autoIncrementFieldsCount > 0)
				throw new InvalidOperationException("There can only be one PrimaryKey if any field has the AutoIncrement option.");

			return this;
		}

		/// <summary>
		/// Creates new Query and adds DropTable element.
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public static Query DropTable(string tableName)
		{
			var query = new Query();

			query.DropTableElement = new TableNameElement(tableName);

			return query;
		}

		/// <summary>
		/// Adds From element to Query.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="shortName"></param>
		/// <returns></returns>
		public Query From(string tableName, string shortName = null)
		{
			var element = new FromElement(tableName, shortName);

			if (this.FromElements == null)
				this.FromElements = new List<FromElement>();

			this.FromElements.Add(element);

			return this;
		}

		/// <summary>
		/// Adds Where element to Query.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="comparision"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Query Where(string fieldName, Is comparision, object value)
		{
			var element = new WhereElement { FieldName = fieldName, Value = value, Comparison = comparision };

			if (this.WhereElements == null)
				this.WhereElements = new List<WhereElement>();

			this.WhereElements.Add(element);

			return this;
		}

		/// <summary>
		/// Adds OrderBy element to Query.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public Query OrderBy(string fieldName, OrderDirection direction = OrderDirection.Ascending)
		{
			var element = new OrderByElement(fieldName, direction);

			if (this.OrderByElements == null)
				this.OrderByElements = new List<OrderByElement>();

			this.OrderByElements.Add(element);

			return this;
		}

		/// <summary>
		/// Adds InnerJoin element to Query.
		/// </summary>
		/// <param name="tableName"></param>
		/// <param name="fieldName1"></param>
		/// <param name="fieldName2"></param>
		/// <returns></returns>
		public Query InnerJoin(string tableName, string fieldName1, string fieldName2)
		{
			var element = new InnerJoinElement(tableName, fieldName1, fieldName2);

			if (this.InnerJoinElements == null)
				this.InnerJoinElements = new List<InnerJoinElement>();

			this.InnerJoinElements.Add(element);

			return this;
		}

		/// <summary>
		/// Adds Limit element to Query.
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		public Query Limit(int count)
		{
			return this.Limit(0, count);
		}

		/// <summary>
		/// Adds Limit element to Query.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public Query Limit(int start, int count)
		{
			this.LimitElement = new LimitElement(start, count);

			return this;
		}

		/// <summary>
		/// Adds Value element to Query, for inserts.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Query Value(string fieldName, object value)
		{
			var element = new FieldValueElement(fieldName, value);

			if (this.FieldValueElements == null)
				this.FieldValueElements = new List<FieldValueElement>();

			this.FieldValueElements.Add(element);

			return this;
		}

		/// <summary>
		/// Adds Set/Value element to Query, for updates.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public Query Set(string fieldName, object value)
		{
			return this.Value(fieldName, value);
		}
	}

	/// <summary>
	/// Holds information about a Select.
	/// </summary>
	public class SelectElement
	{
		public List<string> FieldNames { get; set; }

		public SelectElement(params string[] fieldNames)
		{
			this.FieldNames = new List<string>();
			foreach (var fieldName in fieldNames)
			{
				var name = fieldName.Trim();
				this.FieldNames.Add(name);
			}
		}
	}

	/// <summary>
	/// Holds information about where to select from.
	/// </summary>
	public class FromElement
	{
		public string TableName { get; set; }
		public string ShortName { get; set; }

		public FromElement(string tableName, string shortName)
		{
			this.TableName = tableName;
			this.ShortName = shortName;
		}
	}

	/// <summary>
	/// Holds information about conditions.
	/// </summary>
	public class WhereElement
	{
		public string FieldName { get; set; }
		public object Value { get; set; }
		public Is Comparison { get; set; }
	}

	/// <summary>
	/// Holds information about how to sort a result.
	/// </summary>
	public class OrderByElement
	{
		public string FieldName { get; set; }
		public OrderDirection Direction { get; set; }

		public OrderByElement(string fieldName, OrderDirection direction)
		{
			this.FieldName = fieldName;
			this.Direction = direction;
		}
	}

	/// <summary>
	/// Holds information about table joins.
	/// </summary>
	public class InnerJoinElement
	{
		public string TableName { get; set; }
		public string FieldName1 { get; set; }
		public string FieldName2 { get; set; }

		public InnerJoinElement(string tableName, string fieldName1, string fieldName2)
		{
			this.TableName = tableName;
			this.FieldName1 = fieldName1;
			this.FieldName2 = fieldName2;
		}
	}

	/// <summary>
	/// Holds information about how many datasets to query.
	/// </summary>
	public class LimitElement
	{
		public int Start { get; set; }
		public int Count { get; set; }

		public LimitElement(int start, int count)
		{
			this.Start = start;
			this.Count = count;
		}
	}

	/// <summary>
	/// Holds the name of a table to use in a query.
	/// </summary>
	public class TableNameElement
	{
		public string TableName { get; set; }

		public TableNameElement(string tableName)
		{
			this.TableName = tableName;
		}
	}

	/// <summary>
	/// Holds information about a table creation.
	/// </summary>
	public class CreateTableElement
	{
		public string TableName { get; set; }
		public bool CheckExistence { get; set; }

		public CreateTableElement(string tableName, bool checkExistence)
		{
			this.TableName = tableName;
			this.CheckExistence = checkExistence;
		}
	}

	/// <summary>
	/// Holds the name and the (new) value of a field.
	/// </summary>
	public class FieldValueElement
	{
		public string FieldName { get; set; }
		public object Value { get; set; }

		public FieldValueElement(string fieldName, object value)
		{
			this.FieldName = fieldName;
			this.Value = value;
		}
	}

	/// <summary>
	/// Dummy element.
	/// </summary>
	public class DeleteElement
	{
		public DeleteElement()
		{
		}
	}

	/// <summary>
	/// Holds information about a field that is to be created.
	/// </summary>
	public class FieldDefinitionElement
	{
		public string Name { get; private set; }
		public Type Type { get; private set; }
		public int Length { get; private set; }
		public bool HasDefault { get; private set; }
		public object Default { get; private set; }
		public FieldOptions Options { get; private set; }

		public FieldDefinitionElement(string name, Type type, int length, bool hasDefault, object def, FieldOptions options)
		{
			this.Name = name;
			this.Type = type;
			this.Length = length;
			this.Options = options;
			this.HasDefault = hasDefault;
			this.Default = def;
		}
	}

	/// <summary>
	/// Operators used in Where.
	/// </summary>
	public enum Is
	{
		LowerThen,
		LowerEqualThen,
		GreaterThen,
		GreaterEqualThen,
		Equal,
		NotEqual,
		Like,
		NotLike,
		Is,
		IsNot,
	}

	/// <summary>
	/// Direction to order results in.
	/// </summary>
	public enum OrderDirection
	{
		Ascending,
		Descending,
	}

	/// <summary>
	/// Options for fields that are to be created.
	/// </summary>
	[Flags]
	public enum FieldOptions
	{
		None = 0x00,

		NotNull = 0x01,
		AutoIncrement = 0x02,
		PrimaryKey = 0x04,
	}
}
