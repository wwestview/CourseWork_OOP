using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using CourseWork_OOP;

namespace CourseWork_OOP.Services
{
    public class Html : CoverPageGenerator
    {
        public Html(CoverPageData data) : base(data)
        {

        }
        private string EncHtml(string? text, string defaultText = "") => HttpUtility.HtmlEncode(text ?? defaultText);

        public override async Task GenerateAsync(string baseFileName)
        {
            var context = new StringBuilder();

            context.AppendLine("<!DOCTYPE html>");
            context.AppendLine("<html lang=\"uk\">");
            context.AppendLine("<head>");
            context.AppendLine("  <meta charset=\"UTF-8\">");
            context.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            context.AppendLine($"  <title>Титульна сторінка - {EncHtml(Data.Discipline, "Назва дисципліни")}</title>");
            context.AppendLine("  <style>");
            context.AppendLine("    body { margin: 0; padding: 0; background-color: #eee; }");
            context.AppendLine("    .page-wrapper { width: 21cm; min-height: 29.7cm; padding: 2cm 1.5cm 2cm 3cm; margin: 1cm auto; border: 1px solid #ccc; background-color: #fff; box-shadow: 0 0 10px rgba(0,0,0,0.1); box-sizing: border-box; font-family: 'Times New Roman', Times, serif; font-size: 14pt; line-height: 1.5; color: #000; }");
            context.AppendLine("    p { margin: 0; padding: 0; }");
            context.AppendLine("    .center { text-align: center; }");
            context.AppendLine("    .bold { font-weight: bold; }");
            context.AppendLine("    .header { text-align: center; line-height: 1.3; margin-bottom: 2.5cm; }");
            context.AppendLine("    .header .university { font-weight: bold; }");
            context.AppendLine("    .faculty-dept { margin-bottom: 5cm; line-height: 1.4; }"); 
            context.AppendLine("    .faculty-dept .line { margin-bottom: 0.3em; }");
            context.AppendLine("    .faculty-dept .label { display: inline-block; min-width: 90px; font-weight: normal; overflow-wrap: break-word; word-wrap: break-word; }");
            context.AppendLine("    .faculty-dept .value { display: inline-block; margin-left: 1em; overflow-wrap: break-word; word-wrap: break-word; }");
            context.AppendLine("    .work-title-line { text-align: center; margin-bottom: 5cm; }"); 
            context.AppendLine("    .work-title-line .work-type { font-size: 16pt; font-weight: bold; margin-bottom: 0.5em; overflow-wrap: break-word; word-wrap: break-word; }");
            context.AppendLine("    .work-title-line .discipline-name { font-size: 14pt; font-weight: normal; margin-bottom: 0.5em; overflow-wrap: break-word; word-wrap: break-word; }"); // Додав відступ
            context.AppendLine("    .work-title-line .topic-name { font-size: 14pt; font-weight: normal; overflow-wrap: break-word; word-wrap: break-word; }"); // Стиль для теми
            context.AppendLine("    .executor-supervisor-block { margin-left: auto; margin-right: 0; width: 55%; margin-top: 4cm; line-height: 1.3; text-align: left; }");
            context.AppendLine("    .executor-supervisor-block p { margin: 0 0 0.2em 0; }");
            context.AppendLine("    .executor-supervisor-block .line1 .label { display: inline-block; width: 100px; overflow-wrap: break-word; word-wrap: break-word; }");
            context.AppendLine("    .executor-supervisor-block .line1 .info { display: inline-block; margin-left: 10px; overflow-wrap: break-word; word-wrap: break-word; }");
            context.AppendLine("    .executor-supervisor-block .line2 .name { display: block; margin-left: 110px; overflow-wrap: break-word; word-wrap: break-word; }");
            context.AppendLine("    .executor-supervisor-block .supervisor-details { margin-top: 1.5cm; }");
            context.AppendLine("    .footer { text-align: center; margin-top: 6cm; }"); 
            context.AppendLine("    .footer p { overflow-wrap: break-word; word-wrap: break-word; }");
            context.AppendLine("  </style>");
            context.AppendLine("</head>");
            context.AppendLine("<body>");
            context.AppendLine("  <div class=\"page-wrapper\">"); 

            context.AppendLine("    <div class=\"header\">");
            context.AppendLine($"      <p>Міністерство освіти і науки України</p>");
            context.AppendLine($"      <p class=\"university\">{EncHtml(Data.University, "Назва Університету")}</p>");
            context.AppendLine("    </div>");

            context.AppendLine("    <div class=\"faculty-dept\">");
            context.AppendLine($"      <p class=\"line\"><span class=\"label\">Факультет</span><span class=\"value\">{EncHtml(Data.Faculty, "Назва Факультету")}</span></p>");
            context.AppendLine($"      <p class=\"line\"><span class=\"label\">Кафедра</span><span class=\"value\">{EncHtml(Data.Department, "Назва Кафедри")}</span></p>");
            context.AppendLine("    </div>");

            context.AppendLine("    <div class=\"work-title-line\">");
            context.AppendLine($"      <p class=\"work-type\">Курсова Робота</p>");
            context.AppendLine($"      <p class=\"discipline-name\">по дисципліні &laquo;{EncHtml(Data.Discipline, "Назва дисципліни")}&raquo;</p>");
            context.AppendLine($"      <p class=\"topic-name\">На тему: &laquo;{EncHtml(Data.Topic, "Тема роботи")}&raquo;</p>"); // ДОДАНО
            context.AppendLine("    </div>");

            context.AppendLine("    <div class=\"executor-supervisor-block\">");
            context.AppendLine("        <div class=\"student-details\">");
            context.AppendLine("            <p class=\"line1\">");
            context.AppendLine("                <span class=\"label\">Виконав:</span>");
            context.AppendLine($"                <span class=\"info\">студент гр. {EncHtml(Data.Group, "Група")}</span>");
            context.AppendLine("            </p>");
            context.AppendLine("            <p class=\"line2\">");
            context.AppendLine($"                <span class=\"name\">{EncHtml(Data.StudentsFullName, "ПІБ Студента")}</span>");
            context.AppendLine("            </p>");
            context.AppendLine("        </div>");
            context.AppendLine("        <div class=\"supervisor-details\">");
            context.AppendLine("            <p class=\"line1\">");
            context.AppendLine("                <span class=\"label\">Перевірив:</span>");
            context.AppendLine($"                <span class=\"info\">{EncHtml(Data.SuperVisorPosition, "Посада")}</span>");
            context.AppendLine("            </p>");
            context.AppendLine("            <p class=\"line2\">");
            context.AppendLine($"                <span class=\"name\">{EncHtml(Data.SuperVisorFullName, "ПІБ Керівника")}</span>");
            context.AppendLine("            </p>");
            context.AppendLine("        </div>");
            context.AppendLine("    </div>");

            // --- Низ ---
            context.AppendLine("    <div class=\"footer\">");
            context.AppendLine($"      <p>{EncHtml(Data.City, "Місто")}, {Data.Year}</p>");
            context.AppendLine("    </div>");

            context.AppendLine("  </div>"); 
            context.AppendLine("</body>");
            context.AppendLine("</html>");

            string filePath = $"{baseFileName}.html";
            try
            {
                await File.WriteAllTextAsync(filePath, context.ToString(), Encoding.UTF8);
                System.Diagnostics.Debug.WriteLine($"HTML file saved: {filePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ПОМИЛКА запису файлу {filePath}: {ex.ToString()}");
                throw;
            }
        }
    }
}
