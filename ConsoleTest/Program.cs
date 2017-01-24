using AbstractQuery;
using System;

namespace ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var db = new AbstractQuery.MySql.Database("server=localhost; port=3306; database=abstractquery; uid=abstractquery; password=abstractquery; pooling=true; min pool size=0; max pool size=100; ConvertZeroDateTime=true");
			var db2 = new AbstractQuery.SQLite.Database("test.sqlite");

			TestDb(db);
			TestDb(db2);

			Console.ReadLine();
		}

		static void TestDb(IDatabase db)
		{
			Console.WriteLine(db.GetType().FullName);
			Console.WriteLine("".PadLeft(78, '-'));

			using (var conn = db.GetConnection())
			{
				var count = 0;

				var query = Query.Select("*").From("accounts").Where("authority", Is.GreaterEqualThen, 0).OrderBy("authority", OrderDirection.Descending);
				using (var reader = db.ExecuteReader(query, conn))
				{
					while (reader.Read())
					{
						count++;
						Console.WriteLine(reader.GetString("accountId") + ": " + reader.GetInt32("authority"));
					}
				}

				var newId = "test" + count;

				query = Query.InsertInto("accounts").Value("accountId", newId).Value("authority", 1);
				Console.WriteLine("Inserted records: " + db.Execute(query, conn));

				Console.WriteLine("Press [Return] to update the new account.");
				Console.ReadLine();

				query = Query.Update("accounts").Set("authority", 2).Where("accountId", Is.Equal, newId);
				Console.WriteLine("Updated records: " + db.Execute(query, conn));

				Console.WriteLine("Press [Return] to delete the new account.");
				Console.ReadLine();

				query = Query.Delete().From("accounts").Where("accountId", Is.Equal, "test" + count);
				Console.WriteLine("Deleted records: " + db.Execute(query, conn));
			}

			Console.WriteLine();
		}
	}
}
