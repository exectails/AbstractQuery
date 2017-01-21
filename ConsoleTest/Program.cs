using AbstractQuery;
using System;

namespace ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var db = new AbstractQuery.MySql.Database("server=localhost; port=3306; database=abstractquery; uid=abstractquery; password=abstractquery; pooling=true; min pool size=0; max pool size=100; ConvertZeroDateTime=true");

			using (var conn = db.GetConnection())
			{
				var count = 0;

				var query = Query.Select("*").From("accounts").Where("authority", Is.GreaterEqualThen, 0).OrderBy("authority", OrderDirection.Descending);
				using (var reader = db.Execute(query, conn))
				{
					while (reader.Read())
					{
						count++;
						Console.WriteLine(reader.GetString("accountId") + ": " + reader.GetInt32("authority"));
					}
				}

				query = Query.InsertInto("accounts").Value("accountId", "test" + count).Value("authority", 1);
				using (var reader = db.Execute(query, conn))
				{
				}

				Console.WriteLine("Press [Return] to delete the new account.");
				Console.ReadLine();

				query = Query.Delete().From("accounts").Where("accountId", Is.Equal, "test" + count);
				using (var reader = db.Execute(query, conn))
				{
				}
			}

			Console.ReadLine();
		}
	}
}
