using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork_OOP.Services
{
    public class CoverPageData
    {
        public string? StudentsFullName { get; set; }
        public string? SuperVisorFullName { get; set; }
        public string? SuperVisorPosition { get; set; }
        public string? Group { get; set; }
        public string? Topic { get; set; }
        public string? Discipline { get; set; }
        public string? University { get; set; }
        public string? Faculty { get; set; }
        public string? Department { get; set; }
        public string? City { get; set; } = "Черкаси";
        public int Year { get; set; } = DateTime.Now.Year;
    }
}
