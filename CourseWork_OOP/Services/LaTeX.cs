using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CourseWork_OOP.Services
{
    public class LaTeX : BaseTitlePageGenerator 
    {
        public override string FormatName => "LaTeX";
        public override string FileExtension => ".tex";

        public LaTeX() { }

        private string EscapeLatex(string? text) 
        {
            string inputText = GetText(text); 
            if (string.IsNullOrEmpty(inputText)) return string.Empty;
            return inputText.Replace(@"\", @"\textbackslash{}").Replace("{", @"\{").Replace("}", @"\}")
                       .Replace("&", @"\&").Replace("%", @"\%").Replace("$", @"\$")
                       .Replace("#", @"\#").Replace("_", @"\_").Replace("^", @"\textasciicircum{}")
                       .Replace("~", @"\textasciitilde{}")
                       .Replace("\"", "''")
                       .Replace(" - ", @" -- ")
                       .Replace("–", @"--")
                       .Replace("—", @"---");
        }

        public override async Task GenerateAsync(TitlePageData data, string baseFileName) 
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
            sb.AppendLine(@"\geometry{left=3cm, right=1.5cm, top=2cm, bottom=2cm}");
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
            sb.AppendLine(@"\large {КУРСОВА РОБОТА З ДИСЦИПЛІНИ} \\"); 
            sb.AppendLine($@"\large {{«{EscapeLatex(GetText(data.Discipline, "НАЗВА ДИСЦИПЛІНИ").ToUpperInvariant())}»}} \\[4pt]");
            sb.AppendLine($@" на тему «{{{EscapeLatex(GetText(data.Topic, "Тема не вказана").ToUpperInvariant())}}}»");
            sb.AppendLine(@"\end{center}");
            sb.AppendLine(@"\vspace{1.3cm}");
            sb.AppendLine(@"\begin{flushright}");
            string studentLabel = (GetText(data.Sex) == "Жін") ? "Студентки" : "Студента";
            sb.AppendLine($@"{studentLabel} {EscapeLatex(GetText(data.CourseNumber, "X"))} курсу, групи {EscapeLatex(GetText(data.Group, "XXXX"))} \\");
            sb.AppendLine($@"{EscapeLatex(GetText(data.SpecialtyName, "Спеціальність не вказана"))} \\");
            sb.AppendLine($@"{{{EscapeLatex(GetText(data.StudentsFullName, "ПІБ Студента"))}}} \\[1cm]"); 
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
                string latexSignatureLabel = @"\makebox[3.5cm][c]{\footnotesize(підпис)}"; 
                for (int i = 0; i < 3; i++)
                {
                    string commissionLabel = (i == 0) ? @"\textbf{Члени комісії:} \hspace{0.4cm}" : "";
                    string memberPii = (i < data.CommissionMemberNames.Count && !string.IsNullOrWhiteSpace(data.CommissionMemberNames[i]))
                                       ? EscapeLatex(GetText(data.CommissionMemberNames[i]))
                                       : EscapeLatex(GetText(null, @"\phantom{Прізвище І.П.}")); 

                    sb.Append($@"  {commissionLabel}&{signatureLine} & {memberPii} \\");
                    sb.Append($@"  &{latexSignatureLabel} & ");
                    sb.AppendLine((i < 2) ? @"\\[0.8em]" : "");
                }
                sb.AppendLine(@"\end{tabular}");
                sb.AppendLine(@"\end{flushright}");
            }
            else
            {
                sb.AppendLine(@"\begin{flushright}");
                sb.AppendLine(@"\textbf{Члени комісії:} \\");
                sb.AppendLine(@"\vspace{2.4em}"); 
                sb.AppendLine(@"\end{flushright}");
            }
            sb.AppendLine(@"\vfill");
            sb.AppendLine(@"\begin{center}");
            sb.AppendLine($@"{EscapeLatex(GetText(data.City))} – {EscapeLatex(GetText(data.Year.ToString()))} рік");
            sb.AppendLine(@"\end{center}");
            sb.AppendLine(@"\end{document}");

            string filePath = $"{baseFileName}{this.FileExtension}"; 
            try
            {
                await File.WriteAllTextAsync(filePath, sb.ToString(), new UTF8Encoding(false));
                Debug.WriteLine($"LaTeX file saved: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ПОМИЛКА запису LaTeX файлу {filePath}: {ex}");
                throw;
            }
        }
    }
}