<<<<<<< HEAD
﻿// File: MainWindowViewModel.cs
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
=======
﻿using CommunityToolkit.Mvvm.ComponentModel;
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
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
        [ObservableProperty] private string university = "Назва Університету";
        [ObservableProperty] private string faculty = "Назва Факультету";
        [ObservableProperty] private string department = "Назва Кафедри";
        [ObservableProperty] private string discipline = "Програмування";
        [ObservableProperty] private string topic = "Тема курсової роботи";
        [ObservableProperty] private string studentFullName = "Прізвище Ім'я По-батькові";
        [ObservableProperty] private string studentGroup = "Група";
        [ObservableProperty] private string supervisorFullName = "Прізвище І.П. Керівника";
        [ObservableProperty] private string supervisorPosition = "Посада Керівника";
<<<<<<< HEAD
        [ObservableProperty] private string city = "Черкаси"; // м. Черкаси, Черкаська область, Україна
=======
        [ObservableProperty] private string city = "Черкаси"; 
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
        [ObservableProperty] private int year = DateTime.Now.Year;

        [ObservableProperty] private string statusMessage = "Готово";

        [RelayCommand]
        private async Task GenerateCoverPagesAsync()
        {
<<<<<<< HEAD
            // Діагностичне повідомлення про старт
            System.Diagnostics.Debug.WriteLine($"--- GenerateCoverPagesAsync ЗАПУЩЕНО о {DateTime.Now:HH:mm:ss.fff} ---");

            StatusMessage = "Підготовка до генерації..."; // Оновлюємо статус

            try
            {
                // 1. Збираємо дані з ПУБЛІЧНИХ властивостей ViewModel
=======
            System.Diagnostics.Debug.WriteLine($"--- GenerateCoverPagesAsync ЗАПУЩЕНО о {DateTime.Now:HH:mm:ss.fff} ---");

            StatusMessage = "Підготовка до генерації..."; 

            try
            {
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
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

<<<<<<< HEAD
                // 2. Папка
                string outputDirectory = Path.Combine(AppContext.BaseDirectory, "Generated_TitlePages");
                Directory.CreateDirectory(outputDirectory);

                // 3. Список генераторів
=======
                string outputDirectory = Path.Combine(AppContext.BaseDirectory, "Generated_TitlePages");
                Directory.CreateDirectory(outputDirectory);

>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
                List<CoverPageGenerator> generators = new List<CoverPageGenerator>();
                try
                {
                    generators.Add(new LaTeX(coverData));
                    generators.Add(new Html(coverData));
                    generators.Add(new PlainText(coverData));
<<<<<<< HEAD
                    generators.Add(new Docs(coverData)); // Додайте реалізовані генератори
=======
                    generators.Add(new Docs(coverData)); 
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Помилка при створенні генераторів: {ex.Message}";
                    Debug.WriteLine($"Помилка ініціалізації списку генераторів: {ex.ToString()}");
<<<<<<< HEAD
                    return; // finally виконається
                }
                if (generators.Count == 0) { StatusMessage = "Немає генераторів."; return; } // finally виконається
=======
                    return;
                }
                if (generators.Count == 0) { StatusMessage = "Немає генераторів."; return; } 
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d

                StatusMessage = $"Починаю генерацію у папку: {outputDirectory}";
                List<string> errors = new List<string>();

<<<<<<< HEAD
                // 4. Цикл генерації
=======
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
                foreach (var generator in generators)
                {

                    string formatName = "Unknown";
                    string fileExtension = "tmp";

<<<<<<< HEAD
                    // Визначаємо формат та розширення
=======
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
                    if (generator is LaTeX) { formatName = "LaTeX"; fileExtension = "tex"; }
                    else if (generator is Html) { formatName = "Html"; fileExtension = "html"; }
                    else if (generator is PlainText) { formatName = "PlainText"; fileExtension = "txt"; }
                     else if (generator is Docs) { formatName = "GoogleDoc"; fileExtension = ""; }

<<<<<<< HEAD
                    // Формуємо ім'я файлу / заголовок
                    string safeStudentName = string.Join("_", coverData.StudentsFullName?.Split(Path.GetInvalidFileNameChars()) ?? Array.Empty<string>());
                    string baseFileName = $"Титулка_{safeStudentName}_{formatName}_{DateTime.Now:yyyyMMddHHmmss}"; // Назва без розширення

                    // Визначаємо аргумент для GenerateAsync
                    string argumentForGenerateAsync = (formatName == "GoogleDoc")
                        ? baseFileName // Для Google Doc передаємо лише назву
                        : Path.Combine(outputDirectory, baseFileName); // Для інших - шлях без розширення
=======
                    string safeStudentName = string.Join("_", coverData.StudentsFullName?.Split(Path.GetInvalidFileNameChars()) ?? Array.Empty<string>());
                    string baseFileName = $"Титулка_{safeStudentName}_{formatName}_{DateTime.Now:yyyyMMddHHmmss}"; 

                    string argumentForGenerateAsync = (formatName == "GoogleDoc")
                        ? baseFileName 
                        : Path.Combine(outputDirectory, baseFileName); 
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d

                    StatusMessage = $"Генерація {formatName.ToUpper()}...";
                    Debug.WriteLine($"Запуск генерації для {formatName} з аргументом: {argumentForGenerateAsync}");

                    try
                    {
<<<<<<< HEAD
                        // Викликаємо GenerateAsync
=======
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
                        await generator.GenerateAsync(argumentForGenerateAsync);
                        Debug.WriteLine($"{formatName.ToUpper()} згенеровано успішно.");
                    }
                    catch (NotImplementedException) { string msg = $"Метод GenerateAsync не реалізовано для {formatName}"; Debug.WriteLine(msg); errors.Add(msg); }
<<<<<<< HEAD
                    catch (Exception ex) { string msg = $"Помилка {formatName}: {ex.Message}"; Debug.WriteLine(msg + $"\n{ex.StackTrace}"); errors.Add(msg); } // Додав StackTrace у Debug

                } // Кінець циклу foreach

                // Діагностичне повідомлення про кінець try блоку
                System.Diagnostics.Debug.WriteLine($"--- GenerateCoverPagesAsync ЗАВЕРШИВ try блок о {DateTime.Now:HH:mm:ss.fff} ---");

                // 5. Повідомляємо про результат
                if (errors.Count == 0) { StatusMessage = $"Успішно згенеровано {generators.Count} файл(ів) у папку: {outputDirectory}"; }
                else { StatusMessage = $"Генерація завершена з {errors.Count} помилками."; }
            }
            catch (Exception ex) // Обробка загальних помилок
=======
                    catch (Exception ex) { string msg = $"Помилка {formatName}: {ex.Message}"; Debug.WriteLine(msg + $"\n{ex.StackTrace}"); errors.Add(msg); } 

                } 

                System.Diagnostics.Debug.WriteLine($"--- GenerateCoverPagesAsync ЗАВЕРШИВ try блок о {DateTime.Now:HH:mm:ss.fff} ---");

                if (errors.Count == 0) { StatusMessage = $"Успішно згенеровано {generators.Count} файл(ів) у папку: {outputDirectory}"; }
                else { StatusMessage = $"Генерація завершена з {errors.Count} помилками."; }
            }
            catch (Exception ex) 
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
            {
                StatusMessage = $"Критична помилка: {ex.Message}";
                Debug.WriteLine($"Критична помилка у GenerateCoverPagesAsync: {ex.ToString()}");
                MessageBox.Show($"Сталася неочікувана помилка: {ex.Message}", "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
            }
<<<<<<< HEAD
        } // Кінець методу GenerateCoverPagesAsync
    } // Кінець класу MainWindowViewModel
} // Кінець простору імен
=======
        } 
    } 
} 
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
