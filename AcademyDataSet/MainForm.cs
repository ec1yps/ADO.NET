using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using System.Data.SqlClient;
using System.Configuration;
using System.Web;

namespace AcademyDataSet
{
	public partial class MainForm : Form
	{
		readonly string CONNECTION_STRING = "";
		SqlConnection connection;
		DataSet GroupsRelatedData;

		public MainForm()
		{
			InitializeComponent();
			AllocConsole();
			CONNECTION_STRING = ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString;
			connection = new SqlConnection(CONNECTION_STRING);
			Console.WriteLine(CONNECTION_STRING);

			LoadGroupsRelatedData();
		}

		void AddTablesAndColumns(string table, string[] columns, string primaryColumn)
		{
			GroupsRelatedData.Tables.Add(table);

			for (int i = 0; i < columns.Length; i++)
			{
				GroupsRelatedData.Tables[table].Columns.Add(columns[i]);
			}

			GroupsRelatedData.Tables[table].PrimaryKey = new DataColumn[] { GroupsRelatedData.Tables[table].Columns[primaryColumn] };
		}

		void LoadData(string table, string columns, string condition = "")
		{
			string cmd = $"SELECT {columns} FROM {table}";
			if (condition != "") cmd += $" WHERE {condition}";
			cmd += ";";

			SqlDataAdapter adapter = new SqlDataAdapter(cmd, connection);
			connection.Open();
			adapter.Fill(GroupsRelatedData.Tables[table]);
			connection.Close();
		}

		void LoadGroupsRelatedData()
		{
			Console.WriteLine(nameof(GroupsRelatedData));
			GroupsRelatedData = new DataSet(nameof(GroupsRelatedData));
			
			const string dsTable_Directions = "Directions";
			/*const string dst_col_direction_id = "direction_id";
			const string dst_col_direction_name = "direction_name";*/

			/*GroupsRelatedData.Tables.Add(dsTable_Directions);

			GroupsRelatedData.Tables[dsTable_Directions].Columns.Add(dst_col_direction_id, typeof(byte));
			GroupsRelatedData.Tables[dsTable_Directions].Columns.Add(dst_col_direction_name, typeof(string));
			GroupsRelatedData.Tables[dsTable_Directions].PrimaryKey =
				new DataColumn[] { GroupsRelatedData.Tables[dsTable_Directions].Columns[dst_col_direction_id] };*/

			string[] DirectionsColumns = { "direction_id", "direction_name" };

			AddTablesAndColumns(dsTable_Directions, DirectionsColumns, DirectionsColumns[0]);

			const string dsTable_Groups = "Groups";
			/*const string dst_Groups_col_group_id = "group_id";
			const string dst_Groups_col_group_name = "group_name";
			const string dst_Groups_col_direction = "direction";*/

			string[] GroupsColumns = { "group_id", "group_name", "direction" };

			AddTablesAndColumns(dsTable_Groups, GroupsColumns, GroupsColumns[0]);

			/*GroupsRelatedData.Tables.Add(dsTable_Groups);

			GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_Groups_col_group_id, typeof(int));
			GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_Groups_col_group_name, typeof(string));
			GroupsRelatedData.Tables[dsTable_Groups].Columns.Add(dst_Groups_col_direction, typeof(byte));
			GroupsRelatedData.Tables[dsTable_Groups].PrimaryKey =
				new DataColumn[] { GroupsRelatedData.Tables[dsTable_Groups].Columns[dst_Groups_col_group_id] };*/

			string dsRelation_GroupsDirections = "GroupsDirections";
			GroupsRelatedData.Relations.Add
				(
					dsRelation_GroupsDirections,
					GroupsRelatedData.Tables["Directions"].Columns["direction_id"],
					GroupsRelatedData.Tables["Groups"].Columns["direction"]
				);

			LoadData(dsTable_Directions, "*");
			LoadData(dsTable_Groups, "*");

			/*string directions_cmd = "SELECT * FROM Directions";
			string groups_cmd = "SELECT * FROM Groups";

			SqlDataAdapter directionsAdapter = new SqlDataAdapter(directions_cmd, connection);
			SqlDataAdapter groupsAdapter = new SqlDataAdapter(groups_cmd, connection);

			connection.Open();
			directionsAdapter.Fill(GroupsRelatedData.Tables[dsTable_Directions]);
			groupsAdapter.Fill(GroupsRelatedData.Tables[dsTable_Groups]);
			connection.Close();*/

			foreach (DataRow row in GroupsRelatedData.Tables[dsTable_Directions].Rows)
				Console.WriteLine($"{row[DirectionsColumns[0]]}\t{row[DirectionsColumns[1]]}");

			Console.WriteLine("\n---------------------------------------------------------------\n");

			foreach (DataRow row in GroupsRelatedData.Tables[dsTable_Groups].Rows)
				Console.WriteLine($"{row[GroupsColumns[0]]}\t{row[GroupsColumns[1]]}\t{row.GetParentRow(dsRelation_GroupsDirections)[DirectionsColumns[1]]}");
		}

		[DllImport("kernel32.dll")]
		public static extern bool AllocConsole();
		[DllImport("kernel32.dll")]
		public static extern bool FreeConsole();
	}
}
