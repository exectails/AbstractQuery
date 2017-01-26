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

				Console.WriteLine("Press [Return] to insert new table.");
				Console.ReadLine();

				query = Query
					.CreateTable("foobar")
					.Field<int>("foobarId", FieldOptions.NotNull | FieldOptions.AutoIncrement)
					.Field<string>("name", 100, FieldOptions.NotNull)
					.Field<string>("info")
					.PrimaryKey("foobarId");
				db.Execute(query, conn);

				Console.WriteLine("Press [Return] to insert rows into the new table.");
				Console.ReadLine();

				query = Query.InsertInto("foobar").Value("name", "Bar, Foo").Value("info", "");
				Console.WriteLine("Inserted records: " + db.Execute(query, conn));
				query = Query.InsertInto("foobar").Value("foobarId", 100).Value("name", "Boo, Foo").Value("info", "test");
				Console.WriteLine("Inserted records: " + db.Execute(query, conn));
				query = Query.InsertInto("foobar").Value("name", "Foo, Boo").Value("info", null);
				Console.WriteLine("Inserted records: " + db.Execute(query, conn));

				Console.WriteLine("Press [Return] to delete the new table.");
				Console.ReadLine();

				query = Query.DropTable("foobar");
				db.Execute(query, conn);

				Console.WriteLine("Press [Return] for next db test.");
				Console.ReadLine();
			}

			Console.WriteLine();
		}
	}
}
