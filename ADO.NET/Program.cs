﻿using System;
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
			//Connector.Select("*", "Directors");
			//Connector.Select("title,release_date,FORMATMESSAGE(N'%s %s',first_name,last_name)", "Movies,Directors", "director=director_id");
			Connector.InsertDirector("George", "Martin");
			Connector.SelectDirectors();
			Connector.SelectMovies();
		}
	}
}
