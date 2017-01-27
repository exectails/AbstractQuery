using AbstractQuery.MySql;
using AbstractQuery.SQLite;
using System;
using Xunit;

namespace AbstractQuery.Tests.MySql
{
	public class SQLiteQueryBuilderTests
	{
		[Fact]
		public void CreateTable()
		{
			var query = Query
					.CreateTable("foobar")
					.Field<int>("foobarId", FieldOptions.NotNull | FieldOptions.PrimaryKey | FieldOptions.AutoIncrement)
					.Field<string>("name", 100, FieldOptions.NotNull)
					.Field<string>("info");

			var builder = new SQLiteQueryBuilder();
			var queryString = builder.GetQueryString(query);

			Assert.Equal("CREATE TABLE `foobar` (`foobarId` integer NOT NULL PRIMARY KEY AUTOINCREMENT, `name` text NOT NULL, `info` text) ;", queryString);
		}

		[Fact]
		public void CreateTablePrimaryKey()
		{
			var query = Query
					.CreateTable("foobar")
					.Field<int>("foobarId", FieldOptions.PrimaryKey);
			var builder = new SQLiteQueryBuilder();
			var queryString = builder.GetQueryString(query);

			Assert.Equal("CREATE TABLE `foobar` (`foobarId` integer PRIMARY KEY) ;", queryString);

			query = Query
				   .CreateTable("foobar")
				   .Field<int>("foobarId1", FieldOptions.PrimaryKey)
				   .Field<int>("foobarId2", FieldOptions.PrimaryKey);
			builder = new SQLiteQueryBuilder();
			queryString = builder.GetQueryString(query);

			Assert.Equal("CREATE TABLE `foobar` (`foobarId1` integer, `foobarId2` integer, PRIMARY KEY (`foobarId1`, `foobarId2`)) ;", queryString);
		}

		[Fact]
		public void CreateTableTypes()
		{
			var i = 1;

			var query = Query
					.CreateTable("foobar")
					.Field<bool>("test" + i++)
					.Field<sbyte>("test" + i++)
					.Field<byte>("test" + i++)
					.Field<short>("test" + i++)
					.Field<ushort>("test" + i++)
					.Field<int>("test" + i++)
					.Field<uint>("test" + i++)
					.Field<long>("test" + i++)
					.Field<ulong>("test" + i++)
					.Field<float>("test" + i++)
					.Field<double>("test" + i++)
					.Field<string>("test" + i++)
					.Field<string>("test" + i++, 100)
					.Field<DateTime>("test" + i++)
					;

			var builder = new SQLiteQueryBuilder();
			var queryString = builder.GetQueryString(query);

			Assert.Equal("CREATE TABLE `foobar` (`test1` integer, `test2` integer, `test3` integer, `test4` integer, `test5` integer, `test6` integer, `test7` integer, `test8` integer, `test9` integer, `test10` real, `test11` real, `test12` text, `test13` text, `test14` text) ;", queryString);
		}
	}
}
