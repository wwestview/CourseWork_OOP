using CourseWork_OOP.Interfaces;
using CourseWork_OOP.Services;
using System;
using System.IO;
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
                      .Replace("~", @"\textasciitilde{}");
        }

        private string GetText(string? text, string defaultText = "") => text ?? defaultText;

        public async Task GenerateAsync(TitlePageData data, string argument)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"\documentclass[12pt,a4paper]{article}");
            sb.AppendLine(@"\usepackage[utf8]{inputenc}");
            sb.AppendLine(@"\usepackage[ukrainian]{babel}");
            sb.AppendLine(@"\usepackage{graphicx}");
            sb.AppendLine(@"\usepackage{geometry}");
            sb.AppendLine(@"\usepackage{setspace}");
            sb.AppendLine(@"\usepackage{titlesec}");
            sb.AppendLine(@"\usepackage{parskip}");
            sb.AppendLine(@"\usepackage{fancyhdr}");
            sb.AppendLine(@"\geometry{top=2cm, bottom=2cm, left=3cm, right=1.5cm}");
            sb.AppendLine(@"\pagestyle{empty}");
            sb.AppendLine(@"\setlength{\parindent}{0pt}");
            sb.AppendLine(@"\renewcommand{\baselinestretch}{1}");

            sb.AppendLine(@"\begin{document}");
            sb.AppendLine(@"\thispagestyle{empty}");

            sb.AppendLine(@"\begin{center}");
            sb.AppendLine(@"Міністерство освіти і науки України\\");
            sb.AppendLine($@"\textbf{{{EscapeLatex(GetText(data.University))}}}");
            sb.AppendLine(@"\end{center}");
            sb.AppendLine(@"\vspace{1cm}");
            sb.AppendLine($@"Факультет {EscapeLatex(GetText(data.Faculty))}\\");
            sb.AppendLine($@"Кафедра {EscapeLatex(GetText(data.Department))}\\");
            sb.AppendLine(@"\vspace{1cm}");

            string workType = "КУРСОВА РОБОТА";
            if ((data.Discipline ?? "").ToLower().Contains("програмування"))
                workType = "КУРСОВА РОБОТА З ОБ’ЄКТНО-ОРІЄНТОВАНОГО ПРОГРАМУВАННЯ";

            sb.AppendLine(@"\begin{center}");
            sb.AppendLine($@"\textbf{{\Large {EscapeLatex(workType)}}}\\[0.5cm]");
            sb.AppendLine($@"на тему «\textbf{{{EscapeLatex(GetText(data.Topic))}}}»");
            sb.AppendLine(@"\end{center}");
            sb.AppendLine(@"\vspace{1.0cm}");

            sb.AppendLine(@"\begin{flushright}");
            string studentLabel = (data.Sex == "Жін") ? "Студентки" : "Студента";
            sb.AppendLine($@"{studentLabel} {EscapeLatex(GetText(data.CourseNumber, "2"))} курсу, групи {EscapeLatex(GetText(data.Group))}\\");
            sb.AppendLine($@"{EscapeLatex(GetText(data.SpecialtyName, "спеціальності 121 «Інженерія програмного забезпечення»"))}\\[1em]");
            sb.AppendLine($@"\textbf{{{EscapeLatex(GetText(data.StudentsFullName))}}}\\[1cm]");
            sb.AppendLine($@"Керівник:\hspace{{1em}}{EscapeLatex(GetText(data.SuperVisorPosition))}, \hspace{{0.5em}}{EscapeLatex(GetText(data.SuperVisorFullName))}\\");
            sb.AppendLine(@"\end{flushright}");
            sb.AppendLine(@"\vspace{1cm}");

            sb.AppendLine(@"\begin{flushright}");
            sb.AppendLine(@"Оцінка за шкалою: \underline{\hspace{6cm}}");
            sb.AppendLine(@"\end{flushright}");
            sb.AppendLine(@"\vspace{1cm}");

            sb.AppendLine(@"\begin{flushright}");
            sb.AppendLine(@"\textbf{Члени комісії:}\\[0.3cm]");
            for (int i = 0; i < 3; i++)
            {
                string memberName = (i < data.CommissionMemberNames?.Count && !string.IsNullOrWhiteSpace(data.CommissionMemberNames[i]))
                                    ? EscapeLatex(GetText(data.CommissionMemberNames[i]))
                                    : @"\makebox[6cm]{\hrulefill}";
                sb.AppendLine(@"\noindent" + @"\makebox[4cm]{\hrulefill}" + @"\hspace{1cm}" + memberName + @"\\");
                sb.AppendLine(@"\smallskip");
                sb.AppendLine(@"\hspace*{1.0cm}\makebox[4cm][c]{(підпис)}\hspace*{1.0cm}\makebox[6cm][c]{(прізвище та ініціали)}\\[0.5cm]");
            }
            sb.AppendLine(@"\end{flushright}");
            sb.AppendLine(@"\vfill");

            sb.AppendLine(@"\begin{center}");
            sb.AppendLine($@"{EscapeLatex(GetText(data.City))} – {data.Year} рік");
            sb.AppendLine(@"\end{center}");
            sb.AppendLine(@"\end{document}");

            string filePath = $"{argument}.tex";
            try { await File.WriteAllTextAsync(filePath, sb.ToString(), new UTF8Encoding(false)); System.Diagnostics.Debug.WriteLine($"LaTeX saved: {filePath}"); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"ERROR LaTeX save: {ex}"); throw; }
        }
    }
}