using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseWork_OOP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
    public class CoverPageData
    {
        public string? StudentsFullName { get; set; }
        public string? SuperVisorFullName { get; set; }
        public string? SuperVisorPosition { get; set; }
        public string? Group {  get; set; }
        public string? Topic { get; set; }
        public string? Discipline { get; set; }
        public string? University { get; set; }
        public string? Faculty { get; set; }
        public string? Department { get; set; }
        public string? City { get; set; } = "Черкаси";
        public int Year { get; set; } = DateTime.Now.Year;
    }
    public abstract class CoverPageGenerator
    {
        protected CoverPageData Data { get; }

        protected CoverPageGenerator(CoverPageData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
        public abstract Task GenerateAsync(string baseFileName);
    }
    public class PlainText : CoverPageGenerator
    {
        public PlainText(CoverPageData data) : base(data) 
        {

        }
        public override async Task GenerateAsync(string baseFileName)
        {
            var context = new StringBuilder();
            context.AppendLine($"Міністерство освіти і науки України");
            context.AppendLine(Data.University ?? "Назва Університету");
            context.AppendLine(Data.Faculty ?? "Назва Факультету");
            context.AppendLine(Data.Department ?? "Назва Кафедри");
            context.AppendLine();
            context.AppendLine();

            context.AppendLine($"КУРСОВА РОБОТА");
            context.AppendLine($"з дисципліни \"{Data.Discipline ?? "Назва дисципліни"}\"");
            context.AppendLine($"на тему: \"{Data.Topic ?? "Тема роботи"}\"");
            context.AppendLine();
            context.AppendLine();
            context.AppendLine();
            string studentLine = $"Виконав(ла): студент(ка) групи {Data.Group ?? "Група"}";
            string studentNameLine = $"{Data.StudentsFullName ?? "ПІБ Студента"}";
            string supervisorLine = $"Керівник: {Data.SuperVisorPosition ?? "Посада"}";
            string supervisorNameLine = $"{Data.SuperVisorFullName ?? "ПІБ Керівника"}";
            int consoleWidth = 80;
            context.AppendLine(studentLine.PadLeft(consoleWidth));
            context.AppendLine(studentNameLine.PadLeft(consoleWidth));
            context.AppendLine();
            context.AppendLine(supervisorLine.PadLeft(consoleWidth));
            context.AppendLine(supervisorNameLine.PadLeft(consoleWidth));

            context.AppendLine();
            context.AppendLine();
            context.AppendLine();

            context.AppendLine($"{Data.City ?? "Місто"} , {Data.Year}");
            string filePath = $"{baseFileName}.txt";
            try
            {
                await File.WriteAllTextAsync(filePath, context.ToString(), Encoding.UTF8);
                Console.WriteLine($"Plain text file saved: {filePath}"); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving plain text file {filePath}: {ex.Message}");
            }
        }
    }
    public class Html : CoverPageGenerator
    {
        public Html(CoverPageData data) : base(data)
        {

        }
        public override async Task GenerateAsync(string baseFileName)
        {
            var context = new StringBuilder();
        }
    }
}