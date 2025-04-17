using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace AcademyDataSet
{
	internal class Cache
	{
		readonly string CONNECTION_STRING = "";
		SqlConnection connection;
		public DataSet Set { get; set; }
		List<string> tables;
		List<string> commands;

		public Cache()
		{
			CONNECTION_STRING = ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString;
			connection = new SqlConnection(CONNECTION_STRING);
			Console.WriteLine(CONNECTION_STRING);

			tables = new List<string>();
			Set = new DataSet(nameof(Set));

			//Check();
		}

		public void AddTable(string table, string columns)
		{
			string[] separated_columns = columns.Split(',');

			Set.Tables.Add(table);

			for (int i = 0; i < separated_columns.Length; i++)
				Set.Tables[table].Columns.Add(separated_columns[i]);

			Set.Tables[table].PrimaryKey =
				new DataColumn[] { Set.Tables[table].Columns[separated_columns[0]] };

			tables.Add($"{table},{columns}");
		}

		public void AddRelation(string name, string child, string parent)
		{
			Set.Relations.Add
				(
				name,
				Set.Tables[parent.Split(',')[0]].Columns[parent.Split(',')[1]],
				Set.Tables[child.Split(',')[0]].Columns[child.Split(',')[1]]
				);
		}

		public void Load()
		{
			string[] tables = this.tables.ToArray();
			for (int i = 0; i < tables.Length; i++)
			{
				string columns = "";
				DataColumnCollection column_collection = Set.Tables[tables[i].Split(',')[0]].Columns;
				foreach (DataColumn column in column_collection)
				{
					columns += $"[{column.ColumnName}],";
				}
				columns = columns.Remove(columns.LastIndexOf(','));
				Console.WriteLine(columns);

				//Console.WriteLine(GroupsRelatedData.Tables[tables[i].Split(',')[0]].Columns.ToString());

				string cmd = $"SELECT {columns} FROM {tables[i].Split(',')[0]}";

				SqlDataAdapter adapter = new SqlDataAdapter(cmd, connection);
				adapter.Fill(Set.Tables[tables[i].Split(',')[0]]);
			}
		}

		public void Print(string table)
		{
			Console.WriteLine("\n------------------------------------\n");
			Console.WriteLine(hasParents(table));

			string relation_name = "No relation";
			string parent_table_name = "";
			string parent_column_name = "";

			int parent_index = -1;

			if (hasParents(table))
			{
				relation_name = Set.Tables[table].ParentRelations[0].RelationName;
				parent_table_name = Set.Tables[table].ParentRelations[0].ParentTable.TableName;
				parent_column_name = parent_table_name.ToLower().Substring(0, parent_table_name.Length - 1) + "_name";
				Console.WriteLine(parent_table_name);
				//DataColumn parent_column = GroupsRelatedData.Tables[parent_table_name].Columns["direction_name"];
				//Console.WriteLine(parent_column.ColumnName);
				parent_index =
					Set.Tables[table].Columns.
					IndexOf(parent_table_name.ToLower().Substring(0, parent_table_name.Length - 1));
				Console.WriteLine(parent_index);
			}

			foreach (DataRow row in Set.Tables[table].Rows)
			{
				for (int i = 0; i < row.ItemArray.Length; i++)
				{
					if (i == parent_index)
					{
						DataRow parent_row = row.GetParentRow(relation_name);
						Console.Write(parent_row[parent_column_name]);
						//Console.Write(row.GetParentRow(relation_name)[parent_column_name]);
						//GroupsRelatedData.Tables;
					}
					else
						Console.Write(row[i].ToString() + "\t");
				}
				Console.WriteLine();
			}
			Console.WriteLine("\n------------------------------------\n");
		}

		bool hasParents(string table)
		{
			return Set.Tables[table].ParentRelations.Count > 0;
			/*for (int i = 0; i < GroupsRelatedData.Relations.Count; i++)
			{
				if (GroupsRelatedData.Relations[i].ChildTable.TableName == table) return true;
			}
			return false;*/
		}

		void Check()
		{
			AddTable("Directions", "direction_id,direction_name");
			AddTable("Groups", "group_id,group_name,direction");
			AddTable("Students", "stud_id,last_name,first_name,middle_name,birth_date,group");

			AddRelation("GroupsDirections", "Groups,direction", "Directions,direction_id");
			AddRelation("StudentsGroups", "Students,group", "Groups,group_id");

			Load();

			Print("Directions");
			Print("Groups");
			Print("Students");
		}
	}
}
