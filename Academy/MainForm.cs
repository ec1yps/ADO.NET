using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Configuration;
using System.Runtime.InteropServices;

namespace Academy
{
	public partial class MainForm : Form
	{
		Connector connector;

		public Dictionary<string, int> d_directions;
		public Dictionary<string, int> d_groups;

		public Dictionary<ComboBox, List<ComboBox>> d_dependencies;

		DataGridView[] tables;
		Query[] queries = new Query[]
			{
				new Query
				(
					"last_name,first_name,middle_name,birth_date,group_name,direction_name",
					"Students JOIN Groups ON([group]=group_id) JOIN Directions ON(direction=direction_id)"
					//"[group]=group_id AND direction=direction_id"
				),
				new Query
				(
					"group_name,dbo.GetLearningDaysFor(group_name) AS weekdays,start_time,direction_name",
					"Groups,Directions",
					"direction=direction_id"
				),
				new Query
				(
					"direction_name AS N'Название направления',COUNT(DISTINCT group_id) AS N'Количество групп',COUNT(stud_id) AS N'Количество студентов'",
					"Students RIGHT JOIN Groups ON([group]=group_id),Directions ",
					"direction=direction_id",
					"direction_name"
				),
				new Query("*", "Disciplines"),
				new Query("*", "Teachers"),
				new Query
				(
					"direction_name AS N'Название направления',COUNT(DISTINCT group_id) AS N'Количество групп',COUNT(stud_id) AS N'Количество студентов'",
					"Students RIGHT JOIN Groups ON([group]=group_id) RIGHT JOIN Directions ON(direction=direction_id)",
					"",
					"direction_name"
				)
			};

		string[] status_message = new string[]
			{
				$"Колличество студентов: ",
				$"Колличество групп: ",
				$"Колличество направлений: ",
				$"Колличество дисциплин: ",
				$"Колличество преподавателей: ",
			};
		public MainForm()
		{
			InitializeComponent();

			d_dependencies = new Dictionary<ComboBox, List<ComboBox>>
			{
				{ cbStudentsDirection, new List<ComboBox>(){ cbStudentsGroup } }
			};

			tables = new DataGridView[]
				{
					dgvStudents,
					dgvGroups,
					dgvDirections,
					dgvDisciplines,
					dgvTeachers
				};

			connector = new Connector
				(
					ConfigurationManager.ConnectionStrings["PV_319_Import"].ConnectionString
				);

			d_directions = connector.GetDictionary("*", "Directions");
			d_groups = connector.GetDictionary("group_id,group_name", "Groups");

			cbStudentsGroup.Items.AddRange(d_groups.Select(g => g.Key).ToArray());
			cbStudentsDirection.Items.AddRange(d_directions.Select(d => d.Key).ToArray());
			cbGroupsDirection.Items.AddRange(d_directions.Select(d => d.Key).ToArray());

			cbStudentsGroup.Items.Insert(0, "Все группы");
			cbStudentsDirection.Items.Insert(0, "Все направления");
			cbGroupsDirection.Items.Insert(0, "Все направления");

			cbStudentsGroup.SelectedIndex = 0;
			cbStudentsDirection.SelectedIndex = 0;
			cbGroupsDirection.SelectedIndex = 0;

			dgvStudents.DataSource = connector.Select("last_name,first_name,middle_name,birth_date,group_name,direction_name", "Students,Groups,Directions", "[group]=group_id AND direction=direction_id");
			toolStripStatusLabelCount.Text = $"Колличество студентов: {dgvStudents.RowCount - 1}";
		}

		void LoadPage(int i, Query query = null)
		{
			if(query == null) query = queries[i];
			tables[i].DataSource = connector.Select(query.Columns, query.Tables, query.Condition, query.Group_by);
			toolStripStatusLabelCount.Text = status_message[i] + CountRecordsInDGV(tables[i]);
		}

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			string tab_name = tabControl.SelectedTab.Name;
			Console.WriteLine(tab_name);

			LoadPage(tabControl.SelectedIndex);

			/*switch (tabControl.SelectedIndex)
			{
				case 0:
					dgvStudents.DataSource = connector.Select
						(
							"last_name,first_name,middle_name,birth_date,group_name,direction_name",
							"Students,Groups,Directions",
							"[group]=group_id AND direction=direction_id"
						);
					toolStripStatusLabelCount.Text = $"Колличество студентов: {dgvStudents.RowCount - 1}.";
					break;
				case 1:
					dgvGroups.DataSource = connector.Select
						(
							"group_name,dbo.GetLearningDaysFor(group_name) AS weekdays,start_time,direction_name",
							"Groups,Directions",
							"direction=direction_id"
						);
					toolStripStatusLabelCount.Text = $"Колличество групп: {dgvGroups.RowCount - 1}.";
					break;
				case 2:
					//dgvDirections.DataSource = connector.Select
					//	(
					//		"direction_name,COUNT(DISTINCT group_id) AS N'Количество групп',COUNT(DISTINCT stud_id) AS N'Количество студентов'", 
					//		"Students,Groups,Directions",
					//		"[group]=group_id AND direction=direction_id",
					//		"direction_name"
					//	);
					dgvDirections.DataSource = connector.Select
						(
							"direction_name,COUNT(DISTINCT group_id) AS N'Количество групп',COUNT(DISTINCT stud_id) AS N'Количество студентов'",
							"Students RIGHT JOIN Groups ON([group]=group_id) RIGHT JOIN Directions ON(direction=direction_id)",
							"",
							"direction_name"
						);
					toolStripStatusLabelCount.Text = $"Колличество направлений: {dgvDirections.RowCount - 1}.";
					break;
				case 3:
					dgvDisciplines.DataSource = connector.Select("*", "Disciplines");
					toolStripStatusLabelCount.Text = $"Колличество дисциплин: {dgvDisciplines.RowCount - 1}.";
					break;
				case 4:
					dgvTeachers.DataSource = connector.Select("*", "Teachers");
					toolStripStatusLabelCount.Text = $"Колличество преподавателей: {dgvTeachers.RowCount - 1}.";
					break;
			}*/
		}


