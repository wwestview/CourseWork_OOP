using System;
using System.Collections.Generic;

namespace CourseWork_OOP.Services
{
    public class TitlePageData
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
        public string? Sex { get; set; }
        public string? CourseNumber { get; set; }
        public string? SpecialtyName { get; set; }
        public List<string> CommissionMemberNames { get; set; } = new List<string>();

    }
}