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
				var query = Query.Select("*").From("accounts").Where("authority", Is.GreaterThen, 0).OrderBy("authority", OrderDirection.Descending);

				using (var reader = db.Execute(query, conn))
				{
					while (reader.Read())
						Console.WriteLine(reader.GetString("accountId") + ": " + reader.GetInt32("authority"));
				}
			}

			Console.ReadLine();
		}
	}
}
