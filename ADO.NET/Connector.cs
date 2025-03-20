using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;

namespace ADO.NET
{
	internal class Connector
	{
		const int PADDING = 30;
		const string CONNECTION_STRING =
			"Data Source=(localdb)\\MSSQLLocalDB;" +
			"Initial Catalog=Movies;" +
			"Integrated Security=True;" +
			"Connect Timeout=30;" +
			"Encrypt=False;" +
			"trustServerCertificate=False;" +
			"ApplicationIntent=ReadWrite;" +
			"MultiSubnetFailover=False";

		SqlConnection connection;
		SqlCommand command;
		SqlDataReader reader;

		public Connector(string cmd) 
		{
			connection = new SqlConnection(CONNECTION_STRING);
			command = new SqlCommand(cmd, connection);
			connection.Open();
			reader = command.ExecuteReader();
		}

		public void GetData()
		{
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
					for (int i = 0; i < reader.FieldCount; i++)
					{
						Console.Write(reader[i].ToString().PadRight(PADDING));

					}
					Console.WriteLine();
				}
			}
		}
	}
}
