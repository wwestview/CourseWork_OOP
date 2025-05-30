using CourseWork_OOP.Services; // Переконайтеся, що TitlePageData тут доступний
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CourseWork_OOP.Services
{
    public static class GoogleSheet
    {
        // Допоміжний метод для отримання рядкового значення з комірки за індексом стовпця
        private static string? GetStringFromCell(IList<object> row, int colIndex)
        {
            if (row != null && colIndex < row.Count && row[colIndex] != null && !string.IsNullOrWhiteSpace(row[colIndex]?.ToString()))
            {
                return row[colIndex]?.ToString()?.Trim();
            }
            return null;
        }

        // Допоміжний метод для отримання рядкового значення для властивості за назвою заголовка
        private static string? GetNullableString(IList<object> row, Dictionary<string, int> headerMap, string headerName) // Змінено propertyName на headerName
        {
            if (headerMap.TryGetValue(headerName, out int colIndex)) // Використовуємо headerName
            {
                return GetStringFromCell(row, colIndex);
            }
            return null; // Властивість не знайдена в таблиці, буде null
        }

        // Допоміжний метод для отримання цілочисельного значення для властивості
        private static int GetInt(IList<object> row, Dictionary<string, int> headerMap, string headerName, int defaultValueIfMissingOrUnparsable) // Змінено propertyName на headerName
        {
            if (headerMap.TryGetValue(headerName, out int colIndex)) // Використовуємо headerName
            {
                string? cellValue = GetStringFromCell(row, colIndex);
                if (int.TryParse(cellValue, out int parsedValue))
                {
                    return parsedValue;
                }
            }
            return defaultValueIfMissingOrUnparsable;
        }

        // Допоміжний метод для отримання списку рядків для властивості (наприклад, CommissionMemberNames)
        private static List<string> GetStringList(IList<object> row, Dictionary<string, int> headerMap, string headerName, char separator = '\n') // Змінено propertyName на headerName
        {
            if (headerMap.TryGetValue(headerName, out int colIndex)) // Використовуємо headerName
            {
                string? cellValue = GetStringFromCell(row, colIndex);
                if (!string.IsNullOrEmpty(cellValue))
                {
                    return cellValue.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Trim())
                                    .Where(s => !string.IsNullOrWhiteSpace(s))
                                    .ToList();
                }
            }
            return new List<string>();
        }

        public static async Task<List<TitlePageData>> ReadSheetAsync(string spreadSheetId, string range, CancellationToken cancellationToken = default)
        {
            List<TitlePageData> studentDataList = new List<TitlePageData>();
            Debug.WriteLine($"[GoogleSheet] Спроба зчитати дані з таблиці ID: {spreadSheetId}, діапазон: {range}");

            if (string.IsNullOrWhiteSpace(spreadSheetId) || string.IsNullOrWhiteSpace(range))
            {
                Debug.WriteLine("[GoogleSheet] Не вказано ID таблиці або діапазон.");
                throw new ArgumentException("Необхідно вказати Spreadsheet ID та діапазон.");
            }

            try
            {
                var credential = await GoogleAuth.GetCredentialsAsync(cancellationToken);
                var sheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "CourseWork OOP Sheet Reader"
                });

                SpreadsheetsResource.ValuesResource.GetRequest request = sheetsService.Spreadsheets.Values.Get(spreadSheetId, range);
                Debug.WriteLine("[GoogleSheet] Надсилаємо запит до Google Sheets API...");
                ValueRange response = await request.ExecuteAsync(cancellationToken);
                IList<IList<object>> values = response.Values;

                if (values != null && values.Count > 1)
                {
                    Debug.WriteLine($"[GoogleSheet] Отримано {values.Count} рядків з таблиці (включно з заголовком).");

                    IList<object> headerRowObjects = values[0];
                    var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                    for (int j = 0; j < headerRowObjects.Count; j++)
                    {
                        string? headerName = headerRowObjects[j]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(headerName) && !headerMap.ContainsKey(headerName))
                        {
                            headerMap[headerName] = j;
                            Debug.WriteLine($"[GoogleSheet] Знайдено заголовок '{headerName}' у стовпці {j}.");
                        }
                    }

                    if (!headerMap.Any())
                    {
                        Debug.WriteLine("[GoogleSheet] У першому рядку не знайдено валідних заголовків. Обробка даних неможлива.");
                        return studentDataList;
                    }

                    for (int i = 1; i < values.Count; i++)
                    {
                        IList<object> currentRow = values[i];
                        int actualSheetRowNumberForDebug = 0;
                        if (response.Range != null && response.Range.Contains("!"))
                        {
                            var parts = response.Range.Split('!');
                            if (parts.Length > 1)
                            {
                                var cellRef = parts[1].Split(':')[0];
                                string rowNumStr = new string(cellRef.Where(char.IsDigit).ToArray());
                                if (int.TryParse(rowNumStr, out int startRow))
                                {
                                    actualSheetRowNumberForDebug = startRow + i;
                                }
                            }
                        }
                        if (actualSheetRowNumberForDebug == 0) actualSheetRowNumberForDebug = i + 1;


                        if (currentRow == null || currentRow.All(cell => cell == null || string.IsNullOrWhiteSpace(cell.ToString())))
                        {
                            Debug.WriteLine($"[GoogleSheet] Пропущено порожній рядок (індекс у values: {i}, приблизний номер рядка в таблиці: {actualSheetRowNumberForDebug}).");
                            continue;
                        }

                        try
                        {
                            var studentData = new TitlePageData();

                            // --- ОСЬ ТУТ ВНЕСЕНО ЗМІНИ ДЛЯ ВІДПОВІДНОСТІ ВАШОМУ ЗОБРАЖЕННЮ ---
                            studentData.SuperVisorFullName = GetNullableString(currentRow, headerMap, "Викладач");
                            studentData.Topic = GetNullableString(currentRow, headerMap, "Тема курсової роботи");
                            studentData.StudentsFullName = GetNullableString(currentRow, headerMap, "Студент");
                            studentData.Group = GetNullableString(currentRow, headerMap, "Група");
                            studentData.SuperVisorPosition = GetNullableString(currentRow, headerMap, "Посада керівника");
                            studentData.Sex = GetNullableString(currentRow, headerMap, "Стать");
                            studentData.CommissionMemberNames = GetStringList(currentRow, headerMap, "Члени комісії");
                            studentData.SpecialtyName = GetNullableString(currentRow, headerMap, "Спеціальність");
                            studentData.Discipline = GetNullableString(currentRow, headerMap, "Дисципліна");
                            studentData.University = GetNullableString(currentRow, headerMap, "Університет");
                            studentData.Faculty = GetNullableString(currentRow, headerMap, "Факультет");
                            studentData.Department = GetNullableString(currentRow, headerMap, "Департамент"); 
                            studentData.City = GetNullableString(currentRow, headerMap, "Місто");
                            studentData.CourseNumber = GetNullableString(currentRow, headerMap, "Курс");

                            if (string.IsNullOrWhiteSpace(studentData.StudentsFullName) || string.IsNullOrWhiteSpace(studentData.Topic))
                            {
                                Debug.WriteLine($"[GoogleSheet] Пропущено рядок (індекс у values: {i}, приблизний номер: {actualSheetRowNumberForDebug}) через відсутність ПІБ студента або Теми.");
                                continue;
                            }

                            studentDataList.Add(studentData);
                            Debug.WriteLine($"[GoogleSheet] Успішно розпарсено дані для: {studentData.StudentsFullName} (рядок з індексом values: {i}). Члени комісії: [{string.Join("; ", studentData.CommissionMemberNames)}]");
                        }
                        catch (Exception parseEx)
                        {
                            Debug.WriteLine($"[GoogleSheet] Помилка парсингу рядка (індекс у values: {i}, приблизний номер: {actualSheetRowNumberForDebug}): {parseEx.Message}");
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("[GoogleSheet] Дані у вказаному діапазоні не знайдено, або наявний лише рядок заголовків.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GoogleSheet] Помилка читання даних з Google Sheets: {ex.ToString()}");
                throw new Exception($"Не вдалося прочитати дані з Google Таблиці: {ex.Message}", ex);
            }

            Debug.WriteLine($"[GoogleSheet] Успішно оброблено {studentDataList.Count} записів студентів з Google Sheet.");
            return studentDataList;
        }
    }
}