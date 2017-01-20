using AbstractQuery.MySql;
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

			query = Query.Select("*").From("test").Where("answer", Is.Equal, 42);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` = 42 ;", queryString);
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
	}
}
