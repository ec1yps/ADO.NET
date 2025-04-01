using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy
{
	internal class Query
	{
		public string Columns { get; }
		public string Tables { get; }
		public string Condition { get; }
		public string Group_by { get; }

		public Query(string columns, string tables, string condition = "", string group_by = "")
		{
			Columns = columns;
			Tables = tables;
			Condition = condition;
			Group_by = group_by;
		}
	}
}
