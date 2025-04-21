// File: MainWindowViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
// Переконайтесь, що ці using вірні для вашого проєкту та класів
using CourseWork_OOP.Services; // Припускаємо, що тут генератори (LaTeX, Html, PlainText) та базовий клас (CoverPageGenerator)
using System;
using System.Collections.Generic;
using System.Diagnostics;       // Для Debug.WriteLine
using System.IO;                // Для Path, Directory
using System.Threading.Tasks;   // Для Task (асинхронність)
using System.Windows;           // Для MessageBox

namespace CourseWork_OOP // Перевірте namespace
{
    public partial class MainWindowViewModel : ObservableObject
    {
        // --- Властивості ViewModel ---
        [ObservableProperty] private string university = "Назва Університету";
        [ObservableProperty] private string faculty = "Назва Факультету";
        [ObservableProperty] private string department = "Назва Кафедри";
        [ObservableProperty] private string discipline = "Програмування";
        [ObservableProperty] private string topic = "Тема курсової роботи";
        [ObservableProperty] private string studentFullName = "Прізвище Ім'я По-батькові";
        [ObservableProperty] private string studentGroup = "Група";
        [ObservableProperty] private string supervisorFullName = "Прізвище І.П. Керівника";
        [ObservableProperty] private string supervisorPosition = "Посада Керівника";
        [ObservableProperty] private string city = "Черкаси"; // м. Черкаси, Черкаська область, Україна
        [ObservableProperty] private int year = DateTime.Now.Year;

        [ObservableProperty] private string statusMessage = "Готово";

        [RelayCommand]
        private async Task GenerateCoverPagesAsync()
        {
            // Діагностичне повідомлення про старт
            System.Diagnostics.Debug.WriteLine($"--- GenerateCoverPagesAsync ЗАПУЩЕНО о {DateTime.Now:HH:mm:ss.fff} ---");

            StatusMessage = "Підготовка до генерації..."; // Оновлюємо статус

            try
            {
                // 1. Збираємо дані з ПУБЛІЧНИХ властивостей ViewModel
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

                // 2. Папка
                string outputDirectory = Path.Combine(AppContext.BaseDirectory, "Generated_TitlePages");
                Directory.CreateDirectory(outputDirectory);

                // 3. Список генераторів
                List<CoverPageGenerator> generators = new List<CoverPageGenerator>();
                try
                {
                    generators.Add(new LaTeX(coverData));
                    generators.Add(new Html(coverData));
                    generators.Add(new PlainText(coverData));
                    generators.Add(new Docs(coverData)); // Додайте реалізовані генератори
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Помилка при створенні генераторів: {ex.Message}";
                    Debug.WriteLine($"Помилка ініціалізації списку генераторів: {ex.ToString()}");
                    return; // finally виконається
                }
                if (generators.Count == 0) { StatusMessage = "Немає генераторів."; return; } // finally виконається

                StatusMessage = $"Починаю генерацію у папку: {outputDirectory}";
                List<string> errors = new List<string>();

                // 4. Цикл генерації
                foreach (var generator in generators)
                {

                    string formatName = "Unknown";
                    string fileExtension = "tmp";

                    // Визначаємо формат та розширення
                    if (generator is LaTeX) { formatName = "LaTeX"; fileExtension = "tex"; }
                    else if (generator is Html) { formatName = "Html"; fileExtension = "html"; }
                    else if (generator is PlainText) { formatName = "PlainText"; fileExtension = "txt"; }
                     else if (generator is Docs) { formatName = "GoogleDoc"; fileExtension = ""; }

                    // Формуємо ім'я файлу / заголовок
                    string safeStudentName = string.Join("_", coverData.StudentsFullName?.Split(Path.GetInvalidFileNameChars()) ?? Array.Empty<string>());
                    string baseFileName = $"Титулка_{safeStudentName}_{formatName}_{DateTime.Now:yyyyMMddHHmmss}"; // Назва без розширення

                    // Визначаємо аргумент для GenerateAsync
                    string argumentForGenerateAsync = (formatName == "GoogleDoc")
                        ? baseFileName // Для Google Doc передаємо лише назву
                        : Path.Combine(outputDirectory, baseFileName); // Для інших - шлях без розширення

                    StatusMessage = $"Генерація {formatName.ToUpper()}...";
                    Debug.WriteLine($"Запуск генерації для {formatName} з аргументом: {argumentForGenerateAsync}");

                    try
                    {
                        // Викликаємо GenerateAsync
                        await generator.GenerateAsync(argumentForGenerateAsync);
                        Debug.WriteLine($"{formatName.ToUpper()} згенеровано успішно.");
                    }
                    catch (NotImplementedException) { string msg = $"Метод GenerateAsync не реалізовано для {formatName}"; Debug.WriteLine(msg); errors.Add(msg); }
                    catch (Exception ex) { string msg = $"Помилка {formatName}: {ex.Message}"; Debug.WriteLine(msg + $"\n{ex.StackTrace}"); errors.Add(msg); } // Додав StackTrace у Debug

                } // Кінець циклу foreach

                // Діагностичне повідомлення про кінець try блоку
                System.Diagnostics.Debug.WriteLine($"--- GenerateCoverPagesAsync ЗАВЕРШИВ try блок о {DateTime.Now:HH:mm:ss.fff} ---");

                // 5. Повідомляємо про результат
                if (errors.Count == 0) { StatusMessage = $"Успішно згенеровано {generators.Count} файл(ів) у папку: {outputDirectory}"; }
                else { StatusMessage = $"Генерація завершена з {errors.Count} помилками."; }
            }
            catch (Exception ex) // Обробка загальних помилок
            {
                StatusMessage = $"Критична помилка: {ex.Message}";
                Debug.WriteLine($"Критична помилка у GenerateCoverPagesAsync: {ex.ToString()}");
                MessageBox.Show($"Сталася неочікувана помилка: {ex.Message}", "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
            }
        } // Кінець методу GenerateCoverPagesAsync
    } // Кінець класу MainWindowViewModel
} // Кінець простору імен