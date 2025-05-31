using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq; 

namespace CourseWork_OOP.Services
{
    public class PlainText : BaseTitlePageGenerator
    {
        public override string FormatName => "PlainText";
        public override string FileExtension => ".txt";

        private StringBuilder _context;
        private string _baseOutputNameWithDir;
        private int _pageWidth = 80; 

        public PlainText() { }

        protected override string GetText(string? text, string defaultText = "")
        {
            return text ?? defaultText;
        }

        private string CenterLine(string text, int totalWidth = -1)
        {
            if (totalWidth == -1) totalWidth = _pageWidth;
            if (string.IsNullOrEmpty(text)) return new string(' ', totalWidth);
            int padLeft = (totalWidth - text.Length) / 2;
            return new string(' ', Math.Max(0, padLeft)) + text;
        }
        private string AlignRight(string text, int totalWidth = -1)
        {
            if (totalWidth == -1) totalWidth = _pageWidth;
            if (string.IsNullOrEmpty(text)) return new string(' ', totalWidth);
            return text.PadLeft(totalWidth);
        }
        private string AlignLeft(string text) { return text; } 
        private string Indent(int spaces) => new string(' ', spaces);


        protected override void InitializeInternalContext(TitlePageData data, string argument)
        {
            _context = new StringBuilder();
            _baseOutputNameWithDir = argument;
            Debug.WriteLine($"[{FormatName}.InitializeInternalContext] Context initialized. Base file path: \"{_baseOutputNameWithDir}\"");
        }

        protected override void BuildHeader(TitlePageData data)
        {
            _context.AppendLine(CenterLine("Міністерство освіти і науки України"));
            _context.AppendLine(CenterLine(GetText(data.University))); 
        }

        protected override void BuildFacultyAndDepartment(TitlePageData data)
        {
            _context.AppendLine(Indent(14) + $"Факультет {GetText(data.Faculty)}");
            _context.AppendLine(Indent(14) + $"Кафедра {GetText(data.Department)}");
            _context.AppendLine(); _context.AppendLine(); _context.AppendLine();_context.AppendLine();_context.AppendLine();
        }

        protected override void BuildWorkTitle(TitlePageData data)
        {
            string workType = "КУРСОВА РОБОТА";
            string subjectType = GetText(data.Discipline, "");
            _context.AppendLine(CenterLine(workType));
            _context.AppendLine(CenterLine($"на тему «{GetText(data.Topic)}»"));
            _context.AppendLine(); _context.AppendLine();
        }

        protected override void BuildStudentAndSupervisor(TitlePageData data)
        {
            int rightBlockIndent = 47; 
            string rightIndent = Indent(rightBlockIndent);

            string studentLabel = (GetText(data.Sex) == "Жін") ? "Студентки" : "Студента";
            _context.AppendLine(rightIndent + $"{studentLabel} {GetText(data.CourseNumber, "2")} курсу,");
            _context.AppendLine(rightIndent + $"групи {GetText(data.Group)}");
            _context.AppendLine(rightIndent + $"{GetText(data.SpecialtyName, "спеціальності 121 «Інженерія програмного забезпечення»")}");
            _context.AppendLine(rightIndent + $"{GetText(data.StudentsFullName)}");
            _context.AppendLine(rightIndent + $"(прізвище та ініціали)");
            _context.AppendLine(rightIndent + $"Керівник:  {GetText(data.SuperVisorPosition)}, {GetText(data.SuperVisorFullName)}");
            _context.AppendLine(rightIndent + $"(посада, вчене звання, науковий ступінь, прізвище та ініціали)");
            _context.AppendLine(); _context.AppendLine();
        }

        protected override void BuildEvaluation(TitlePageData data)
        {
            int rightBlockIndent = 47;
            string rightIndent = Indent(rightBlockIndent);
            _context.AppendLine(rightIndent + $"Оцінка за шкалою: {new string('_', 20)}"); 
            _context.AppendLine(rightIndent + $"  (національною, кількість балів, ECTS)");
            _context.AppendLine();
        }

        protected override void BuildCommissionMembers(TitlePageData data)
        {
            int rightBlockIndent = 49;
            string rightIndent = Indent(rightBlockIndent);
            string commissionIndent = Indent(rightBlockIndent + 2);


            string sigLine = "______________"; string piiLinePlaceholder = "________________________"; string spacing = "        "; 
            string subSig = "(підпис)";
            int subSigPad = ((sigLine.Length - subSig.Length) / 2);
            int subSigPad0 = 10;
            string subSigLine0 = new string(' ', Math.Max(0, subSigPad0)) + subSig;
            string subSigLine = new string(' ', Math.Max(0, subSigPad)) + subSig;
            string memberName0 = piiLinePlaceholder;
            if (data.CommissionMemberNames != null && data.CommissionMemberNames.Count > 0 && !string.IsNullOrWhiteSpace(data.CommissionMemberNames[0]))
            {
                memberName0 = GetText(data.CommissionMemberNames[0]).PadRight(piiLinePlaceholder.Length);
            }

            _context.AppendLine((Indent(rightBlockIndent-16)  + $"Члени комісії:    {sigLine}{spacing}{memberName0.Substring(0, Math.Min(memberName0.Length, piiLinePlaceholder.Length))}"));
            _context.AppendLine(Indent(rightBlockIndent-5) + $"{subSigLine0}{new string(' ', spacing.Length + Math.Max(0, (piiLinePlaceholder.Length - subSigLine.Trim().Length)))}");
            for (int i = 1; i < 3; i++)
            {
                string memberName = piiLinePlaceholder;
                if (data.CommissionMemberNames != null && i < data.CommissionMemberNames.Count && !string.IsNullOrWhiteSpace(data.CommissionMemberNames[i]))
                {
                    memberName = GetText(data.CommissionMemberNames[i]).PadRight(piiLinePlaceholder.Length);
                }
                _context.AppendLine(commissionIndent + $"{sigLine}{spacing}{memberName.Substring(0, Math.Min(memberName.Length, piiLinePlaceholder.Length))}");
                _context.AppendLine(commissionIndent + $"{subSigLine}{new string(' ', spacing.Length + Math.Max(0, (piiLinePlaceholder.Length - subSigLine.Trim().Length)))}");
            }
        }

        protected override void BuildFooter(TitlePageData data)
        {
            _context.AppendLine(); _context.AppendLine(); _context.AppendLine();
            _context.AppendLine(CenterLine($"{GetText(data.City)} - {GetText(data.Year.ToString())} рік"));
        }

        protected override async Task FinalizeAndPersistOutputAsync(TitlePageData data, string argument)
        {
            string filePath = $"{_baseOutputNameWithDir}{this.FileExtension}";
            Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] Saving to file: \"{filePath}\"");
            try
            {
                await File.WriteAllTextAsync(filePath, _context.ToString(), new UTF8Encoding(false));
                Debug.WriteLine($"[{FormatName}] PlainText file saved: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{FormatName}] ПОМИЛКА запису PlainText файлу {filePath}: {ex}");
                throw;
            }
        }
    }
}