using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;

namespace ADO.NET
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string cmd = 
				"SELECT title,release_date,FORMATMESSAGE(N'%s %s',first_name,last_name) FROM Movies, Directors WHERE director=director_id";
			
			Connector con = new Connector(cmd);
			con.GetData();
		}
	}
}
