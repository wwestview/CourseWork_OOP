using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web; 
using System.Diagnostics;
using System.Linq; 

namespace CourseWork_OOP.Services
{
    public class Html : BaseTitlePageGenerator
    {
        public override string FormatName => "HTML";
        public override string FileExtension => ".html";

        private StringBuilder _context;
        private string _baseOutputNameWithDir;

        public Html() { }

        private string EncHtml(string? text, string defaultText = "") => HttpUtility.HtmlEncode(GetText(text, defaultText));

        protected override void InitializeInternalContext(TitlePageData data, string argument)
        {
            _context = new StringBuilder();
            _baseOutputNameWithDir = argument;
            Debug.WriteLine($"[{FormatName}.InitializeInternalContext] Context initialized. Base file path: \"{_baseOutputNameWithDir}\"");

            _context.AppendLine("<!DOCTYPE html>");
            _context.AppendLine("<html lang=\"uk\">");
            _context.AppendLine("<head>");
            _context.AppendLine("  <meta charset=\"UTF-8\">");
            _context.AppendLine($"  <title>Титульна сторінка - {EncHtml(GetText(data.Topic, "Генератор титулок"))}</title>"); 
            _context.AppendLine("  <style>");
            _context.AppendLine("    body { margin: 0; padding: 0; background-color: #eee; } .page-wrapper { width: 21cm; min-height: 29.7cm; padding: 2cm 1.5cm 2cm 3cm; margin: 1cm auto; border: 1px solid #ccc; background-color: #fff; box-shadow: 0 0 10px rgba(0,0,0,0.1); box-sizing: border-box; font-family: 'Times New Roman', Times, serif; font-size: 14pt; line-height: 1.5; color: #000; position: relative; } p { margin-top:0; margin-right:0; margin-bottom:4.0pt; margin-left:0; line-height:1.5; font-size: 14pt; font-family: 'Times New Roman', serif; } .centered { text-align: center; } .bold { font-weight: bold; } .large { font-size: 16pt; } .small { font-size: 9.0pt; } .no-wrap { white-space: nowrap; } .underline-placeholder { border-bottom: 1px solid black; display: inline-block; } .underline-short { min-width: 4.5cm; } .underline-medium { min-width: 160px; } .underline-inline { display: inline-block; border-bottom: 1px solid black; width: 6cm; margin-left: 1em; } .block-header { text-align: center; margin-bottom: 0cm; } .block-univer { text-align: center; margin-bottom: 0cm} .block-faculty { margin-bottom: 4cm; margin-top:0cm; text-align:center; } .block-title { text-align: center; margin-bottom: 1.5cm; } .right-aligned { margin-left: auto; width: fit-content; margin-right: 0; } .block-executor p, .block-evaluation p, .block-commission p { margin-bottom: 0.2em; } .block-evaluation, .block-commission, .block-executor { margin-top: 0.6cm; margin-bottom: 0.8cm; } .commission-label { display: inline-block; width: 120px; } .commission-line { margin-bottom: 1em !important; white-space: nowrap; height: 1.5em; } .commission-signature-box { display: inline-block; width: 120px; text-align: center; margin-left: 0.5em; } .commission-pii-box { display: inline-block; width: 180px; text-align: center; margin-left: 0.5em; } .commission-sub-label-row { margin-left: 125px; margin-top: -20pt !important; } .commission-sub-label { display: inline-block; text-align: center;   font-size: 10pt; } .commission-sub-sig { width: 120px; margin-left: 0.5em; } .commission-sub-pii { width: 180px; margin-left: 0.5em; } .footer { text-align: center; position: absolute; bottom: 1cm; left: 3cm; right: 1.5cm; }");
            _context.AppendLine("  </style>");
            _context.AppendLine("</head>");
            _context.AppendLine("<body>");
            _context.AppendLine("  <div class=\"page-wrapper\">");
        }

        protected override void BuildHeader(TitlePageData data)
        {
            _context.AppendLine("    <div class=\"block-header\">");
            _context.AppendLine($"      <p>Міністерство освіти і науки України</p>");
            _context.AppendLine($"      <p class=\"block-univer \">{EncHtml(GetText(data.University, "[Назва університету]"))}</p>");
            _context.AppendLine("    </div>");
        }

        protected override void BuildFacultyAndDepartment(TitlePageData data)
        {
            _context.AppendLine("    <div class=\"block-faculty\" style=\"text-align:center\">");
            _context.AppendLine($"      <p>Факультет {EncHtml(GetText(data.Faculty, "[Назва факультету]"))}</p>");
            _context.AppendLine($"      <p>Кафедра {EncHtml(GetText(data.Department, "[Назва кафедри]"))}</p>");
            _context.AppendLine("    </div>");
        }

