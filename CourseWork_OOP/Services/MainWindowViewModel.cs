using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseWork_OOP.Interfaces;
using CourseWork_OOP.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CourseWork_OOP
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IEnumerable<ITitlePageGenerator> _availableGenerators;

        [ObservableProperty] private string university = "Черкаський національний університет імені Богдана Хмельницького";
        [ObservableProperty] private string faculty = "обчислювальної техніки, інтелектуальних та управляючих систем";
        [ObservableProperty] private string department = "програмного забезпечення автоматизованих систем\r\n\r\n";
        [ObservableProperty] private string discipline = "ОБ'ЄКТО-ОРІЄНТОВАНОГО ПРОГРАМУВАННЯ";
        [ObservableProperty] private string topic = "Генератор титулок";
        [ObservableProperty] private string studentFullName = "Прізвище Ім'я По-батькові";
        [ObservableProperty] private string studentGroup = "КС-00";
        [ObservableProperty] private string supervisorFullName = "Прізвище І.П. Керівника";
        [ObservableProperty] private string supervisorPosition = "Посада Керівника";
        [ObservableProperty] private string city = "Черкаси";
        [ObservableProperty] private int year = DateTime.Now.Year;

        [ObservableProperty] private bool isMaleSelected = true;
        [ObservableProperty] private bool isFemaleSelected = false;
        [ObservableProperty] private string courseNumber = "2";
        [ObservableProperty] private string specialtyName = "спеціальності 121 «Інженерія програмного забезпечення»";
        [ObservableProperty] private string commissionInputText = ""; 
        [ObservableProperty] private bool isManualSource = true;
        [ObservableProperty] private bool isSheetSource = false;
        [ObservableProperty] private string spreadsheetId = "1dxJM2Gpj9YfyPVUEzMgQyjl4jc_QAF5U0sD2im8oxj4"; 
        [ObservableProperty] private string sheetRange = "ООП!B5:H"; 

        [ObservableProperty] private bool isLatexSelected = true;
        [ObservableProperty] private bool isHtmlSelected = true;
        [ObservableProperty] private bool isPlainTextSelected = true;
        [ObservableProperty] private bool isDocsSelected = true;

        [ObservableProperty] private string statusMessage = "Готово";


        public MainWindowViewModel(IEnumerable<ITitlePageGenerator> generators)
        {
            _availableGenerators = generators ?? throw new ArgumentNullException(nameof(generators));
            if (!_availableGenerators.Any()) { StatusMessage = "Увага: Жодного генератора не було передано."; }
        }

        [RelayCommand]
        private async Task GenerateCoverPagesAsync()
        {
            System.Diagnostics.Debug.WriteLine($"--- GenerateCoverPagesAsync ЗАПУЩЕНО о {DateTime.Now:HH:mm:ss.fff} ---");
            StatusMessage = "Перевірка налаштувань...";
            List<TitlePageData> dataToProcess = new List<TitlePageData>();
            string currentSex = IsMaleSelected ? "Чол" : "Жін";

            try
            {
                if (IsManualSource)
                {
                    StatusMessage = "Обробка даних, введених вручну...";
                    List<string> commissionNames = CommissionInputText?
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Take(3).ToList() ?? new List<string>();

                    TitlePageData manualData = new TitlePageData
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
                        Year = this.Year,
                        Sex = currentSex,
                        CourseNumber = this.CourseNumber,
                        SpecialtyName = this.SpecialtyName,
                        CommissionMemberNames = commissionNames
                    };

                    if (string.IsNullOrWhiteSpace(manualData.StudentsFullName) || string.IsNullOrWhiteSpace(manualData.Topic))
                    { StatusMessage = "Відсутні ПІБ студента або Тема роботи."; MessageBox.Show("Будь ласка, введіть ПІБ студента та Тему роботи.", "Відсутні Дані"); return; }
                    dataToProcess.Add(manualData);
                }
                else if (IsSheetSource)
                {
                    StatusMessage = "Читання даних з Google Sheets...";
                    if (string.IsNullOrWhiteSpace(SpreadsheetId) || string.IsNullOrWhiteSpace(SheetRange))
                    { StatusMessage = "Не вказано ID таблиці або діапазон."; MessageBox.Show("Будь ласка, вкажіть ID таблиці та діапазон для читання.", "Налаштування Google Sheets"); return; }

                    dataToProcess = await GoogleSheet.ReadSheetAsync(this.SpreadsheetId, this.SheetRange);
                    if (dataToProcess == null || dataToProcess.Count == 0) { StatusMessage = "Дані студентів не знайдено у таблиці."; return; }

                    foreach (var dataItem in dataToProcess)
                    {
                        dataItem.Sex ??= currentSex;
                        dataItem.CourseNumber ??= this.CourseNumber; 
                        dataItem.SpecialtyName ??= this.SpecialtyName; 

                        dataItem.University ??= this.University;
                        dataItem.Faculty ??= this.Faculty;
                        dataItem.Department ??= this.Department;
                        dataItem.Discipline ??= this.Discipline;

                         if (dataItem.CommissionMemberNames == null || !dataItem.CommissionMemberNames.Any())
                         {
                             dataItem.CommissionMemberNames = new List<string> { "Онищенко Б.О", "Порубльов І.М", "Гребенович Ю.Є" }; 
                         }
                    }
                }
                else { StatusMessage = "Джерело даних не вибрано."; return; }

                var selectedGenerators = _availableGenerators.Where(g =>
                     (IsLatexSelected && g is LaTeX) ||
                     (IsHtmlSelected && g is Html) ||
                     (IsPlainTextSelected && g is PlainText) ||
                     (IsDocsSelected && g is Docs)
                ).ToList();

                if (selectedGenerators.Count == 0) { StatusMessage = "Не вибрано жодного формату для генерації."; MessageBox.Show("Будь ласка, виберіть хоча б один формат.", "Формат не вибрано"); return; }

                StatusMessage = $"Отримано {dataToProcess.Count} записів. Обрано {selectedGenerators.Count} формат(ів). Починаю генерацію...";
                string outputDirectory = Path.Combine(AppContext.BaseDirectory, "Generated_TitlePages");
                Directory.CreateDirectory(outputDirectory);
                List<string> totalErrors = new List<string>();
                int generatedFileCount = 0;

                for (int i = 0; i < dataToProcess.Count; i++)
                {
                    TitlePageData currentStudentData = dataToProcess[i];
                    if (string.IsNullOrWhiteSpace(currentStudentData.StudentsFullName) || string.IsNullOrWhiteSpace(currentStudentData.Topic))
                    {
                        totalErrors.Add($"Пропуск запису #{i + 1}: Відсутнє ПІБ або Тема.");
                        continue;
                    }
                    string studentIdentifier = $"{(IsSheetSource ? $"({i + 1}/{dataToProcess.Count}) " : "")}{currentStudentData.StudentsFullName}";
                    StatusMessage = $"Генерація для {studentIdentifier}...";

                    foreach (var generator in selectedGenerators)
                    {
                        string formatName = "Unknown"; string fileExtension = "";
                        if (generator is LaTeX) { formatName = "LaTeX"; fileExtension = ".tex"; }
                        else if (generator is Html) { formatName = "Html"; fileExtension = ".html"; }
                        else if (generator is PlainText) { formatName = "PlainText"; fileExtension = ".txt"; }
                        else if (generator is Docs) { formatName = "GoogleDoc"; fileExtension = ""; }

                        string safeStudentName = string.Join("_", currentStudentData.StudentsFullName.Split(Path.GetInvalidFileNameChars()));
                        string baseFileName = $"Титулка_{safeStudentName}_{formatName}_{DateTime.Now:yyyyMMddHHmmss}";
                        string argumentForGenerateAsync = (generator is Docs) ? baseFileName : Path.Combine(outputDirectory, baseFileName);

                        Debug.WriteLine($"Запуск генерації {formatName} для {studentIdentifier} з аргументом: {argumentForGenerateAsync}");
                        try { await generator.GenerateAsync(currentStudentData, argumentForGenerateAsync); generatedFileCount++; }
                        catch (Exception ex) { string msg = $"Помилка {formatName} для {studentIdentifier}: {ex.Message}"; Debug.WriteLine(msg + $"\n{ex.StackTrace}"); totalErrors.Add(msg); }
                    }
                }
                string finalMessage = $"Завершено. Оброблено записів: {dataToProcess.Count}. ";
                finalMessage += (totalErrors.Count == 0) ? $"Успішно згенеровано {generatedFileCount} файлів/документів." : $"Успішно: {generatedFileCount}. Помилок: {totalErrors.Count}.";
                StatusMessage = finalMessage;
            }
            catch (Exception ex) { StatusMessage = $"Критична помилка: {ex.Message}"; Debug.WriteLine($"Критична помилка: {ex.ToString()}"); }
            // IsBusy видалено
        }
    }
}
