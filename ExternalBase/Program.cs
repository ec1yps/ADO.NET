using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ExternalBase
{
	internal class Program
	{
		static void Main(string[] args)
		{
            //Connector.Select("*", "Disciplines");
            //Console.WriteLine("discipline id: " + Connector.GetDisciplineID("N'Админ%Windows'"));

            //Connector.Select("*", "Teachers");
            //Console.WriteLine("teacher_id: " + Connector.GetTeacherID("N'Ковтун'"));

            Console.WriteLine("number of students: " + Connector.StudentsCounter());
        }
	}
}
