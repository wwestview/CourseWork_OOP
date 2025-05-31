using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseWork_OOP.Interfaces;
using CourseWork_OOP.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace CourseWork_OOP
{
    public partial class MainWindowViewModel : ObservableObject, IDataErrorInfo
    {
        private readonly IEnumerable<ITitlePageGenerator> _availableGenerators;
        private readonly string[] _validatedProperties;

        public IAsyncRelayCommand GenerateCoverPagesCommand { get; }

        [ObservableProperty]
        private string _university = "Черкаський національний університет імені Богдана Хмельницького";
        [ObservableProperty]
        private string _faculty = "обчислювальної техніки, інтелектуальних та управляючих систем";
        [ObservableProperty]
        private string _department = "програмного забезпечення автоматизованих систем";
        [ObservableProperty]
        private string _discipline = "ОБ'ЄКТО-ОРІЄНТОВАНОГО ПРОГРАМУВАННЯ";
        [ObservableProperty]
        private string _topic = "Генератор титулок";
        [ObservableProperty]
        private string _studentFullName = "Прізвище Ім'я По-батькові";
        [ObservableProperty]
        private string _studentGroup = "КС-23";
        [ObservableProperty]
        private string _supervisorFullName = "Прізвище І.П. Керівника";
        [ObservableProperty]
        private string _supervisorPosition = "Посада Керівника";
        [ObservableProperty]
        private string _city = "Черкаси";
        [ObservableProperty]
        private int _year = DateTime.Now.Year;
        [ObservableProperty]
        private bool _isMaleSelected = true;
        [ObservableProperty]
        private bool _isFemaleSelected = false;
        [ObservableProperty]
        private string _courseNumber = "2";
        [ObservableProperty]
        private string _specialtyName = "спеціальності 121 «Інженерія програмного забезпечення»";
        [ObservableProperty]
        private string _commissionInputText = "";
        [ObservableProperty]
        private bool _isManualSource = true;
        [ObservableProperty]
        private bool _isSheetSource = false;
        [ObservableProperty]
        private string _spreadsheetId = "1dxJM2Gpj9YfyPVUEzMgQyjl4jc_QAF5U0sD2im8oxj4";
        [ObservableProperty]
        private string _sheetRange = "ООП!B4:O";
        [ObservableProperty]
        private bool _isLatexSelected = true;
        [ObservableProperty]
        private bool _isHtmlSelected = true;
        [ObservableProperty]
        private bool _isPlainTextSelected = true;
        [ObservableProperty]
        private bool _isDocsSelected = true;
        [ObservableProperty]
        private string _statusMessage = "Готово";

        public MainWindowViewModel(IEnumerable<ITitlePageGenerator> generators)
        {
            _availableGenerators = generators ?? throw new ArgumentNullException(nameof(generators));
            if (!_availableGenerators.Any()) { StatusMessage = "Увага: Жодного генератора не було передано."; }

            _validatedProperties = new[]
            {
                nameof(University), nameof(Faculty), nameof(Department), nameof(Discipline), nameof(Topic),
                nameof(StudentFullName), nameof(CourseNumber), nameof(StudentGroup), nameof(SpecialtyName),
                nameof(SupervisorFullName), nameof(SupervisorPosition), nameof(City), nameof(Year),
                nameof(SpreadsheetId), nameof(SheetRange), nameof(CommissionInputText)
            };

            GenerateCoverPagesCommand = new AsyncRelayCommand(ExecuteGenerateCoverPagesAsync, CanGenerate);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (_validatedProperties.Contains(e.PropertyName) ||
                e.PropertyName == nameof(IsManualSource) ||
                e.PropertyName == nameof(IsSheetSource))
            {
                GenerateCoverPagesCommand.NotifyCanExecuteChanged();
            }

            if (e.PropertyName == nameof(IsManualSource) || e.PropertyName == nameof(IsSheetSource))
            {
                RefreshAllValidations();
            }
        }

        private void RefreshAllValidations()
        {
            foreach (var propertyName in _validatedProperties)
            {
                OnPropertyChanged(propertyName);
            }
        }

        private bool CanGenerate()
        {
            if (IsManualSource)
            {
                foreach (var property in _validatedProperties)
                {
                    if (property == nameof(SpreadsheetId) || property == nameof(SheetRange)) continue;
                    if (!string.IsNullOrEmpty(this[property])) return false;
                }
            }
            else if (IsSheetSource)
            {
                foreach (var property in new[] { nameof(SpreadsheetId), nameof(SheetRange) })
                {
                    if (!string.IsNullOrEmpty(this[property])) return false;
                }
            }
            return true;
        }

        private async Task ExecuteGenerateCoverPagesAsync()
        {
            System.Diagnostics.Debug.WriteLine($"--- ExecuteGenerateCoverPagesAsync ЗАПУЩЕНО о {DateTime.Now:HH:mm:ss.fff} ---");
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
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList() ?? new List<string>();

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
                    dataToProcess.Add(manualData);
                }
                else if (IsSheetSource)
                {
                    StatusMessage = "Читання даних з Google Sheets...";
                    dataToProcess = await GoogleSheet.ReadSheetAsync(this.SpreadsheetId, this.SheetRange);
                    if (dataToProcess == null || !dataToProcess.Any()) { StatusMessage = "Дані студентів не знайдено у таблиці або сталася помилка."; return; }
                    foreach (var dataItem in dataToProcess)
                    {
                        dataItem.Sex ??= currentSex; dataItem.CourseNumber ??= this.CourseNumber; dataItem.SpecialtyName ??= this.SpecialtyName;
                        dataItem.University ??= this.University; dataItem.Faculty ??= this.Faculty; dataItem.Department ??= this.Department;
                        dataItem.Discipline ??= this.Discipline; dataItem.City ??= this.City; dataItem.Year = this.Year;
                        if (dataItem.CommissionMemberNames == null || !dataItem.CommissionMemberNames.Any())
                        { dataItem.CommissionMemberNames = new List<string>(); }
                    }
                }
                else { StatusMessage = "Джерело даних не вибрано."; return; }

                var selectedGenerators = new List<ITitlePageGenerator>();
                if (IsLatexSelected && _availableGenerators.FirstOrDefault(g => g is LaTeX) is { } latexGen) selectedGenerators.Add(latexGen);
                if (IsHtmlSelected && _availableGenerators.FirstOrDefault(g => g is Html) is { } htmlGen) selectedGenerators.Add(htmlGen);
                if (IsPlainTextSelected && _availableGenerators.FirstOrDefault(g => g is PlainText) is { } plainTextGen) selectedGenerators.Add(plainTextGen);
                if (IsDocsSelected && _availableGenerators.FirstOrDefault(g => g is Docs) is { } docsGen) selectedGenerators.Add(docsGen);

                if (!selectedGenerators.Any()) { StatusMessage = "Не вибрано жодного формату для генерації."; MessageBox.Show("Будь ласка, виберіть хоча б один формат.", "Формат не вибрано"); return; }

                StatusMessage = $"Отримано {dataToProcess.Count} записів. Обрано {selectedGenerators.Count} формат(ів). Починаю генерацію...";
                string outputDirectory = Path.Combine(AppContext.BaseDirectory, "Generated_TitlePages");
                Directory.CreateDirectory(outputDirectory);
                List<string> totalErrors = new List<string>();
                int generatedFileCount = 0;

                for (int i = 0; i < dataToProcess.Count; i++)
                {
                    TitlePageData currentStudentData = dataToProcess[i];
                    if (string.IsNullOrWhiteSpace(currentStudentData.StudentsFullName) || string.IsNullOrWhiteSpace(currentStudentData.Topic))
                    { totalErrors.Add($"Пропуск запису #{i + 1}: Відсутнє ПІБ або Тема."); continue; }

                    string studentIdentifier = $"{(IsSheetSource ? $"({i + 1}/{dataToProcess.Count}) " : "")}{currentStudentData.StudentsFullName}";
                    StatusMessage = $"Генерація для {studentIdentifier}...";

                    foreach (var generator in selectedGenerators)
                    {
                        var baseGen = (BaseTitlePageGenerator)generator;
                        string formatName = baseGen.FormatName;
                        string safeStudentNamePart = string.Join("_", currentStudentData.StudentsFullName?.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries) ?? new string[] { "UnknownStudent" });
                        if (string.IsNullOrWhiteSpace(safeStudentNamePart)) safeStudentNamePart = "UnknownStudent";
                        string baseFileNameComponent = $"Титулка_{safeStudentNamePart}_{formatName.Replace(" ", "_")}_{DateTime.Now:yyyyMMddHHmmss}";
                        string argumentForGenerateAsync;
                        if (generator is Docs)
                        {
                            argumentForGenerateAsync = baseFileNameComponent;
                        }
                        else
                        {
                            argumentForGenerateAsync = Path.Combine(outputDirectory, baseFileNameComponent);
                        }
                        Debug.WriteLine($"Запуск генерації {formatName} для {studentIdentifier} з аргументом: {argumentForGenerateAsync}");
                        try
                        {
                            await generator.GenerateAsync(currentStudentData, argumentForGenerateAsync);
                            generatedFileCount++;
                        }
                        catch (Exception ex)
                        {
                            string msg = $"Помилка {formatName} для {studentIdentifier}: {ex.Message}";
                            Debug.WriteLine(msg + $"\n{ex.StackTrace}"); totalErrors.Add(msg);
                        }
                    }
                }
                string finalMessage = $"Завершено. Оброблено записів: {dataToProcess.Count}. ";
                finalMessage += (totalErrors.Count == 0) ? $"Успішно згенеровано {generatedFileCount} файлів/документів." : $"Успішно: {generatedFileCount}. Помилок: {totalErrors.Count}.";
                if (totalErrors.Any())
                {
                    finalMessage += "\nДеталі помилок див. у Debug Output або у вікні з помилками.";
                    MessageBox.Show("Під час генерації виникли помилки:\n" + string.Join("\n", totalErrors.Take(5)) + (totalErrors.Count > 5 ? "\n...та інші." : ""), "Помилки генерації", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                StatusMessage = finalMessage;
            }
            catch (Exception ex) { StatusMessage = $"Критична помилка: {ex.Message}"; Debug.WriteLine($"Критична помилка: {ex.ToString()}"); }
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string result = null;
                string tempValue;

                if (IsManualSource)
                {
                    switch (columnName)
                    {
                        case nameof(University):
                            tempValue = University;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "Назва університету не може бути порожньою.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}\s().,'-]+$")) result = "Університет: дозволені літери, пробіли та символи ().,'-";
                            break;
                        case nameof(Faculty):
                            tempValue = Faculty;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "Назва факультету не може бути порожньою.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}\s().,'-]+$")) result = "Факультет: дозволені літери, пробіли та символи ().,'-";
                            break;
                        case nameof(Department):
                            tempValue = Department;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "Назва кафедри не може бути порожньою.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}\s().,'-]+$")) result = "Кафедра: дозволені літери, пробіли та символи ().,'-";
                            break;
                        case nameof(Discipline):
                            tempValue = Discipline;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "Назва дисципліни не може бути порожньою.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}\s().,'""«»-]+$")) result = "Дисципліна: дозволені літери, пробіли, символи ().,'\"«»-";
                            break;
                        case nameof(Topic):
                            tempValue = Topic;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "Тема роботи не може бути порожньою.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}\d\s().,'""«»;:?-]+$")) result = "Тема: недозволені символи. Дозволені літери, цифри, пробіли та .,'\"«»;:?-";
                            break;
                        case nameof(StudentFullName):
                            tempValue = StudentFullName;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "ПІБ студента не може бути порожнім.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}\s'.-]+$")) result = "ПІБ: дозволені літери, пробіли, апостроф, крапка, дефіс.";
                            break;
                        case nameof(CourseNumber):
                            tempValue = CourseNumber;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "Номер курсу не може бути порожнім.";
                            else if (!Regex.IsMatch(tempValue, @"^\d{1,2}$")) result = "Курс: має бути числом (1 або 2 цифри).";
                            break;
                        case nameof(StudentGroup):
                            tempValue = StudentGroup;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "Номер групи не може бути порожнім.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}0-9-]+$")) result = "Група: дозволені літери, цифри та дефіс.";
                            break;
                        case nameof(SpecialtyName):
                            tempValue = SpecialtyName;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "Назва спеціальності не може бути порожньою.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}\d\s«»().,'""-]+$")) result = "Спеціальність: дозволені літери, цифри, пробіли, «»().,'\"-";
                            break;
                        case nameof(SupervisorFullName):
                            tempValue = SupervisorFullName;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "ПІБ керівника не може бути порожнім.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}\s'.-]+$")) result = "ПІБ керівника: дозволені літери, пробіли, апостроф, крапка, дефіс.";
                            break;
                        case nameof(SupervisorPosition):
                            tempValue = SupervisorPosition;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "Посада керівника не може бути порожньою.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}\d\s().,'""«»/-]+$")) result = "Посада: дозволені літери, цифри, пробіли та символи ().,'\"«»/-";
                            break;
                        case nameof(CommissionInputText):
                            tempValue = CommissionInputText;
                            if (!string.IsNullOrEmpty(tempValue))
                            {
                                var members = tempValue.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim())
                                    .Where(s => !string.IsNullOrWhiteSpace(s))
                                    .ToList();
                                if (members.Count > 3) result = "Максимум 3 члени комісії.";
                                else
                                {
                                    foreach (var member in members)
                                    {
                                        if (!Regex.IsMatch(member, @"^[\p{L}\s.]+$"))
                                        {
                                            result = "Члени комісії: ПІБ може містити літери, пробіли, крапки.";
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        case nameof(City):
                            tempValue = City;
                            if (string.IsNullOrWhiteSpace(tempValue)) result = "Назва міста не може бути порожньою.";
                            else if (!Regex.IsMatch(tempValue, @"^[\p{L}\s'-]+$")) result = "Місто: дозволені літери, пробіли, апостроф, дефіс.";
                            break;
                        case nameof(Year):
                            if (Year < 1950 || Year > DateTime.Now.Year + 10) result = $"Рік: має бути в межах від 1950 до {DateTime.Now.Year + 10}.";
                            break;
                    }
                }
                else if (IsSheetSource)
                {
                    switch (columnName)
                    {
                        case nameof(SpreadsheetId):
                            if (string.IsNullOrWhiteSpace(SpreadsheetId)) result = "ID таблиці не може бути порожнім.";
                            break;
                        case nameof(SheetRange):
                            if (string.IsNullOrWhiteSpace(SheetRange)) result = "Діапазон не може бути порожнім.";
                            break;
                    }
                }
                return result;
            }
        }
    }
}