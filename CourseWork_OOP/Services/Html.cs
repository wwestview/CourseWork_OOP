using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;

namespace CourseWork_OOP.Services
{
    public class Html : BaseTitlePageGenerator 
    {
        public override string FormatName => "HTML";
        public override string FileExtension => ".html";

        public Html() { }

        private string EncHtml(string? text, string defaultText = "") => HttpUtility.HtmlEncode(GetText(text, defaultText)); 

        public override async Task GenerateAsync(TitlePageData data, string baseOutputNameWithDir) 
        {
            var context = new StringBuilder();
            string nbsp = "&nbsp;";
            context.AppendLine("<!DOCTYPE html>");
            context.AppendLine("<html lang=\"uk\">");
            context.AppendLine("<head>");
            context.AppendLine("  <meta charset=\"UTF-8\">");
            context.AppendLine($"  <title>Титульна сторінка - {EncHtml(GetText(data.Topic, "[Тема роботи]"))}</title>");
            context.AppendLine("  <style>");
            context.AppendLine("    body { margin: 0; padding: 0; background-color: #eee; }");
            context.AppendLine("    .page-wrapper { width: 21cm; min-height: 29.7cm; padding: 2cm 1.5cm 2cm 3cm; margin: 1cm auto; border: 1px solid #ccc; background-color: #fff; box-shadow: 0 0 10px rgba(0,0,0,0.1); box-sizing: border-box; font-family: 'Times New Roman', Times, serif; font-size: 14pt; line-height: 1.5; color: #000; position: relative; }");
            context.AppendLine("    p { margin-top:0; margin-right:0; margin-bottom:4.0pt; margin-left:0; line-height:1.5; font-size: 14pt; font-family: 'Times New Roman', serif; }");
            context.AppendLine("    .centered { text-align: center; }");
            context.AppendLine("    .bold { font-weight: bold; }");
            context.AppendLine("    .large { font-size: 16pt; }"); 
            context.AppendLine("    .small { font-size: 9.0pt; }"); 
            context.AppendLine("    .no-wrap { white-space: nowrap; }");
            context.AppendLine("    .underline-placeholder { border-bottom: 1px solid black; display: inline-block; }");
            context.AppendLine("    .underline-short { min-width: 100px; }");
            context.AppendLine("    .underline-medium { min-width: 160px; }");
            context.AppendLine("    .underline-inline { display: inline-block; border-bottom: 1px solid black; width: 6cm; margin-left: 1em; }");
            context.AppendLine("    .block-header { text-align: center; margin-bottom: 1.5cm; }");
            context.AppendLine("    .block-univer { text-align: center; margin-bottom: 1cm}");
            context.AppendLine("    .block-faculty { margin-bottom: 0.5cm; margin-top:0cm; text-align:center; }");
            context.AppendLine("    .block-title { text-align: center; margin-bottom: 1.5cm; }");
            context.AppendLine("    .right-aligned { margin-left: auto; width: fit-content; margin-right: 0; }");
            context.AppendLine("    .block-executor p, .block-evaluation p, .block-commission p { margin-bottom: 0.2em; }");
            context.AppendLine("    .block-evaluation, .block-commission, .block-executor { margin-top: 0.6cm; margin-bottom: 0.8cm; }");
            context.AppendLine("    .commission-label { display: inline-block; width: 120px; }");
            context.AppendLine("    .commission-line { margin-bottom: 1em !important; white-space: nowrap; height: 1.5em; }");
            context.AppendLine("    .commission-signature-box { display: inline-block; width: 120px; text-align: center; margin-left: 0.5em; }");
            context.AppendLine("    .commission-pii-box { display: inline-block; width: 180px; text-align: center; margin-left: 0.5em; }");
            context.AppendLine("    .commission-sub-label-row { margin-left: 125px; margin-top: -4pt !important; }");
            context.AppendLine("    .commission-sub-label { display: inline-block; text-align: center; font-size: 10pt; }");
            context.AppendLine("    .commission-sub-sig { width: 120px; margin-left: 0.5em; }");
            context.AppendLine("    .commission-sub-pii { width: 180px; margin-left: 0.5em; }");
            context.AppendLine("    .footer { text-align: center; position: absolute; bottom: 2cm; left: 3cm; right: 1.5cm; }");
            context.AppendLine("  </style>");
            context.AppendLine("</head>");
            context.AppendLine("<body>");
            context.AppendLine("  <div class=\"page-wrapper\">");
            context.AppendLine("    <div class=\"block-header\">");
            context.AppendLine($"      <p>Міністерство освіти і науки України</p>");
            context.AppendLine($"      <p class=\"block-univer \">{EncHtml(GetText(data.University, "[Назва університету]"))}</p>");
            context.AppendLine("    </div>");
            context.AppendLine("    <div class=\"block-faculty\" style=\"text-align:center\">");
            context.AppendLine($"      <p>Факультет {EncHtml(GetText(data.Faculty, "[Назва факультету]"))}</p>");
            context.AppendLine($"      <p>Кафедра {EncHtml(GetText(data.Department, "[Назва кафедри]"))}</p>");
            context.AppendLine("    </div>");
            context.AppendLine("    <div class=\"block-title\">");
            string workType;
            string disciplineText = GetText(data.Discipline);
            if (!string.IsNullOrWhiteSpace(disciplineText)) { workType = $"КУРСОВА РОБОТА З ДИСЦИПЛІНИ «{EncHtml(disciplineText)}»"; }
            else { workType = "КУРСОВА РОБОТА"; }
            context.AppendLine($"      <p class=\"large\">{EncHtml(workType)}</p>"); 
            context.AppendLine($"      <p>на тему «{EncHtml(GetText(data.Topic, "[Тема роботи]"))}»</p>");
            context.AppendLine("    </div>");
            context.AppendLine("    <div class=\"block-executor right-aligned\">");
            context.AppendLine("      <div style=\"margin-bottom: 1cm;\">");
            string studentLabel = (GetText(data.Sex) == "Жін") ? "Студентки" : "Студента";
            context.AppendLine($"        <p>{studentLabel} {EncHtml(GetText(data.CourseNumber, "2"))} курсу, групи {EncHtml(GetText(data.Group, "[Група]"))}</p>");
            context.AppendLine($"        <p>{EncHtml(GetText(data.SpecialtyName, "спеціальності 121 «Інженерія програмного забезпечення»"))}</p>");
            context.AppendLine($"        <p class=\"pii-line\" style=\"margin-top: 1em;\">{EncHtml(GetText(data.StudentsFullName, "[ПІБ студента]"))}</p>");
            context.AppendLine("      </div>");
            context.AppendLine("      <div>");
            context.AppendLine($"        <p class=\"no-wrap\">Керівник:{nbsp}{nbsp}{EncHtml(GetText(data.SuperVisorPosition, "[посада]"))},{nbsp}{EncHtml(GetText(data.SuperVisorFullName, "[ПІБ Керівника]"))}</p>");
            context.AppendLine($"        <p class=\"small\" style=\"text-align:right;margin-right: 4.5em;\">(посада, вчене звання, науковий ступінь, прізвище та ініціали)</p>");
            context.AppendLine("      </div>");
            context.AppendLine("    </div>");
            context.AppendLine("    <div class=\"block-evaluation right-aligned\">");
            context.AppendLine("      <p><span>Оцінка за шкалою:</span><span class=\"underline-inline\">&nbsp;</span></p>");
            context.AppendLine("      <p class=\"small\" style=\"text-align:right; margin-right: 1em;\" >(національною, кількість балів, ECTS)</p>");
            context.AppendLine("    </div>");
            context.AppendLine("    <div class=\"block-commission right-aligned\">");
            for (int i = 0; i < 3; i++)
            {
                string memberName = (i < data.CommissionMemberNames?.Count && !string.IsNullOrWhiteSpace(data.CommissionMemberNames[i]))
                                    ? EncHtml(data.CommissionMemberNames[i])
                                    : "<span class=\"underline-placeholder underline-medium\">&nbsp;</span>";
                string commissionLabelText = (i == 0) ? "Члени комісії:" : "&nbsp;";
                context.AppendLine($"      <p class=\"commission-line\"><span class=\"commission-label\">{commissionLabelText}</span><span class=\"commission-signature-box\"><span class=\"underline-placeholder underline-short\">&nbsp;</span></span><span class=\"commission-pii-box\">{memberName}</span></p>");
                string piiSubLabel = memberName.Contains("underline-placeholder") ? "(прізвище та ініціали)" : "";
                context.AppendLine($"      <p class=\"commission-sub-label-row\"><span class=\"commission-signature-box commission-sub-label\">(підпис)</span><span class=\"commission-pii-box commission-sub-label\">{piiSubLabel}</span></p>");
            }
            context.AppendLine("    </div>");
            context.AppendLine("    <div class=\"footer\">");
            context.AppendLine($"      <p>{EncHtml(GetText(data.City, "[Місто]"))} - {EncHtml(GetText(data.Year.ToString(), DateTime.Now.Year.ToString()))} рік</p>");
            context.AppendLine("    </div>");
            context.AppendLine("  </div>");
            context.AppendLine("</body>");
            context.AppendLine("</html>");

            string filePath = $"{baseOutputNameWithDir}{this.FileExtension}"; 
            try
            {
                await File.WriteAllTextAsync(filePath, context.ToString(), Encoding.UTF8);
                Debug.WriteLine($"HTML file saved: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ПОМИЛКА запису HTML файлу {filePath}: {ex}");
                throw;
            }
        }
    }
}