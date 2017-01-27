using AbstractQuery.MySql;
using System;
using System.Linq;
using Xunit;

namespace AbstractQuery.Tests.MySql
{
	public class MySqlQueryBuilderTests
	{
		[Fact]
		public void SelectFrom()
		{
			var query = Query.Select("*").From("test");
			var queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` ;", queryString);

			query = Query.Select("testId").From("test");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId` FROM `test` ;", queryString);

			query = Query.Select("testId", "name").From("test");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId`, `name` FROM `test` ;", queryString);

			query = Query.Select("testId").From("test", "t");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId` FROM `test` AS `t` ;", queryString);

			query = Query.Select("t.testId").From("test", "t");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `t`.`testId` FROM `test` AS `t` ;", queryString);

			query = Query.Select("t.testId", "t.name").From("test", "t");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `t`.`testId`, `t`.`name` FROM `test` AS `t` ;", queryString);

			query = Query.Select("t1.testId", "t2.name").From("test1", "t1").From("test2", "t2");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `t1`.`testId`, `t2`.`name` FROM `test1` AS `t1`, `test2` AS `t2` ;", queryString);
		}

		[Fact]
		public void SelectExceptions()
		{
			// Missing from
			var query = Query.Select("*");
			var builder = new MySqlQueryBuilder();

			Assert.Throws<InvalidOperationException>(() => builder.GetQueryString(query));
		}

		[Fact]
		public void SelectFromWhere()
		{
			var query = Query.Select("testId").From("test", "t").Where("answer", Is.Equal, 42);
			var queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId` FROM `test` AS `t` WHERE `answer` = 42 ;", queryString);

			query = Query.Select("testId").From("test", "t").Where("answer", Is.Equal, 42).Where("name", Is.Like, "test%");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId` FROM `test` AS `t` WHERE `answer` = 42 AND `name` LIKE \"test%\" ;", queryString);


