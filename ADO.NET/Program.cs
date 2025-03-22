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
			//Connector.Select("*", "Directors");
			//Connector.Select("title,release_date,FORMATMESSAGE(N'%s %s',first_name,last_name)", "Movies,Directors", "director=director_id");
			//Connector.InsertDirector("George", "Martin");
			//Connector.Insert("Directors(first_name,last_name)", "N'George',N'Lucas'");
			Connector.SelectDirectors();

			Connector.Insert("Movies(title,release_date,director)", "N'Star Wars: Episod 1 - The Phantom Menace', N'1999.05.16', N'207'");
			//Connector.InsertMovies("Avatar", 10, 12, 2009, 1);
			Connector.SelectMovies();

		}
	}
}
