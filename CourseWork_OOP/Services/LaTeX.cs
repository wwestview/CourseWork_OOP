using CourseWork_OOP.Interfaces;
using CourseWork_OOP.Services; 
using System;
using System.IO;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;

namespace CourseWork_OOP.Services
{
    public class LaTeX : ITitlePageGenerator 
    {

        public LaTeX() { }

        private string EscapeLatex(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Replace(@"\", @"\textbackslash{}").Replace("{", @"\{").Replace("}", @"\}")
                       .Replace("&", @"\&").Replace("%", @"\%").Replace("$", @"\$")
                       .Replace("#", @"\#").Replace("_", @"\_").Replace("^", @"\textasciicircum{}")
                       .Replace("~", @"\textasciitilde{}")
                       .Replace(" - ", @" -- ")
                       .Replace("–", @"--")
                       .Replace("—", @"---");
        }

        private string GetText(string? text, string defaultPlaceholder = "") =>
            string.IsNullOrWhiteSpace(text) ? defaultPlaceholder : text;

        public async Task GenerateAsync(TitlePageData data, string argument) 
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"\documentclass[a4paper,12pt]{article}");
            sb.AppendLine(@"\usepackage[utf8]{inputenc}");
            sb.AppendLine(@"\usepackage[ukrainian]{babel}");
            sb.AppendLine(@"\usepackage{mathptmx}"); 
            sb.AppendLine(@"\usepackage{graphicx}");
            sb.AppendLine(@"\usepackage{geometry}");
            sb.AppendLine(@"\usepackage{setspace}");
            sb.AppendLine(@"\usepackage{parskip}"); 
            sb.AppendLine(@"\usepackage{ulem}");    
            sb.AppendLine(@"\geometry{");
            sb.AppendLine(@"  left=3cm,");
            sb.AppendLine(@"  right=1.5cm,");
            sb.AppendLine(@"  top=2cm,");
            sb.AppendLine(@"  bottom=2cm");
            sb.AppendLine(@"}");
            sb.AppendLine(@"\pagestyle{empty}");
            sb.AppendLine(@"\setlength{\parindent}{0pt}");
            sb.AppendLine(@"\linespread{1.2}"); 

            sb.AppendLine(@"\begin{document}");
            sb.AppendLine(@"\thispagestyle{empty}");

            sb.AppendLine(@"\begin{center}");
            sb.AppendLine(@"Міністерство освіти і науки України \\[4pt]");
            sb.AppendLine($@"{{{EscapeLatex(GetText(data.University, "НАЗВА УНІВЕРСИТЕТУ"))}}}");
            sb.AppendLine(@"\end{center}");

            sb.AppendLine(@"\vspace{0.8cm}");

            sb.AppendLine(@"\begin{center}");
            sb.AppendLine($@"Факультет {EscapeLatex(GetText(data.Faculty, "Факультет не вказано"))} \\");
            sb.AppendLine($@"Кафедра {EscapeLatex(GetText(data.Department, "Кафедру не вказано"))}");
            sb.AppendLine(@"\end{center}");

            sb.AppendLine(@"\vspace{1.2cm}");

            sb.AppendLine(@"\begin{center}");
            sb.AppendLine(@"\large \textbf{КУРСОВА РОБОТА З ДИСЦИПЛІНИ} \\");
            sb.AppendLine($@"\large \textbf{{«{EscapeLatex(GetText(data.Discipline, "НАЗВА ДИСЦИПЛІНИ").ToUpperInvariant())}»}} \\[4pt]");
            sb.AppendLine($@"\normalsize на тему «\textbf{{{EscapeLatex(GetText(data.Topic, "Тема не вказана"))}}}»");
            sb.AppendLine(@"\end{center}");

            sb.AppendLine(@"\vspace{1.3cm}");

            sb.AppendLine(@"\begin{flushright}");
            string studentLabel = (GetText(data.Sex) == "Жін") ? "Студентки" : "Студента";
            sb.AppendLine($@"{studentLabel} {EscapeLatex(GetText(data.CourseNumber, "X"))} курсу, групи {EscapeLatex(GetText(data.Group, "XXXX"))} \\");
            sb.AppendLine($@"{EscapeLatex(GetText(data.SpecialtyName, "Спеціальність не вказана"))} \\");
            sb.AppendLine($@"\textbf{{{EscapeLatex(GetText(data.StudentsFullName, "ПІБ Студента"))}}} \\[1cm]");
            sb.AppendLine($@"Керівник:\hspace{{0.5em}}{EscapeLatex(GetText(data.SuperVisorPosition, "Посада керівника"))} , {EscapeLatex(GetText(data.SuperVisorFullName, "ПІБ Керівника"))} \\");
            sb.AppendLine(@"\footnotesize (посада, вчене звання, науковий ступінь, прізвище та ініціали)");
            sb.AppendLine(@"\end{flushright}");

            sb.AppendLine(@"\vspace{1cm}");

            sb.AppendLine(@"\begin{flushright}");
            sb.AppendLine(@"Оцінка за шкалою:\hspace{0.5em}\uline{\hspace{5.5cm}} \\");
            sb.AppendLine(@"\footnotesize (національною, кількість балів, ECTS)");
            sb.AppendLine(@"\end{flushright}");

            sb.AppendLine(@"\vspace{0.9cm}");

            if (data.CommissionMemberNames != null && data.CommissionMemberNames.Any())
            {
                sb.AppendLine(@"\begin{flushright}");
                sb.AppendLine(@"\begin{tabular}{l@{}l@{\hspace{0.4cm}}l@{}}");
                sb.AppendLine(@"  && \\");
                sb.AppendLine(@"  && \\");

                string signatureLine = @"\makebox[3.5cm]{\hrulefill}";
                string signatureLabel = @"\makebox[3.5cm][c]{\footnotesize(підпис)}";


                for (int i = 0; i < 3; i++)
                {
                    string commissionLabel = (i == 0) ? @"\textbf{Члени комісії:} \hspace{0.4cm}" : "";
                    string memberPii = EscapeLatex(GetText(data.CommissionMemberNames[i], @"\phantom{ПІБ Члена Комісії}"));

                    sb.Append($@"  {commissionLabel}&{signatureLine} & {memberPii} \\");
                    sb.Append($@"  &{signatureLabel} & ");
                    sb.AppendLine((i < 3 - 1) ? @"\\[0.8em]" : ""); 
                }
                sb.AppendLine(@"\end{tabular}");
                sb.AppendLine(@"\end{flushright}");
            }

            sb.AppendLine(@"\vfill");

            sb.AppendLine(@"\begin{center}");
            sb.AppendLine($@"{EscapeLatex(GetText(data.City))} – {EscapeLatex(GetText(data.Year.ToString()))} рік");
            sb.AppendLine(@"\end{center}");

            sb.AppendLine(@"\end{document}");

            string filePath = $"{argument}.tex"; 
            try
            {
                var utf8NoBom = new UTF8Encoding(false);
                await File.WriteAllTextAsync(filePath, sb.ToString(), utf8NoBom);
                System.Diagnostics.Debug.WriteLine($"LaTeX file saved: {filePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ПОМИЛКА запису LaTeX файлу {filePath}: {ex}");
                throw;
            }
        }
    }
}