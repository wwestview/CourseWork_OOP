using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CourseWork_OOP.Services
{
    public class PlainText : CoverPageGenerator
    {
        public PlainText(CoverPageData data) : base(data)
        {

        }
        private string GetText(string? text, string defaultText = "") => text ?? defaultText;

        private string CenterLine(string text, int totalWidth, string pageIndent)
        {
            if (string.IsNullOrEmpty(text)) return pageIndent;
            int textWidth = text.Length;
            // Доступна ширина після основного відступу
            int availableWidth = totalWidth - pageIndent.Length;
            if (availableWidth < 0) availableWidth = 0; 
            int padLeft = (availableWidth - textWidth) / 2;
            if (padLeft < 0) padLeft = 0; 
            return pageIndent + new string(' ', padLeft) + text;
        }

        private string IndentLine(string text, string pageIndent)
        {
            return pageIndent + text;
        }

        public override async Task GenerateAsync(string baseFileName)
        {
            var context = new StringBuilder();
            int consoleWidth = 80; 

            string pageIndent = new string(' ', 10);

            context.AppendLine(CenterLine("Міністерство освіти і науки України", consoleWidth, pageIndent));
            context.AppendLine(CenterLine(GetText(Data.University, "Назва Університету"), consoleWidth, pageIndent));
            context.AppendLine(pageIndent); 
            context.AppendLine(pageIndent);

            int labelWidthFacultyDept = 15;
            context.AppendLine(IndentLine($"{"Факультет".PadRight(labelWidthFacultyDept)}{GetText(Data.Faculty, "Назва Факультету")}", pageIndent));
            context.AppendLine(IndentLine($"{"Кафедра".PadRight(labelWidthFacultyDept)}{GetText(Data.Department, "Назва Кафедри")}", pageIndent));
            context.AppendLine(pageIndent);
            context.AppendLine(pageIndent);
            context.AppendLine(pageIndent);

            context.AppendLine(CenterLine("Курсова Робота", consoleWidth, pageIndent));
            context.AppendLine(CenterLine($"по дисципліні «{GetText(Data.Discipline, "Об'єкто-орієнтоване програмування")}»", consoleWidth, pageIndent));
<<<<<<< HEAD
            context.AppendLine(CenterLine($"На тему: «{GetText(Data.Topic, "Тема роботи")}»", consoleWidth, pageIndent)); // ДОДАНО
=======
            context.AppendLine(CenterLine($"На тему: «{GetText(Data.Topic, "Тема роботи")}»", consoleWidth, pageIndent)); 
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
            context.AppendLine(pageIndent);
            context.AppendLine(pageIndent);

            string internalBlockIndent = new string(' ', 25); 
            int internalLabelWidth = 12; 
            string internalNameIndent = internalBlockIndent + new string(' ', internalLabelWidth); 

            // Блок виконавця
            context.Append(pageIndent + internalBlockIndent);
            context.Append("Виконав:".PadRight(internalLabelWidth));
<<<<<<< HEAD
            // !!! Перевірте назву властивості 'Group' !!!
            context.AppendLine($"студент гр. {GetText(Data.Group, "Група")}");

            context.Append(pageIndent + internalNameIndent);
            // !!! Перевірте назву властивості 'StudentsFullName' !!!
=======
            context.AppendLine($"студент гр. {GetText(Data.Group, "Група")}");

            context.Append(pageIndent + internalNameIndent);
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
            context.AppendLine(GetText(Data.StudentsFullName, "ПІБ Студента"));

            context.AppendLine(pageIndent); 

            context.Append(pageIndent + internalBlockIndent);
            context.Append("Перевірив:".PadRight(internalLabelWidth));
            context.AppendLine(GetText(Data.SuperVisorPosition, "Посада"));

            context.Append(pageIndent + internalNameIndent);
            context.AppendLine(GetText(Data.SuperVisorFullName, "ПІБ Керівника"));
            context.AppendLine(pageIndent);
            context.AppendLine(pageIndent);
            context.AppendLine(pageIndent);

            context.AppendLine(CenterLine($"{GetText(Data.City, "Місто")}, {Data.Year}", consoleWidth, pageIndent));

            string filePath = $"{baseFileName}.txt";
            try
            {
                var utf8WithoutBom = new UTF8Encoding(false);
                await File.WriteAllTextAsync(filePath, context.ToString(), utf8WithoutBom);
                System.Diagnostics.Debug.WriteLine($"Plain text file saved: {filePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ПОМИЛКА запису файлу {filePath}: {ex.ToString()}");
                throw;
            }
        }
    }
}
