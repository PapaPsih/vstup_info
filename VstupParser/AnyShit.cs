using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VstupParser
{


    class PageLoader
    {
        /// <summary>
        /// Create new web request to url
        /// </summary>
        /// <param name="url">Url to request</param>
        /// <returns>WebResponse from url</returns>
        public static WebResponse Load(Uri url)
        {
            WebRequest request = WebRequest.Create(url);
            return request.GetResponse();
        }

        /// <summary>
        /// Create new web request to url
        /// </summary>
        /// <param name="url">Url to request</param>
        /// <returns>String response from url</returns>
        public static String LoadToString(Uri url)
        {
            WebResponse response = Load(url);
            StreamReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
            return reader.ReadToEnd();
        }
    }

    class DepartamentLoader
    {
        private Int32 Year;

        public DepartamentLoader(Int32 Year)
        {
            this.Year = Year;
        }

        public StudentModel[] GetDepartamentStudents(Int32 UniversityId, Int32 DepartamentId)
        {
            String htmlText = PageLoader.LoadToString(UrlMaster.GetDepartementUri(Year, UniversityId, DepartamentId));
            return DepartamentStudentsParser.Parse(htmlText);
        }
    }

    class UrlMaster
    {
        /// <summary>
        /// Returns URL's page with departament info and totals
        /// </summary>
        /// <param name="Year">Year for search</param>
        /// <param name="UniversityId">Id of university</param>
        /// <param name="DepartamentId">Id of departament</param>
        /// <returns>URL page</returns>
        public static Uri GetDepartementUri(Int32 Year, Int32 UniversityId, Int32 DepartamentId)
        {
            return new Uri(String.Format("http://vstup.info/{0}/{1}/i{0}i{1}p{2}.html", Year, UniversityId, DepartamentId));
        }
    }

    class StudentModel
    {
        public Int32 Position { get; set; }

        public String Name { get; set; }

        public Double Total { get; set; }

        public Double AverageScore { get; set; }

        public StudentStatus Status { get; set; }
    }

    class DepartamentStudentsParser
    {
        public static StudentModel[] Parse(String Page)
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(Page);

            HtmlNodeCollection nodes = document.DocumentNode
                .SelectSingleNode("//tbody").ChildNodes;

            List<StudentModel> students = new List<StudentModel>();

            foreach (HtmlNode node in nodes)
            {
                if (node.Name != "tr" || node.ChildNodes.Count != 12)
                    continue;

                students.Add(getStudent(node));
            }

            return students.ToArray();
        }

        private static StudentModel getStudent(HtmlNode node)
        {
            StudentStatus status;
            if (node.ChildNodes[0].Attributes["style"] != null)
            {
                if (node.ChildNodes[0].Attributes["style"].Value == "background:#b4caeb")
                {
                    status = StudentStatus.Accepted;
                }
                else //if (node.ChildNodes[0].Attributes["style"].Value == "background:lightgreen")
                {
                    status = StudentStatus.Recommended;
                }
            }
            else
            {
                status = StudentStatus.Allowed;
            }
            return new StudentModel()
            {
                Position = Convert.ToInt32(node.ChildNodes[0].InnerText),
                Name = node.ChildNodes[1].InnerText,
                Total = Convert.ToDouble(node.ChildNodes[2].InnerText),
                AverageScore = Convert.ToDouble(node.ChildNodes[3].InnerText),
                Status = status
            };
        }
    }

    public enum StudentStatus
    {
        Accepted,
        Recommended,
        Allowed
    }
}
