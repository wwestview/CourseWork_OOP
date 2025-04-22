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
            System.Diagnostics.Debug.WriteLine($"--- GenerateCoverPagesAsync  ЗАПУЩЕНО о {DateTime.Now:HH:mm:ss.fff} ---");

            StatusMessage = "Читання даних з Google Sheets...";

            string spreadSheetId = "1dxJM2Gpj9YfyPVUEzMgQyjl4jc_QAF5U0sD2im8oxj4";
            string range = "ООП!B5:F";

            List<CoverPageData> allStudentData;
            try
            {
                allStudentData = await GoogleSheet.ReadSheetAsync(spreadSheetId, range);
            }
            catch (ArgumentException argEx)
            {
                StatusMessage = $"Помилка конфігурації: {argEx.Message}";
                MessageBox.Show(argEx.Message, "Помилка конфігурації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Помилка читання даних з таблиці: {ex.Message}";
                Debug.WriteLine($"Помилка читання даних з таблиці: {ex.ToString()}");
                MessageBox.Show($"Не вдалося прочитати дані: {ex.Message}", "Помилка Google Sheets", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (allStudentData == null || allStudentData.Count == 0)
            {
                StatusMessage = "Дані студентів не знайдено у таблиці або сталася помилка читання.";
                return;
            }

            StatusMessage = $"Отримано дані для {allStudentData.Count} студентів. Починаю генерацію...";

            string outputDirectory = Path.Combine(AppContext.BaseDirectory, "Generated_TitlePages");
            Directory.CreateDirectory(outputDirectory);

            List<string> totalErrors = new List<string>();
            int successCount = 0;
            int generatorCountPerStudent = 4; 
            int totalFilesToGenerate = allStudentData.Count * generatorCountPerStudent;
            int generatedFileCount = 0;


            try 
            {
                for (int i = 0; i < allStudentData.Count; i++)
                {
                    CoverPageData currentStudentData = allStudentData[i];

                    if (string.IsNullOrWhiteSpace(currentStudentData.StudentsFullName) || string.IsNullOrWhiteSpace(currentStudentData.Topic))
                    {
                        string errorMsg = $"Пропуск студента #{i + 1}: Відсутнє ПІБ або Тема.";
                        Debug.WriteLine(errorMsg);
                        totalErrors.Add(errorMsg);
                        continue;
                    }

                    string studentIdentifier = $"{i + 1}/{allStudentData.Count}: {currentStudentData.StudentsFullName}";
                    StatusMessage = $"Генерація для {studentIdentifier}...";
                    Debug.WriteLine($"--- Початок генерації для: {studentIdentifier} ---");

                    List<CoverPageGenerator> generators = new List<CoverPageGenerator>();
                    try
                    {
                        generators.Add(new LaTeX(currentStudentData));
                        generators.Add(new Html(currentStudentData));
                        generators.Add(new PlainText(currentStudentData));
                       // generators.Add(new Docs(currentStudentData));
                    }
                    catch (Exception ex)
                    {
                        string errorMsg = $"Помилка створення генераторів для {studentIdentifier}: {ex.Message}";
                        Debug.WriteLine(errorMsg);
                        totalErrors.Add(errorMsg);
                        continue;
                    }

                    foreach (var generator in generators)
                    {

                        string formatName = "Unknown";
                        string fileExtension = "tmp";

                        if (generator is LaTeX) { formatName = "LaTeX"; fileExtension = "tex"; }
                        else if (generator is Html) { formatName = "Html"; fileExtension = "html"; }
                        else if (generator is PlainText) { formatName = "PlainText"; fileExtension = "txt"; }
                       // else if (generator is Docs) { formatName = "GoogleDoc"; fileExtension = ""; }

                        string safeStudentName = string.Join("_", currentStudentData.StudentsFullName.Split(Path.GetInvalidFileNameChars()));
                        string baseFileName = $"Титулка_{safeStudentName}_{formatName}_{DateTime.Now:yyyyMMddHHmmss}";
                        string argumentForGenerateAsync = (formatName == "GoogleDoc") ? baseFileName : Path.Combine(outputDirectory, baseFileName);

                        Debug.WriteLine($"Запуск генерації {formatName} для {studentIdentifier} з аргументом: {argumentForGenerateAsync}");

                        try
                        {
                            await generator.GenerateAsync(argumentForGenerateAsync);
                            Debug.WriteLine($"{formatName.ToUpper()} згенеровано успішно для {studentIdentifier}.");
                            generatedFileCount++;
                        }
                        catch (NotImplementedException) {  }
                        catch (Exception ex) {  }

                    } 


                } 

                string finalMessage = $"Завершено. Оброблено студентів: {allStudentData.Count}. ";
                if (totalErrors.Count == 0) { finalMessage += $"Успішно згенеровано {generatedFileCount} файлів/документів."; }
                else { finalMessage += $"Успішно: {generatedFileCount} файлів/документів. Помилок: {totalErrors.Count}."; }
                StatusMessage = finalMessage;
                Debug.WriteLine($"--- GenerateCoverPagesAsync (Batch Mode, No IsBusy) ЗАВЕРШИВ роботу о {DateTime.Now:HH:mm:ss.fff} ---");

            }
            catch (Exception ex)
            {
                StatusMessage = $"Критична помилка під час генерації: {ex.Message}";
                Debug.WriteLine($"Критична помилка у GenerateCoverPagesAsync (Batch): {ex.ToString()}");
                MessageBox.Show($"Сталася неочікувана помилка під час пакетної генерації: {ex.Message}", "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } 
    } 
} 