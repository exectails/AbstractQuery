using AbstractQuery.MySql;
using Xunit;

namespace AbstractQuery.Tests.MySql
{
	public class SqlQueryBuilderTests
	{
		[Fact]
		public void Select()
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

			query = Query.Select("testId").From("test", "t").Where("answer", Is.Equal, 42);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId` FROM `test` AS `t` WHERE `answer` = 42 ;", queryString);

			query = Query.Select("testId").From("test", "t").Where("answer", Is.Equal, 42).Where("name", Is.Like, "test%");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT `testId` FROM `test` AS `t` WHERE `answer` = 42 AND `name` LIKE \"test%\" ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.Equal, 42);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` = 42 ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.Equal, 42).OrderBy("testId", OrderDirection.Ascending);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` = 42 ORDER BY `testId` ASC ;", queryString);

			query = Query.Select("*").From("test").Where("answer", Is.Equal, 42).OrderBy("testId", OrderDirection.Descending);
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` WHERE `answer` = 42 ORDER BY `testId` DESC ;", queryString);

			query = Query.Select("*").From("test").OrderBy("testId");
			queryString = new SqlQueryBuilder().GetQueryString(query);
			Assert.Equal("SELECT * FROM `test` ORDER BY `testId` ASC ;", queryString);
		}
	}
}