		int CountRecordsInDGV(DataGridView dgv)
		{
			return dgv.RowCount == 0 ? 0 : dgv.RowCount - 1;
		}

		private void cb_ShowEmptyDirections_CheckedChanged(object sender, EventArgs e)
		{
			Query query = queries.Last();
			if (cb_ShowEmptyDirections.Checked)
			{
				tables[tabControl.SelectedIndex].DataSource = connector.Select
					(
						query.Columns, query.Tables, query.Condition, query.Group_by
					);
				toolStripStatusLabelCount.Text = "Колличество направлений: " + CountRecordsInDGV(tables[tabControl.SelectedIndex]);
			}
			else tabControl_SelectedIndexChanged(sender, e);
		}

		private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			string cb_name = (sender as ComboBox).Name;
            Console.WriteLine(cb_name);
            string tab_name = tabControl.SelectedTab.Name;
			Console.WriteLine(tab_name);

			int last_capitalIndex = Array.FindLastIndex<char>(cb_name.ToCharArray(), Char.IsUpper);
			string cb_suffix = cb_name.Substring(last_capitalIndex, cb_name.Length-last_capitalIndex);
            Console.WriteLine(cb_suffix);

			string dictionary_name = $"d_{cb_suffix.ToLower()}s";
			Dictionary<string, int> dictionary = 
				this.GetType().GetField(dictionary_name).GetValue(this) as Dictionary<string, int>;

            int i = (sender as ComboBox).SelectedIndex;

			#region Filtercb_StudentsGroup
				/*Dictionary<string, int> d_groups = connector.GetDictionary
					(
						"group_id,group_name",
						"Groups",
						i == 0 ? "" : $"{cb_suffix.ToLower()}={dictionary[(sender as ComboBox).SelectedItem.ToString()]}"
					);

				cbStudentsGroup.Items.Clear();
				cbStudentsGroup.Items.AddRange(d_groups.Select(g => g.Key).ToArray());
				cbStudentsGroup.Items.Insert(0, "Все группы"); */
			if(d_dependencies.ContainsKey(sender as ComboBox))
			{
				foreach (ComboBox cb in d_dependencies[sender as ComboBox])
				{
					GetDependentData(cb, sender as ComboBox);
				}
			}
			#endregion

			Query query = new Query(queries[tabControl.SelectedIndex]);
			/*string condition;
			if (cbStudentsDirection.SelectedIndex != 0 && cbStudentsGroup.SelectedIndex == 0)
				condition = $"direction={cbStudentsDirection.SelectedIndex}";
			else 
			{
				condition =
					(
						i == 0 || (sender as ComboBox).SelectedItem == null ?
						"" :
						$"[{cb_suffix.ToLower()}]={dictionary[$"{(sender as ComboBox).SelectedItem}"]}"
					);
			}*/
			string condition =
					(
						i == 0 || (sender as ComboBox).SelectedItem == null ?
						"" :
						$"[{cb_suffix.ToLower()}]={dictionary[$"{(sender as ComboBox).SelectedItem}"]}"
					);

			if (query.Condition == "") query.Condition = condition;
			else if (condition != "") query.Condition += $" AND {condition}";
				
			LoadPage(tabControl.SelectedIndex, query);
		}

		void GetDependentData(ComboBox dependent, ComboBox determinant)
		{
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine(dependent.Name + "\t" + determinant.Name);

			string dependent_root = 
				dependent.Name.Substring(Array.FindLastIndex<char>(dependent.Name.ToCharArray(), Char.IsUpper));
			string determinant_root = 
				determinant.Name.Substring(Array.FindLastIndex<char>(determinant.Name.ToCharArray(), Char.IsUpper));

			Dictionary<string, int> dictionary = 
				connector.GetDictionary
				(
					$"{dependent_root.ToLower()}_id,{dependent_root.ToLower()}_name",
					$"{dependent_root}s,{determinant_root}s",
					determinant.SelectedIndex == 0 ? "" : $"{determinant_root.ToLower()}={determinant.SelectedIndex}"
				);
			
			dependent.Items.Clear();
			dependent.Items.AddRange(dictionary.Select(d => d.Key).ToArray());
			dependent.Items.Insert(0, "Все группы");

			Console.WriteLine("Dependent:\t" + dependent_root);
            Console.WriteLine("Determinant:\t" + determinant_root);
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }
	}
}
