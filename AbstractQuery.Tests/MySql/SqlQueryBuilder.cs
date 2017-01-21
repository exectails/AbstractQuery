using AbstractQuery.MySql;
using System.Linq;
using Xunit;

namespace AbstractQuery.Tests.MySql
{
	public class SqlQueryBuilderTests
	{
		[Fact]
		public void SelectFrom()
		{
			var query = Query.Select("*").From("test");
			var queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` ;", queryString);

			query = Query.Select("testId").From("test");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId` FROM `test` ;", queryString);

			query = Query.Select("testId", "name").From("test");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId`, `name` FROM `test` ;", queryString);

			query = Query.Select("testId").From("test", "t");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId` FROM `test` AS `t` ;", queryString);

			query = Query.Select("t.testId").From("test", "t");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `t`.`testId` FROM `test` AS `t` ;", queryString);

			query = Query.Select("t.testId", "t.name").From("test", "t");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `t`.`testId`, `t`.`name` FROM `test` AS `t` ;", queryString);

			query = Query.Select("t1.testId", "t2.name").From("test1", "t1").From("test2", "t2");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `t1`.`testId`, `t2`.`name` FROM `test1` AS `t1`, `test2` AS `t2` ;", queryString);
		}

		[Fact]
		public void SelectFromWhere()
		{
			var query = Query.Select("testId").From("test", "t").Where("answer", Is.Equal, 42);
			var queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId` FROM `test` AS `t` WHERE `answer` = 42 ;", queryString);

			query = Query.Select("testId").From("test", "t").Where("answer", Is.Equal, 42).Where("name", Is.Like, "test%");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId` FROM `test` AS `t` WHERE `answer` = 42 AND `name` LIKE \"test%\" ;", queryString);


			query = Query.Select("*").From("test").Where("answer", Is.LowerThen, 42);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` < 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.LowerEqualThen, 42);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` <= 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.GreaterThen, 42);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` > 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.GreaterEqualThen, 42);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` >= 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.Equal, 42);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` = 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.NotEqual, 42);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` != 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.Like, "42");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` LIKE \"42\" ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.NotLike, "42");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` NOT LIKE \"42\" ;", queryString);
		}

		[Fact]
		public void SelectFromWhereOrderBy()
		{
			var query = Query.Select("*").From("test").Where("answer", Is.Equal, 42).OrderBy("testId", OrderDirection.Ascending);
			var queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` = 42 ORDER BY `testId` ASC ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.Equal, 42).OrderBy("testId", OrderDirection.Descending);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` = 42 ORDER BY `testId` DESC ;", queryString);

			query = Query.Select("*").From("test").OrderBy("testId");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` ORDER BY `testId` ASC ;", queryString);
		}

		[Fact]
		public void SelectInnerJoin()
		{
			var query = Query.Select("*").From("test1").InnerJoin("test2", "test1.testId", "test2.testId");
			var queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test1` INNER JOIN `test2` ON `test1`.`testId` = `test2`.`testId` ;", queryString);

			query = Query.Select("*").From("test1").InnerJoin("test2", "test1.testId", "test2.testId").OrderBy("testId");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test1` INNER JOIN `test2` ON `test1`.`testId` = `test2`.`testId` ORDER BY `testId` ASC ;", queryString);

			query = Query.Select("*").From("test1").InnerJoin("test2", "test1.testId", "test2.testId").InnerJoin("test3", "test1.testId", "test3.testId");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test1` INNER JOIN `test2` ON `test1`.`testId` = `test2`.`testId` INNER JOIN `test3` ON `test1`.`testId` = `test3`.`testId` ;", queryString);
		}

		[Fact]
		public void SelectLimit()
		{
			var query = Query.Select("*").From("test").Limit(10);
			var queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` LIMIT 0, 10 ;", queryString);

			query = Query.Select("*").From("test").Limit(100, 10);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` LIMIT 100, 10 ;", queryString);

			query = Query.Select("*").From("test").OrderBy("testId").Limit(10);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` ORDER BY `testId` ASC LIMIT 0, 10 ;", queryString);
		}

		[Fact]
		public void SelectWhereParameters()
		{
			var query = Query.Select("*").From("test").Where("name", Is.Equal, "admin");
			var builder = new SqlQueryBuilder();
			var queryString = builder.GetQueryString(query, true);
			var parameters = builder.GetParameters();

			Assert.Equal("SELECT * FROM `test` WHERE `name` = @p0 ;", queryString);
			Assert.Equal(1, parameters.Count);
			Assert.Equal("@p0", parameters.Keys.First());
			Assert.Equal("admin", parameters.Values.First());

			query = Query.Select("*").From("test").Where("testId", Is.GreaterEqualThen, 10).Where("testId", Is.LowerEqualThen, 20);
			builder = new SqlQueryBuilder();
			queryString = builder.GetQueryString(query, true);
			parameters = builder.GetParameters();

			Assert.Equal("SELECT * FROM `test` WHERE `testId` >= @p0 AND `testId` <= @p1 ;", queryString);
			Assert.Equal(2, parameters.Count);
			Assert.Equal("@p0", parameters.Keys.ElementAt(0));
			Assert.Equal(10, parameters.Values.ElementAt(0));
			Assert.Equal("@p1", parameters.Keys.ElementAt(1));
			Assert.Equal(20, parameters.Values.ElementAt(1));
		}
	}
}
