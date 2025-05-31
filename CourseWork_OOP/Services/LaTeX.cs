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

        private StringBuilder _sb;
        private string _baseOutputNameWithDir;

        public LaTeX() { }

        private string EscapeLatex(string? text)
        {
            string inputText = GetText(text);
            if (string.IsNullOrEmpty(inputText)) return string.Empty;
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

        protected override void InitializeInternalContext(TitlePageData data, string argument)
        {
            _sb = new StringBuilder();
            _baseOutputNameWithDir = argument;
            Debug.WriteLine($"[{FormatName}.InitializeInternalContext] Context initialized. Base file path: \"{_baseOutputNameWithDir}\"");

            // Преамбула LaTeX
            _sb.AppendLine(@"\documentclass[a4paper,12pt]{article}");
            _sb.AppendLine(@"\usepackage[utf8]{inputenc}");
            _sb.AppendLine(@"\usepackage[ukrainian]{babel}");
            _sb.AppendLine(@"\usepackage{mathptmx}");
            _sb.AppendLine(@"\usepackage{graphicx}");
            _sb.AppendLine(@"\usepackage{geometry}");
            _sb.AppendLine(@"\usepackage{setspace}");
            _sb.AppendLine(@"\usepackage{parskip}");
            _sb.AppendLine(@"\usepackage{ulem}");
            _sb.AppendLine(@"\geometry{left=3cm, right=1.5cm, top=2cm, bottom=2cm}");
            _sb.AppendLine(@"\pagestyle{empty}");
            _sb.AppendLine(@"\setlength{\parindent}{0pt}");
            _sb.AppendLine(@"\linespread{1.2}");
            _sb.AppendLine(@"\begin{document}");
            _sb.AppendLine(@"\thispagestyle{empty}");
        }

        protected override void BuildHeader(TitlePageData data)
        {
            _sb.AppendLine(@"\begin{center}");
            _sb.AppendLine(@"Міністерство освіти і науки України \\[4pt]");
            _sb.AppendLine($@"{{{EscapeLatex(GetText(data.University, "НАЗВА УНІВЕРСИТЕТУ"))}}}");
            _sb.AppendLine(@"\end{center}");
        }

        protected override void BuildFacultyAndDepartment(TitlePageData data)
        {
            _sb.AppendLine(@"\vspace{0.0cm}");
            _sb.AppendLine(@"\begin{center}");
            _sb.AppendLine($@"Факультет {EscapeLatex(GetText(data.Faculty, "Факультет не вказано"))} \\");
            _sb.AppendLine($@"Кафедра {EscapeLatex(GetText(data.Department, "Кафедру не вказано"))}");
            _sb.AppendLine(@"\end{center}");
        }

        protected override void BuildWorkTitle(TitlePageData data)
        {
            _sb.AppendLine(@"\vspace{3.2cm}");
            _sb.AppendLine(@"\begin{center}");
            _sb.AppendLine($@"\large КУРСОВА РОБОТА З {{«{EscapeLatex(GetText(data.Discipline, "НАЗВА ДИСЦИПЛІНИ").ToUpperInvariant())}»}} \\[4pt]");
            _sb.AppendLine($@"на тему «{{{EscapeLatex(GetText(data.Topic, "Тема не вказана"))}}}»"); 
            _sb.AppendLine(@"\end{center}");
        }

        protected override void BuildStudentAndSupervisor(TitlePageData data)
        {
            _sb.AppendLine(@"\vspace{1.3cm}");
            _sb.AppendLine(@"\begin{flushright}");
            string studentLabel = (GetText(data.Sex) == "Жін") ? "Студентки" : "Студента";
            _sb.AppendLine($@"{studentLabel} {EscapeLatex(GetText(data.CourseNumber, "X"))} курсу, групи {EscapeLatex(GetText(data.Group, "XXXX"))} \\");
            _sb.AppendLine($@"{EscapeLatex(GetText(data.SpecialtyName, "Спеціальність не вказана"))} \\");
            _sb.AppendLine($@"{{{EscapeLatex(GetText(data.StudentsFullName, "ПІБ Студента"))}}} \\[0cm]"); 
            _sb.AppendLine($@"Керівник:\hspace{{0.5em}}{EscapeLatex(GetText(data.SuperVisorPosition, "Посада керівника"))} , {EscapeLatex(GetText(data.SuperVisorFullName, "ПІБ Керівника"))} \\");
            _sb.AppendLine(@"\footnotesize (посада, вчене звання, науковий ступінь, прізвище та ініціали)");
            _sb.AppendLine(@"\end{flushright}");
        }

        protected override void BuildEvaluation(TitlePageData data)
        {
            _sb.AppendLine(@"\vspace{1cm}");
            _sb.AppendLine(@"\begin{flushright}");
            _sb.AppendLine(@"Оцінка за шкалою:\hspace{0.5em}\uline{\hspace{5.5cm}} \\");
            _sb.AppendLine(@"\footnotesize (національною, кількість балів, ECTS)");
            _sb.AppendLine(@"\end{flushright}");
        }

        protected override void BuildCommissionMembers(TitlePageData data)
        {
            _sb.AppendLine(@"\vspace{0.9cm}");
            if (data.CommissionMemberNames != null && data.CommissionMemberNames.Any())
            {
                _sb.AppendLine(@"\begin{flushright}");
                _sb.AppendLine(@"\begin{tabular}{l@{}l@{\hspace{0.4cm}}l@{}}");
                _sb.AppendLine(@"  && \\");
                _sb.AppendLine(@"  && \\");
                string signatureLine = @"\makebox[3.5cm]{\hrulefill}";
                string latexSignatureLabel = @"\makebox[3.5cm][c]{\footnotesize(підпис)}";
                for (int i = 0; i < 3; i++)
                {
                    string commissionLabel = (i == 0) ? @"\textbf{Члени комісії:} \hspace{0.4cm}" : "";
                    string memberPii = EscapeLatex(GetText(data.CommissionMemberNames.Count > i ? data.CommissionMemberNames[i] : null, @"\phantom{Прізвище І.П.}"));

                    _sb.Append($@"  {commissionLabel}&{signatureLine} & {memberPii} \\");
                    _sb.Append($@"  &{latexSignatureLabel} & ");
                    _sb.AppendLine((i < 2) ? @"\\[0.8em]" : "");
                }
                _sb.AppendLine(@"\end{tabular}");
                _sb.AppendLine(@"\end{flushright}");
            }
        }

        protected override void BuildFooter(TitlePageData data)
        {
            _sb.AppendLine(@"\vfill");
            _sb.AppendLine(@"\begin{center}");
            _sb.AppendLine($@"{EscapeLatex(GetText(data.City))} – {EscapeLatex(GetText(data.Year.ToString()))} рік");
            _sb.AppendLine(@"\end{center}");
        }

        protected override async Task FinalizeAndPersistOutputAsync(TitlePageData data, string argument)
        {
            _sb.AppendLine(@"\end{document}");
            string filePath = $"{_baseOutputNameWithDir}{this.FileExtension}";
            Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] Saving to file: \"{filePath}\"");
            try
            {
                await File.WriteAllTextAsync(filePath, _sb.ToString(), new UTF8Encoding(false));
                Debug.WriteLine($"[{FormatName}] LaTeX file saved: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{FormatName}] ПОМИЛКА запису LaTeX файлу {filePath}: {ex}");
                throw;
            }
        }
    }
}