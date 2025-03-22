using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace ADO.NET
{
	static class Connector
	{
		const int PADDING = 30;
		const string CONNECTION_STRING =
			"Data Source=(localdb)\\MSSQLLocalDB;" +
			"Initial Catalog=Movies;" +
			"Integrated Security=True;" +
			"Connect Timeout=30;" +
			"Encrypt=False;" +
			"TrustServerCertificate=False;" +
			"ApplicationIntent=ReadWrite;" +
			"MultiSubnetFailover=False";
		static readonly SqlConnection connection;

		static Connector()
		{
			connection = new SqlConnection(CONNECTION_STRING);
			//Статический конструктор нужун только для инициализации статических полей класса.
		}

		public static void SelectDirectors()
		{
			Select("*", "Directors");

		}
		public static void SelectMovies()
		{
			Select("title,release_date,FORMATMESSAGE(N'%s %s',first_name,last_name)", "Movies,Directors", "director=director_id");
		}
		public static void Select(string columns, string tables, string condition = null)
		{ 
			string cmd = $"SELECT {columns} FROM {tables}";
			if (condition != null) cmd += $" WHERE {condition}";
			cmd += ";";

			SqlCommand command = new SqlCommand(cmd, connection);

			connection.Open();
			SqlDataReader reader = command.ExecuteReader();

			if (reader.HasRows)
			{
				Console.WriteLine("====================================================================================================");
				for (int i = 0; i < reader.FieldCount; i++)
					Console.Write(reader.GetName(i).PadRight(PADDING));
                Console.WriteLine();
                Console.WriteLine("====================================================================================================");
				while (reader.Read())
				{
					//Console.WriteLine($"{reader[0].ToString().PadRight(5)}{reader[2].ToString().PadRight(10)}{reader[1].ToString().PadRight(15)}");
					for(int i = 0;i<reader.FieldCount;i++)
					{
						Console.Write(reader[i].ToString().PadRight(PADDING));

                    }
					Console.WriteLine();
				}
			}

			reader.Close();
			connection.Close();
		}
		
		public static void InsertDirector(string first_name, string last_name)
		{
			string cmd = $"INSERT Directors(first_name,last_name) VALUES (N'{first_name}',N'{last_name}')";
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();

			command.ExecuteNonQuery();

			connection.Close();
		}
		public static void InsertMovies(string title, int day, int month, int year, int director_id)
		{
			DateTime release_date = new DateTime(year, day, month, 0, 0, 0, 0);
			string cmd = $"INSERT Movies(title,release_date,director) VALUES (N'{title}',N'{release_date.Date}', {director_id})";
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();

			command.ExecuteNonQuery();

			connection.Close();
		}
		public static void Insert(string table_and_columns, string values)
		{

			string cmd = $"INSERT {table_and_columns} VALUES ({values})";
			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();

			command.ExecuteNonQuery();

			connection.Close();
		}

	}
}
