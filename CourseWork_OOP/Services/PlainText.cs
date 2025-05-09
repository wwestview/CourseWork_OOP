using CourseWork_OOP.Interfaces;
using CourseWork_OOP.Services;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork_OOP.Services
{
    public class PlainText : ITitlePageGenerator
    {
        public PlainText() { }

        private string GetText(string? text, string defaultText = "") => text ?? defaultText;
        private string CenterLine(string text, int totalWidth = 80) { if (string.IsNullOrEmpty(text)) return ""; int padLeft = (totalWidth - text.Length) / 2; return new string(' ', Math.Max(0, padLeft)) + text; }
        private string Indent(int spaces) => new string(' ', spaces);

        public async Task GenerateAsync(TitlePageData data, string argument)
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
            string subjectType = data.Discipline ?? "";
            if (subjectType.ToLower().Contains("програмування")) workType = $"КУРСОВА РОБОТА З ОБ’ЄКТНО-ОРІЄНТОВАНОГО ПРОГРАМУВАННЯ";
            else if (!string.IsNullOrEmpty(subjectType)) workType = $"КУРСОВА РОБОТА З ДИСЦИПЛІНИ «{GetText(data.Discipline)}»";
            context.AppendLine(CenterLine(workType, pageWidth));
            context.AppendLine(CenterLine($"на тему «{GetText(data.Topic)}»", pageWidth));
            context.AppendLine(); context.AppendLine();

            string studentLabel = (data.Sex == "Жін") ? "Студентки" : "Студента";
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

            context.AppendLine(rightIndent + $"Оцінка за шкалою: ____________________");
            context.AppendLine(rightIndent + $"  (національною, кількість балів, ECTS)");
            context.AppendLine();

            context.AppendLine(rightIndent + $"  Члени комісії:");
            context.AppendLine();

            string sigLine = "______________"; string piiLinePlaceholder = "________________________"; string spacing = "       ";
            string subSig = "(підпис)"; string subPii = "(прізвище та ініціали)";
            int subSigPad = (sigLine.Length - subSig.Length) / 2; int subPiiPad = (piiLinePlaceholder.Length - subPii.Length) / 2;
            string subSigLine = new string(' ', Math.Max(0, subSigPad)) + subSig;
            string subPiiLine = new string(' ', Math.Max(0, subPiiPad)) + subPii;

            for (int i = 0; i < 3; i++)
            {
                string memberName = (i < data.CommissionMemberNames?.Count && !string.IsNullOrWhiteSpace(data.CommissionMemberNames[i]))
                                    ? GetText(data.CommissionMemberNames[i]).PadRight(piiLinePlaceholder.Length)
                                    : piiLinePlaceholder;
                context.AppendLine(commissionIndent + $"{sigLine}{spacing}{memberName.Substring(0, Math.Min(memberName.Length, piiLinePlaceholder.Length))}");
                context.AppendLine(commissionIndent + $"{subSigLine}{spacing}{subPiiLine}");
                if (i < 2) context.AppendLine();
            }

            context.AppendLine(); context.AppendLine(); context.AppendLine();
            context.AppendLine(CenterLine($"{GetText(data.City)} - {data.Year} рік", pageWidth));

            string filePath = $"{argument}.txt";
            try { await File.WriteAllTextAsync(filePath, context.ToString(), new UTF8Encoding(false)); System.Diagnostics.Debug.WriteLine($"Plain text saved: {filePath}"); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"ПОМИЛКА запису файлу {filePath}: {ex.ToString()}"); throw; }
        }
    }
}