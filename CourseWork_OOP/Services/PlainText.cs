using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CourseWork_OOP.Services
{
    public class PlainText : BaseTitlePageGenerator 
    {
        public override string FormatName => "PlainText";
        public override string FileExtension => ".txt";

        public PlainText() { }


        private string CenterLine(string text, int totalWidth = 80)
        {
            if (string.IsNullOrEmpty(text)) return new string(' ', totalWidth);
            int padLeft = (totalWidth - text.Length) / 2;
            return new string(' ', Math.Max(0, padLeft)) + text;
        }
        private string Indent(int spaces) => new string(' ', spaces);

        public override async Task GenerateAsync(TitlePageData data, string baseFileName) 
        {
            var context = new StringBuilder();
            int pageWidth = 80;
            int rightBlockIndent = 47; 
            string rightIndent = Indent(rightBlockIndent);
            string commissionIndent = Indent(rightBlockIndent + 2);

            context.AppendLine(CenterLine("Міністерство освіти і науки України", pageWidth));
            context.AppendLine(CenterLine(GetText(data.University), pageWidth)); 
            context.AppendLine();
            context.AppendLine(Indent(14) + $"Факультет {GetText(data.Faculty)}");
            context.AppendLine(Indent(14) + $"Кафедра {GetText(data.Department)}");
            context.AppendLine(); context.AppendLine(); context.AppendLine();
            string workType = "КУРСОВА РОБОТА";
            string subjectType = GetText(data.Discipline, ""); 
            if (subjectType.ToLower().Contains("програмування") && subjectType.ToLower().Contains("об’єктно-орієнтованого")) workType = $"КУРСОВА РОБОТА З ОБ’ЄКТНО-ОРІЄНТОВАНОГО ПРОГРАМУВАННЯ";
            else if (!string.IsNullOrEmpty(subjectType)) workType = $"КУРСОВА РОБОТА З ДИСЦИПЛІНИ «{subjectType}»"; 
            context.AppendLine(CenterLine(workType, pageWidth));
            context.AppendLine(CenterLine($"на тему «{GetText(data.Topic)}»", pageWidth));
            context.AppendLine(); context.AppendLine();
            string studentLabel = (GetText(data.Sex) == "Жін") ? "Студентки" : "Студента";
            context.AppendLine(rightIndent + $"{studentLabel} {GetText(data.CourseNumber, "2")} курсу,");
            context.AppendLine(rightIndent + $"групи {GetText(data.Group)}");
            context.AppendLine(rightIndent + $"{GetText(data.SpecialtyName, "спеціальності 121 «Інженерія програмного забезпечення»")}");
            context.AppendLine();
            context.AppendLine(rightIndent + $"{GetText(data.StudentsFullName)}");
            context.AppendLine(rightIndent + $"(прізвище та ініціали)"); 
            context.AppendLine(); context.AppendLine();
            context.AppendLine(rightIndent + $"Керівник:  {GetText(data.SuperVisorPosition)}, {GetText(data.SuperVisorFullName)}"); 
            context.AppendLine(rightIndent + $"(посада, вчене звання, науковий ступінь, прізвище та ініціали)");
            context.AppendLine(); context.AppendLine();
            context.AppendLine(rightIndent + $"Оцінка за шкалою: {new string('_', 20)}"); 
            context.AppendLine(rightIndent + $"  (національною, кількість балів, ECTS)");
            context.AppendLine();
            context.AppendLine(rightIndent + $"  Члени комісії:");
            context.AppendLine();
            string sigLine = "______________"; string piiLinePlaceholder = "________________________"; string spacing = "        "; 
            string subSig = "(підпис)";
            int subSigPad = (sigLine.Length - subSig.Length) / 2;
            string subSigLine = new string(' ', Math.Max(0, subSigPad)) + subSig;
            for (int i = 0; i < 3; i++)
            {
                string memberName = piiLinePlaceholder; 
                if (data.CommissionMemberNames != null && i < data.CommissionMemberNames.Count && !string.IsNullOrWhiteSpace(data.CommissionMemberNames[i]))
                {
                    memberName = GetText(data.CommissionMemberNames[i]).PadRight(piiLinePlaceholder.Length);
                }
                context.AppendLine(commissionIndent + $"{sigLine}{spacing}{memberName.Substring(0, Math.Min(memberName.Length, piiLinePlaceholder.Length))}");
                context.AppendLine(commissionIndent + $"{subSigLine}{new string(' ', spacing.Length + (piiLinePlaceholder.Length - subSigLine.Trim().Length) / 2)}"); 
                if (i < 2) context.AppendLine();
            }
            context.AppendLine(); context.AppendLine(); context.AppendLine();
            context.AppendLine(CenterLine($"{GetText(data.City)} - {GetText(data.Year.ToString())} рік", pageWidth));

            string filePath = $"{baseFileName}{this.FileExtension}"; 
            try
            {
                await File.WriteAllTextAsync(filePath, context.ToString(), new UTF8Encoding(false));
                Debug.WriteLine($"Plain text file saved: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ПОМИЛКА запису PlainText файлу {filePath}: {ex}");
                throw;
            }
        }
    }
}