using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CourseWork_OOP.Services
{
    public class LaTeX : CoverPageGenerator
    {
        public LaTeX(CoverPageData data) : base(data)
        {

        }

        private string EscapeLatex(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace(@"\", @"\textbackslash{}").Replace("{", @"\{").Replace("}", @"\}")
                       .Replace("&", @"\&").Replace("%", @"\%").Replace("$", @"\$")
                       .Replace("#", @"\#").Replace("_", @"\_").Replace("^", @"\textasciicircum{}")
                       .Replace("~", @"\textasciitilde{}");
        }

        private string GetText(string? text, string defaultText = "") => text ?? defaultText;

        public override async Task GenerateAsync(string baseFileName)
        {
            var context = new StringBuilder();

            context.AppendLine(@"\documentclass[14pt, a4paper]{extarticle}");
            context.AppendLine(@"\usepackage[T2A]{fontenc}");
            context.AppendLine(@"\usepackage[utf8]{inputenc}");
            context.AppendLine(@"\usepackage[ukrainian]{babel}");
            context.AppendLine(@"\usepackage{geometry}");
            context.AppendLine(@"\usepackage{setspace}");
            context.AppendLine(@"\usepackage{amsmath}");
            context.AppendLine(@"\usepackage{graphicx}");
            context.AppendLine(@"\usepackage{indentfirst}");
            context.AppendLine(@"\geometry{left=3cm, right=1.5cm, top=2cm, bottom=2cm}");
            context.AppendLine(@"\onehalfspacing");

            context.AppendLine(@"\begin{document}");
            context.AppendLine(@"\sloppy");
            context.AppendLine(@"\begin{titlepage}"); 

            context.AppendLine(@"\begin{center}");
            context.AppendLine($@"Міністерство освіти і науки України\\[0.5em]");
            context.AppendLine($@"{{\bfseries {EscapeLatex(GetText(Data.University, "Назва Університету"))}}}\\[2cm]");
            context.AppendLine(@"\end{center}");

            context.AppendLine($@"\noindent Факультет \hspace{{1em}} {EscapeLatex(GetText(Data.Faculty, "Назва Факультету"))}\\[0.5em]");
            context.AppendLine($@"\noindent Кафедра \hspace{{1.5em}} {EscapeLatex(GetText(Data.Department, "Назва Кафедри"))}\\[4cm]"); 

            context.AppendLine(@"\begin{center}");
            context.AppendLine(@"{\Large\bfseries Курсова Робота}\\[1em]");
            context.AppendLine($@"{{по дисципліні «{EscapeLatex(GetText(Data.Discipline, "Назва дисципліни"))}»}}\\[1em]"); 
            context.AppendLine($@"{{На тему: «{EscapeLatex(GetText(Data.Topic, "Тема роботи"))}»}}\\[4cm]"); 
            context.AppendLine(@"\end{center}");

            context.AppendLine(@"\vspace*{\fill}");
            context.AppendLine(@"\noindent\hspace*{7cm}\begin{minipage}[t]{8cm}");
            context.AppendLine(@"\raggedright");

            context.AppendLine($@"Виконав: \hspace{{1em}} студент гр. {EscapeLatex(GetText(Data.Group, "Група"))}\\");
            context.AppendLine($@"\hspace{{2.5em}} {EscapeLatex(GetText(Data.StudentsFullName, "ПІБ Студента"))}\\[2cm]");

            context.AppendLine($@"Перевірив: \hspace{{0.5em}} {EscapeLatex(GetText(Data.SuperVisorPosition, "Посада"))}\\");
            context.AppendLine($@"\hspace{{2.5em}} {EscapeLatex(GetText(Data.SuperVisorFullName, "ПІБ Керівника"))}");

            context.AppendLine(@"\end{minipage}");
            context.AppendLine(@"\vspace*{\fill}");

            context.AppendLine(@"\begin{center}");
            context.AppendLine($@"{EscapeLatex(GetText(Data.City, "Місто"))}, {Data.Year}");
            context.AppendLine(@"\end{center}");

            context.AppendLine(@"\end{titlepage}"); 
            context.AppendLine(@"\end{document}"); 
            string filePath = $"{baseFileName}.tex";
            try
            {
                var utf8WithoutBom = new UTF8Encoding(false);
                await File.WriteAllTextAsync(filePath, context.ToString(), utf8WithoutBom);
                System.Diagnostics.Debug.WriteLine($"LaTeX file saved: {filePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ПОМИЛКА запису файлу {filePath}: {ex.ToString()}");
                throw;
            }
        }
    }
}