        protected override void BuildWorkTitle(TitlePageData data)
        {
            _context.AppendLine("    <div class=\"block-title\">");
            _context.AppendLine($"      <p class=\"large\">КУРСОВА РОБОТА З «{EncHtml(GetText(data.Discipline, "ДИЦСИПЛІНА").ToUpperInvariant())}»</p>");
            _context.AppendLine($"      <p>на тему «{EncHtml(GetText(data.Topic, "..."))}»</p>");
            _context.AppendLine("    </div>");
        }

        protected override void BuildStudentAndSupervisor(TitlePageData data)
        {
            string nbsp = "&nbsp;";
            _context.AppendLine("    <div class=\"block-executor\">"); 
            _context.AppendLine("      <div style=\"margin-bottom: 0cm; margin-left: 8cm;\">"); 
            string studentLabel = (GetText(data.Sex) == "Жін") ? "Студентки" : "Студента";
            _context.AppendLine($"        <p>{studentLabel} {EncHtml(GetText(data.CourseNumber, "X"))} курсу, групи {EncHtml(GetText(data.Group, "XXXX"))}</p>");
            _context.AppendLine($"        <p>спеціальності  {EncHtml(GetText(data.SpecialtyName, "Спеціальність"))}</p>"); 
            _context.AppendLine($"        <p class=\"pii-line\" style=\"margin-top: 0.0cm;\">{EncHtml(GetText(data.StudentsFullName, "ПІБ Студента"))}</p>");
            _context.AppendLine("      </div>");
            _context.AppendLine("      <div>");
            _context.AppendLine($"        <p class=\"no-wrap\" style=\"margin-left: 6cm;\">Керівник:{nbsp}{nbsp}{EncHtml(GetText(data.SuperVisorPosition, "[Посада]"))}, {EncHtml(GetText(data.SuperVisorFullName, "[ПІБ Керівника]"))}</p>");
            _context.AppendLine($"        <p class=\"small\" style=\"text-align:right;margin-right: 0.1cm;\">(посада, вчене звання, науковий ступінь, прізвище та ініціали)</p>");
            _context.AppendLine("      </div>");
            _context.AppendLine("    </div>");
        }

        protected override void BuildEvaluation(TitlePageData data)
        {
            _context.AppendLine("    <div class=\"block-evaluation right-aligned\">");
            _context.AppendLine("      <p><span>Оцінка за шкалою:</span><span class=\"underline-inline\">&nbsp;</span></p>");
            _context.AppendLine("      <p class=\"small\" style=\"text-align:right; margin-right: 1em;\" >(національною, кількість балів, ECTS)</p>");
            _context.AppendLine("    </div>");
        }

        protected override void BuildCommissionMembers(TitlePageData data)
        {
            _context.AppendLine("    <div class=\"block-commission right-aligned\">");
            for (int i = 0; i < 3; i++)
            {
                string memberName;
                if (data.CommissionMemberNames != null && i < data.CommissionMemberNames.Count && !string.IsNullOrWhiteSpace(data.CommissionMemberNames[i]))
                {
                    memberName = EncHtml(GetText(data.CommissionMemberNames[i]));
                }
                else
                {
                    memberName = EncHtml(GetText(null, $"[ПІБ Члена комісії {i + 1}]"));
                }

                string commissionLabelText = (i == 0) ? "Члени комісії:" : "&nbsp;";

                _context.AppendLine($"      <p class=\"commission-line\"><span class=\"commission-label\">{commissionLabelText}</span><span class=\"commission-signature-box\"> <span class=\"underline-placeholder underline-short\">&nbsp;</span></span><span class=\"commission-pii-box\" style=\"margin-left: 1cm;\">{memberName}</span></p>");
                _context.AppendLine($"      <p class=\"commission-sub-label-row\"><span class=\"commission-signature-box commission-sub-label\" style=\"margin-left: 0.76cm;\">(підпис)</span><span class=\"commission-pii-box commission-sub-label\"></span></p>");
                if (i < 2) _context.AppendLine(); 
            }
            _context.AppendLine("    </div>");
        }

        protected override void BuildFooter(TitlePageData data)
        {
            _context.AppendLine("    <div class=\"footer\">");
            _context.AppendLine($"      <p>{EncHtml(GetText(data.City, "Місто"))} - {EncHtml(GetText(data.Year.ToString(), DateTime.Now.Year.ToString()))} рік</p>");
            _context.AppendLine("    </div>");
        }

        protected override async Task FinalizeAndPersistOutputAsync(TitlePageData data, string argument)
        {
            _context.AppendLine("  </div>");
            _context.AppendLine("</body>");
            _context.AppendLine("</html>");

            string filePath = $"{_baseOutputNameWithDir}{this.FileExtension}";
            Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] Saving to file: \"{filePath}\"");
            try
            {
                await File.WriteAllTextAsync(filePath, _context.ToString(), Encoding.UTF8);
                Debug.WriteLine($"[{FormatName}] HTML file saved: {Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{FormatName}] ПОМИЛКА запису HTML файлу {filePath}: {ex}");
                throw;
            }
        }
    }
}
