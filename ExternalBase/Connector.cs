using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Configuration;

namespace ExternalBase
{
	static class Connector
	{
		const int PADDING = 16;
		static readonly string CONNECTION_STRING = ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString;
		static SqlConnection connection;
		static Connector()
		{
			Console.WriteLine(CONNECTION_STRING);
			connection = new SqlConnection(CONNECTION_STRING);
		}

		public static void Select(string fields, string tables, string conditions = "")
		{
			string cmd = $"SELECT {fields} FROM {tables}";
			if (conditions != "") cmd += $"WHERE {conditions}";

			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();

			SqlDataReader reader = command.ExecuteReader();
			if (reader.HasRows)
			{
				for (int i = 0; i < reader.FieldCount; i++)
				{
					Console.Write(reader.GetName(i).PadRight(PADDING));
				}
				Console.WriteLine();
				while (reader.Read())
				{
					for (int i = 0; i < reader.FieldCount; i++)
					{
						Console.Write(reader[i].ToString().PadRight(PADDING));
					}
					Console.WriteLine();
				}
			}
			
			connection.Close();
		}

		static ushort GetID(string field, string table, string conditions)
		{
			ushort id = 0;
			string cmd = $"SELECT {field} FROM {table} WHERE {conditions}";

			SqlCommand command = new SqlCommand (cmd, connection);
			connection.Open();

			id = Convert.ToUInt16(command.ExecuteScalar());

			connection.Close();
			return id;
		}
		public static ushort GetDisciplineID(string discipline)
		{
			return GetID("discipline_id", "Disciplines", $"discipline_name LIKE {discipline}");
		}
		public static ushort GetTeacherID(string last_name)
		{
			return GetID("teacher_id", "Teachers", $"last_name LIKE {last_name}");
		}

		public static ushort StudentsCounter(string tables = "", string conditions = "")
		{
			ushort counter = 0;

			string cmd = "SELECT COUNT(stud_id) FROM Students";
			if (tables != "") cmd += $",{tables}";
			if (conditions != "") cmd += $"WHERE {conditions}";

			SqlCommand command = new SqlCommand(cmd, connection);
			connection.Open();

			counter = Convert.ToUInt16(command.ExecuteScalar());

			connection.Close();
			return counter;
		}
	}
}
