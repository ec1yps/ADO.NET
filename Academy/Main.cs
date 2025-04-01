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

namespace Academy
{
	public partial class Main : Form
	{
		Connector connector;

		Dictionary<string, int> d_directions;

		DataGridView[] tables;
		Query[] queries = new Query[]
			{
				new Query
				(
					"last_name,first_name,middle_name,birth_date,group_name,direction_name",
					"Students,Groups,Directions",
					"[group]=group_id AND direction=direction_id"
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
		public Main()
		{
			InitializeComponent();

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
			cbGroupsDirection.Items.AddRange(d_directions.Select(k => k.Key).ToArray());

			dgvStudents.DataSource = connector.Select("last_name,first_name,middle_name,birth_date,group_name,direction_name", "Students,Groups,Directions", "[group]=group_id AND direction=direction_id");
			toolStripStatusLabelCount.Text = $"Колличество студентов: {dgvStudents.RowCount - 1}";
		}

		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			int i = tabControl.SelectedIndex;
			Query query = queries[i];
			tables[i].DataSource = connector.Select(query.Columns, query.Tables, query.Condition, query.Group_by);
			toolStripStatusLabelCount.Text = status_message[i] + CountRecordsInDGV(tables[i]);
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
				toolStripStatusLabelCount.Text += "Колличество направлений" + CountRecordsInDGV(tables[tabControl.SelectedIndex]);
			}
			else tabControl_SelectedIndexChanged(sender, e);
		}
	}
}
