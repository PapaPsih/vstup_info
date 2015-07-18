using System;
using System.Globalization;

namespace VstupParser
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            DepartamentLoader dpLoader = new DepartamentLoader(2014);
            
            Int32 univecityId = 252;
            Int32 departamentId = 164475;

            StudentModel[] students = dpLoader.GetDepartamentStudents(univecityId, departamentId);

            foreach (StudentModel st in students)
            {
                Console.WriteLine("{1} - {0}", st.Name, st.AverageScore);
            }

            Console.ReadKey();
        }
    }
}
