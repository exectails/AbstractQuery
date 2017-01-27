using System;
using System.Collections.Generic;

namespace AbstractQuery
{
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

		public static Query Select(params string[] fieldNames)
		{
			var query = new Query();

			query.SelectElement = new SelectElement(fieldNames);

			return query;
		}

		public static Query InsertInto(string tableName)
		{
			var query = new Query();

			query.InsertIntoElement = new TableNameElement(tableName);

			return query;
		}

		public static Query Update(string tableName)
		{
			var query = new Query();

			query.UpdateElement = new TableNameElement(tableName);

			return query;
		}

		public static Query Delete()
		{
			var query = new Query();

			query.DeleteElement = new DeleteElement();

			return query;
		}

		public static Query CreateTable(string tableName)
		{
			return CreateTable(tableName, false);
		}

		public static Query CreateTable(string tableName, bool checkExistence)
		{
			var query = new Query();

			query.CreateTableElement = new CreateTableElement(tableName, checkExistence);

			return query;
		}

		public Query Field<T>(string name)
		{
			return this.Field<T>(name, -1, false, default(T), FieldOptions.None);
		}

		public Query Field<T>(string name, int length)
		{
			return this.Field<T>(name, length, false, default(T), FieldOptions.None);
		}

		public Query Field<T>(string name, FieldOptions options)
		{
			return this.Field<T>(name, -1, false, default(T), options);
		}

		public Query Field<T>(string name, T def, FieldOptions options)
		{
			return this.Field<T>(name, -1, true, def, options);
		}

		public Query Field<T>(string name, int length, FieldOptions options)
		{
			return this.Field<T>(name, length, false, default(T), options);
		}

		public Query Field<T>(string name, int length, bool hasDefault, T def, FieldOptions options)
		{
			var element = new FieldDefinitionElement(name, typeof(T), length, hasDefault, def, options);

			if (this.FieldDefinitionElements == null)
				this.FieldDefinitionElements = new List<FieldDefinitionElement>();

			this.FieldDefinitionElements.Add(element);

			return this;
		}

		public static Query DropTable(string tableName)
		{
			var query = new Query();

			query.DropTableElement = new TableNameElement(tableName);

			return query;
		}

		public Query From(string tableName, string shortName = null)
		{
			var element = new FromElement(tableName, shortName);

			if (this.FromElements == null)
				this.FromElements = new List<FromElement>();

			this.FromElements.Add(element);

			return this;
		}

		public Query Where(string fieldName, Is comparision, object value)
		{
			var element = new WhereElement { FieldName = fieldName, Value = value, Comparison = comparision };

			if (this.WhereElements == null)
				this.WhereElements = new List<WhereElement>();

			this.WhereElements.Add(element);

			return this;
		}

		public Query OrderBy(string fieldName, OrderDirection direction = OrderDirection.Ascending)
		{
			var element = new OrderByElement(fieldName, direction);

			if (this.OrderByElements == null)
				this.OrderByElements = new List<OrderByElement>();

			this.OrderByElements.Add(element);

			return this;
		}

		public Query InnerJoin(string tableName, string fieldName1, string fieldName2)
		{
			var element = new InnerJoinElement(tableName, fieldName1, fieldName2);

			if (this.InnerJoinElements == null)
				this.InnerJoinElements = new List<InnerJoinElement>();

			this.InnerJoinElements.Add(element);

			return this;
		}

		public Query Limit(int count)
		{
			return this.Limit(0, count);
		}

		public Query Limit(int start, int count)
		{
			this.LimitElement = new LimitElement(start, count);

			return this;
		}

		public Query Value(string fieldName, object value)
		{
			var element = new FieldValueElement(fieldName, value);

			if (this.FieldValueElements == null)
				this.FieldValueElements = new List<FieldValueElement>();

			this.FieldValueElements.Add(element);

			return this;
		}

		public Query Set(string fieldName, object value)
		{
			return this.Value(fieldName, value);
		}
	}

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

	public class WhereElement
	{
		public string FieldName { get; set; }
		public object Value { get; set; }
		public Is Comparison { get; set; }
	}

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

	public class TableNameElement
	{
		public string TableName { get; set; }

		public TableNameElement(string tableName)
		{
			this.TableName = tableName;
		}
	}

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

	public class DeleteElement
	{
		public DeleteElement()
		{
		}
	}

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

	public enum OrderDirection
	{
		Ascending,
		Descending,
	}

	[Flags]
	public enum FieldOptions
	{
		None = 0x00,

		NotNull = 0x01,
		AutoIncrement = 0x02,
		PrimaryKey = 0x04,
	}
}
