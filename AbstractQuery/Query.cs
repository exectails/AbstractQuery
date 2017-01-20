﻿using System.Collections.Generic;

namespace AbstractQuery
{
	public class Query
	{
		public SelectElement SelectElement { get; set; }
		public FromElement FromElement { get; set; }
		public List<WhereElement> WhereElements { get; set; }
		public List<OrderByElement> OrderByElements { get; set; }

		protected Query()
		{
		}

		public static Query Select(params string[] fieldNames)
		{
			var query = new Query();

			query.SelectElement = new SelectElement(fieldNames);

			return query;
		}

		//public static Query InsertInto(string tableName)
		//{
		//	var query = new Query();

		//	return query;
		//}

		//public static Query Update(string fieldName, object newValue)
		//{
		//	var query = new Query();

		//	return query;
		//}

		//public static Query Delete(string fieldName)
		//{
		//	var query = new Query();

		//	return query;
		//}

		//public static Query Create(string fieldName)
		//{
		//	var query = new Query();

		//	return query;
		//}

		public Query From(string tableName, string shortName = null)
		{
			this.FromElement = new FromElement(tableName, shortName);

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

	public enum Is
	{
		LowerThen,
		LowerEqualThen,
		GreaterThen,
		GreaterEqualThen,
		Equal,
		Like,
	}

	public enum OrderDirection
	{
		Ascending,
		Descending,
	}
}
