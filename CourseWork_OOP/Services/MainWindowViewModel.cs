using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseWork_OOP.Services; 
using System;
using System.Collections.Generic;
using System.Diagnostics;       
using System.IO;                
using System.Threading.Tasks;   
using System.Windows;           

namespace CourseWork_OOP 
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty] private string university = "Назва Університету";
        [ObservableProperty] private string faculty = "Назва Факультету";
        [ObservableProperty] private string department = "Назва Кафедри";
        [ObservableProperty] private string discipline = "Програмування";
        [ObservableProperty] private string topic = "Тема курсової роботи";
        [ObservableProperty] private string studentFullName = "Прізвище Ім'я По-батькові";
        [ObservableProperty] private string studentGroup = "Група";
        [ObservableProperty] private string supervisorFullName = "Прізвище І.П. Керівника";
        [ObservableProperty] private string supervisorPosition = "Посада Керівника";
        [ObservableProperty] private string city = "Черкаси"; 
        [ObservableProperty] private int year = DateTime.Now.Year;

        [ObservableProperty] private string statusMessage = "Готово";

        [RelayCommand]
        private async Task GenerateCoverPagesAsync()
        {
            System.Diagnostics.Debug.WriteLine($"--- GenerateCoverPagesAsync ЗАПУЩЕНО о {DateTime.Now:HH:mm:ss.fff} ---");

            StatusMessage = "Підготовка до генерації..."; 

            try
            {
                CoverPageData coverData = new CoverPageData
                {
                    University = this.University,
                    Faculty = this.Faculty,
                    Department = this.Department,
                    Discipline = this.Discipline,
                    Topic = this.Topic,
                    StudentsFullName = this.StudentFullName,
                    Group = this.StudentGroup,
                    SuperVisorFullName = this.SupervisorFullName,
                    SuperVisorPosition = this.SupervisorPosition,
                    City = this.City,
                    Year = this.Year
                };

                string outputDirectory = Path.Combine(AppContext.BaseDirectory, "Generated_TitlePages");
                Directory.CreateDirectory(outputDirectory);

                List<CoverPageGenerator> generators = new List<CoverPageGenerator>();
                try
                {
                    generators.Add(new LaTeX(coverData));
                    generators.Add(new Html(coverData));
                    generators.Add(new PlainText(coverData));
                    generators.Add(new Docs(coverData)); 
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Помилка при створенні генераторів: {ex.Message}";
                    Debug.WriteLine($"Помилка ініціалізації списку генераторів: {ex.ToString()}");
                    return;
                }
                if (generators.Count == 0) { StatusMessage = "Немає генераторів."; return; } 

                StatusMessage = $"Починаю генерацію у папку: {outputDirectory}";
                List<string> errors = new List<string>();

                foreach (var generator in generators)
                {

                    string formatName = "Unknown";
                    string fileExtension = "tmp";

                    if (generator is LaTeX) { formatName = "LaTeX"; fileExtension = "tex"; }
                    else if (generator is Html) { formatName = "Html"; fileExtension = "html"; }
                    else if (generator is PlainText) { formatName = "PlainText"; fileExtension = "txt"; }
                     else if (generator is Docs) { formatName = "GoogleDoc"; fileExtension = ""; }

                    string safeStudentName = string.Join("_", coverData.StudentsFullName?.Split(Path.GetInvalidFileNameChars()) ?? Array.Empty<string>());
                    string baseFileName = $"Титулка_{safeStudentName}_{formatName}_{DateTime.Now:yyyyMMddHHmmss}"; 

                    string argumentForGenerateAsync = (formatName == "GoogleDoc")
                        ? baseFileName 
                        : Path.Combine(outputDirectory, baseFileName); 

                    StatusMessage = $"Генерація {formatName.ToUpper()}...";
                    Debug.WriteLine($"Запуск генерації для {formatName} з аргументом: {argumentForGenerateAsync}");

                    try
                    {
                        await generator.GenerateAsync(argumentForGenerateAsync);
                        Debug.WriteLine($"{formatName.ToUpper()} згенеровано успішно.");
                    }
                    catch (NotImplementedException) { string msg = $"Метод GenerateAsync не реалізовано для {formatName}"; Debug.WriteLine(msg); errors.Add(msg); }
                    catch (Exception ex) { string msg = $"Помилка {formatName}: {ex.Message}"; Debug.WriteLine(msg + $"\n{ex.StackTrace}"); errors.Add(msg); } 

                } 

                System.Diagnostics.Debug.WriteLine($"--- GenerateCoverPagesAsync ЗАВЕРШИВ try блок о {DateTime.Now:HH:mm:ss.fff} ---");

                if (errors.Count == 0) { StatusMessage = $"Успішно згенеровано {generators.Count} файл(ів) у папку: {outputDirectory}"; }
                else { StatusMessage = $"Генерація завершена з {errors.Count} помилками."; }
            }
            catch (Exception ex) 
            {
                StatusMessage = $"Критична помилка: {ex.Message}";
                Debug.WriteLine($"Критична помилка у GenerateCoverPagesAsync: {ex.ToString()}");
                MessageBox.Show($"Сталася неочікувана помилка: {ex.Message}", "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
            }
        } 
    } 
} 