			query = Query.Select("*").From("test").Where("answer", Is.LowerThen, 42);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` < 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.LowerEqualThen, 42);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` <= 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.GreaterThen, 42);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` > 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.GreaterEqualThen, 42);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` >= 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.Equal, 42);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` = 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.NotEqual, 42);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` != 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.Like, "42");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` LIKE \"42\" ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.NotLike, "42");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` NOT LIKE \"42\" ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.Is, true);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` IS TRUE ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.IsNot, false);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` IS NOT FALSE ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.IsNot, null);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` IS NOT NULL ;", queryString);
		}

		[Fact]
		public void SelectFromWhereOrderBy()
		{
			var query = Query.Select("*").From("test").Where("answer", Is.Equal, 42).OrderBy("testId", OrderDirection.Ascending);
			var queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` = 42 ORDER BY `testId` ASC ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.Equal, 42).OrderBy("testId", OrderDirection.Descending);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` = 42 ORDER BY `testId` DESC ;", queryString);

			query = Query.Select("*").From("test").OrderBy("testId");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` ORDER BY `testId` ASC ;", queryString);
		}

		[Fact]
		public void SelectInnerJoin()
		{
			var query = Query.Select("*").From("test1").InnerJoin("test2", "test1.testId", "test2.testId");
			var queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test1` INNER JOIN `test2` ON `test1`.`testId` = `test2`.`testId` ;", queryString);

			query = Query.Select("*").From("test1").InnerJoin("test2", "test1.testId", "test2.testId").OrderBy("testId");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test1` INNER JOIN `test2` ON `test1`.`testId` = `test2`.`testId` ORDER BY `testId` ASC ;", queryString);

			query = Query.Select("*").From("test1").InnerJoin("test2", "test1.testId", "test2.testId").InnerJoin("test3", "test1.testId", "test3.testId");
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test1` INNER JOIN `test2` ON `test1`.`testId` = `test2`.`testId` INNER JOIN `test3` ON `test1`.`testId` = `test3`.`testId` ;", queryString);
		}

		[Fact]
		public void SelectLimit()
		{
			var query = Query.Select("*").From("test").Limit(10);
			var queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` LIMIT 0, 10 ;", queryString);

			query = Query.Select("*").From("test").Limit(100, 10);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` LIMIT 100, 10 ;", queryString);

			query = Query.Select("*").From("test").OrderBy("testId").Limit(10);
			queryString = new MySqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` ORDER BY `testId` ASC LIMIT 0, 10 ;", queryString);
		}

		[Fact]
		public void SelectWhereParameters()
		{
			var query = Query.Select("*").From("test").Where("name", Is.Equal, "admin");
			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query, true);
			var parameters = builder.GetParameters();

			Assert.Equal("SELECT * FROM `test` WHERE `name` = @p0 ;", queryString);
			Assert.Equal(1, parameters.Count);
			Assert.Equal("@p0", parameters.Keys.First());
			Assert.Equal("admin", parameters.Values.First());

			query = Query.Select("*").From("test").Where("testId", Is.GreaterEqualThen, 10).Where("testId", Is.LowerEqualThen, 20);
			builder = new MySqlQueryBuilder();
			queryString = builder.GetQueryString(query, true);
			parameters = builder.GetParameters();

			Assert.Equal("SELECT * FROM `test` WHERE `testId` >= @p0 AND `testId` <= @p1 ;", queryString);
			Assert.Equal(2, parameters.Count);
			Assert.Equal("@p0", parameters.Keys.ElementAt(0));
			Assert.Equal(10, parameters.Values.ElementAt(0));
			Assert.Equal("@p1", parameters.Keys.ElementAt(1));
			Assert.Equal(20, parameters.Values.ElementAt(1));
		}

		[Fact]
		public void InsertInto()
		{
			var query = Query.InsertInto("accounts").Value("accountId", 1234);
			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query);

			Assert.Equal("INSERT INTO `accounts` (`accountId`) VALUES (1234) ;", queryString);

			query = Query.InsertInto("accounts").Value("accountId", 12345).Value("name", "Foobar");
			builder = new MySqlQueryBuilder();
			queryString = builder.GetQueryString(query);

			Assert.Equal("INSERT INTO `accounts` (`accountId`, `name`) VALUES (12345, \"Foobar\") ;", queryString);
		}

		[Fact]
		public void InsertIntoExceptions()
		{
			// Missing values
			var query = Query.InsertInto("accounts");
			var builder = new MySqlQueryBuilder();

			Assert.Throws<InvalidOperationException>(() => builder.GetQueryString(query));
		}

		[Fact]
		public void InsertIntoParameters()
		{
			var query = Query.InsertInto("accounts").Value("accountId", 1234);
			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query, true);
			var parameters = builder.GetParameters();

			Assert.Equal("INSERT INTO `accounts` (`accountId`) VALUES (@p0) ;", queryString);
			Assert.Equal(1, parameters.Count);
			Assert.Equal("@p0", parameters.Keys.ElementAt(0));
			Assert.Equal(1234, parameters.Values.ElementAt(0));

			query = Query.InsertInto("accounts").Value("accountId", 12345).Value("name", "Foobar");
			builder = new MySqlQueryBuilder();
			queryString = builder.GetQueryString(query, true);
			parameters = builder.GetParameters();

			Assert.Equal("INSERT INTO `accounts` (`accountId`, `name`) VALUES (@p0, @p1) ;", queryString);
			Assert.Equal(2, parameters.Count);
			Assert.Equal("@p0", parameters.Keys.ElementAt(0));
			Assert.Equal(12345, parameters.Values.ElementAt(0));
			Assert.Equal("@p1", parameters.Keys.ElementAt(1));
			Assert.Equal("Foobar", parameters.Values.ElementAt(1));
		}

		[Fact]
		public void Delete()
		{
			var query = Query.Delete().From("accounts").Where("accountId", Is.Equal, 9876);
			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query);

			Assert.Equal("DELETE FROM `accounts` WHERE `accountId` = 9876 ;", queryString);

			query = Query.Delete().From("accounts").From("more_accounts").Where("accountId", Is.Equal, 98765).Where("foo", Is.NotEqual, "bar");
			builder = new MySqlQueryBuilder();
			queryString = builder.GetQueryString(query);

			Assert.Equal("DELETE FROM `accounts`, `more_accounts` WHERE `accountId` = 98765 AND `foo` != \"bar\" ;", queryString);
		}

		[Fact]
		public void DeleteParameters()
		{
			var query = Query.Delete().From("accounts");
			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query, true);
			var parameters = builder.GetParameters();

			Assert.Equal("DELETE FROM `accounts` ;", queryString);
			Assert.Equal(0, parameters.Count);

			query = Query.Delete().From("accounts").Where("accountId", Is.Equal, 9876);
			builder = new MySqlQueryBuilder();
			queryString = builder.GetQueryString(query, true);
			parameters = builder.GetParameters();

			Assert.Equal("DELETE FROM `accounts` WHERE `accountId` = @p0 ;", queryString);
			Assert.Equal(1, parameters.Count);
			Assert.Equal("@p0", parameters.Keys.ElementAt(0));
			Assert.Equal(9876, parameters.Values.ElementAt(0));

			query = Query.Delete().From("accounts").From("more_accounts").Where("accountId", Is.Equal, 98765).Where("foo", Is.NotEqual, "bar");
			builder = new MySqlQueryBuilder();
			queryString = builder.GetQueryString(query, true);
			parameters = builder.GetParameters();

			Assert.Equal("DELETE FROM `accounts`, `more_accounts` WHERE `accountId` = @p0 AND `foo` != @p1 ;", queryString);
			Assert.Equal(2, parameters.Count);
			Assert.Equal("@p0", parameters.Keys.ElementAt(0));
			Assert.Equal(98765, parameters.Values.ElementAt(0));
			Assert.Equal("@p1", parameters.Keys.ElementAt(1));
			Assert.Equal("bar", parameters.Values.ElementAt(1));
		}

		[Fact]
		public void Update()
		{
			var query = Query.Update("accounts").Set("accountId", 1234);
			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query);

			Assert.Equal("UPDATE `accounts` SET `accountId` = 1234 ;", queryString);

			query = Query.Update("accounts").Set("accountId", 12345).Set("name", "Foobar");
			builder = new MySqlQueryBuilder();
			queryString = builder.GetQueryString(query);

			Assert.Equal("UPDATE `accounts` SET `accountId` = 12345, `name` = \"Foobar\" ;", queryString);

			query = Query.Update("accounts").Set("accountId", 12345).Set("name", "Foobar").Where("accountId", Is.NotEqual, 12345);
			builder = new MySqlQueryBuilder();
			queryString = builder.GetQueryString(query);

			Assert.Equal("UPDATE `accounts` SET `accountId` = 12345, `name` = \"Foobar\" WHERE `accountId` != 12345 ;", queryString);
		}

		[Fact]
		public void UpdateExceptions()
		{
			// Missing values
			var query = Query.Update("accounts");
			var builder = new MySqlQueryBuilder();

			Assert.Throws<InvalidOperationException>(() => builder.GetQueryString(query));
		}

		[Fact]
		public void UpdateParameters()
		{
			var query = Query.Update("accounts").Set("accountId", 1234);
			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query, true);
			var parameters = builder.GetParameters();

			Assert.Equal("UPDATE `accounts` SET `accountId` = @p0 ;", queryString);
			Assert.Equal(1, parameters.Count);
			Assert.Equal("@p0", parameters.Keys.ElementAt(0));
			Assert.Equal(1234, parameters.Values.ElementAt(0));

			query = Query.Update("accounts").Set("accountId", 12345).Set("name", "Foobar");
			builder = new MySqlQueryBuilder();
			queryString = builder.GetQueryString(query, true);
			parameters = builder.GetParameters();

			Assert.Equal("UPDATE `accounts` SET `accountId` = @p0, `name` = @p1 ;", queryString);
			Assert.Equal(2, parameters.Count);
			Assert.Equal("@p0", parameters.Keys.ElementAt(0));
			Assert.Equal(12345, parameters.Values.ElementAt(0));
			Assert.Equal("@p1", parameters.Keys.ElementAt(1));
			Assert.Equal("Foobar", parameters.Values.ElementAt(1));
		}

		[Fact]
		public void DropTable()
		{
			var query = Query.DropTable("accounts");
			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query);

			Assert.Equal("DROP TABLE `accounts` ;", queryString);
		}

		[Fact]
		public void CreateTable()
		{
			var query = Query
					.CreateTable("foobar")
					.Field<int>("foobarId", FieldOptions.NotNull | FieldOptions.PrimaryKey | FieldOptions.AutoIncrement)
					.Field<string>("name", 100, FieldOptions.NotNull)
					.Field<string>("info");

			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query);

			Assert.Equal("CREATE TABLE `foobar` (`foobarId` int NOT NULL PRIMARY KEY AUTO_INCREMENT, `name` varchar(100) NOT NULL, `info` text) ;", queryString);
		}

		[Fact]
		public void CreateTablePrimaryKey()
		{
			var query = Query
					.CreateTable("foobar")
					.Field<int>("foobarId", FieldOptions.PrimaryKey);
			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query);

			Assert.Equal("CREATE TABLE `foobar` (`foobarId` int PRIMARY KEY) ;", queryString);

			query = Query
				   .CreateTable("foobar")
				   .Field<int>("foobarId1", FieldOptions.PrimaryKey)
				   .Field<int>("foobarId2", FieldOptions.PrimaryKey);
			builder = new MySqlQueryBuilder();
			queryString = builder.GetQueryString(query);

			Assert.Equal("CREATE TABLE `foobar` (`foobarId1` int, `foobarId2` int, PRIMARY KEY (`foobarId1`, `foobarId2`)) ;", queryString);
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

			var builder = new MySqlQueryBuilder();
			var queryString = builder.GetQueryString(query);

			Assert.Equal("CREATE TABLE `foobar` (`test1` tinyint(1), `test2` tinyint, `test3` tinyint unsigned, `test4` smallint, `test5` smallint unsigned, `test6` int, `test7` int unsigned, `test8` bigint, `test9` bigint unsigned, `test10` float, `test11` double, `test12` text, `test13` varchar(100), `test14` datetime) ;", queryString);
		}
	}
}
