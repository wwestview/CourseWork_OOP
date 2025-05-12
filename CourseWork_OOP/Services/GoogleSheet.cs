using CourseWork_OOP.Services; 
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
        private static string? GetRowValue(IList<object> row, int index, string? defaultValue = null)
        {
            if (row != null && index < row.Count && row[index] != null && !string.IsNullOrWhiteSpace(row[index]?.ToString()))
            {
                return row[index]?.ToString();
            }
            return defaultValue;
        }


        public static async Task<List<TitlePageData>> ReadSheetAsync(string spreadSheetId, string range, CancellationToken cancellationToken = default)
        {
            List<TitlePageData> studentDataList = new List<TitlePageData>();
            Debug.WriteLine($"Спроба читання даних з таблиці ID: {spreadSheetId}, діапазон: {range}");

            if (string.IsNullOrWhiteSpace(spreadSheetId) || string.IsNullOrWhiteSpace(range))
            {
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
                Debug.WriteLine("Надсилаємо запит до Google Sheets API...");
                ValueRange response = await request.ExecuteAsync(cancellationToken);
                IList<IList<object>> values = response.Values;

                if (values != null && values.Count > 0)
                {
                    Debug.WriteLine($"Отримано {values.Count} рядків даних.");
                    int currentSheetRowNumber = 5;

                    foreach (var row in values)
                    {
                        if (row == null || row.All(cell => cell == null || string.IsNullOrWhiteSpace(cell.ToString())))
                        {
                            Debug.WriteLine($"Пропущено порожній рядок (фактичний номер у таблиці: {currentSheetRowNumber}).");
                            currentSheetRowNumber++;
                            continue;
                        }

                        // B=0, C=1, D=2, E=3, F=4
                        const int expectedCoreColumnCount = 5;

                        if (row.Count < expectedCoreColumnCount)
                        {
                            Debug.WriteLine($"Пропущено рядок {currentSheetRowNumber} через недостатню кількість основних стовпців (очікувалось >= {expectedCoreColumnCount}, отримано {row.Count}). Діапазон: {range}");
                            currentSheetRowNumber++;
                            continue;
                        }

                        try
                        {
                            var studentData = new TitlePageData
                            {
                                SuperVisorFullName = GetRowValue(row, 0), // Стовпець B
                                Topic = GetRowValue(row, 1), // Стовпець C
                                StudentsFullName = GetRowValue(row, 2), // Стовпець D
                                Group = GetRowValue(row, 3), // Стовпець E
                                SuperVisorPosition = GetRowValue(row, 4), // Стовпець F

                                Sex = GetRowValue(row, 5, "Чол"),

                                CommissionMemberNames = (row.Count > 6 && row[6] != null)
                                                      ? GetRowValue(row, 6, "")
                                                            ?.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                                            .Select(s => s.Trim())
                                                            .Take(3)
                                                            .ToList() ?? new List<string>()
                                                      : new List<string>(),

                            };


                            if (!string.IsNullOrWhiteSpace(studentData.StudentsFullName))
                            {
                                studentDataList.Add(studentData);
                                Debug.WriteLine($"Додано дані для студента: {studentData.StudentsFullName}, Комісія: [{string.Join("; ", studentData.CommissionMemberNames)}]");
                            }
                            else
                            {
                                Debug.WriteLine($"Пропущено рядок {currentSheetRowNumber} через відсутність ПІБ студента.");
                            }
                        }
                        catch (Exception parseEx)
                        {
                            Debug.WriteLine($"Помилка парсингу рядка {currentSheetRowNumber}: {parseEx.Message}");
                        }
                        currentSheetRowNumber++;
                    }
                }
                else
                {
                    Debug.WriteLine("Не знайдено даних у вказаному діапазоні.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка читання даних з Google Sheets: {ex.ToString()}");
                throw new Exception($"Не вдалося прочитати дані з Google Таблиці: {ex.Message}", ex);
            }

            Debug.WriteLine($"Успішно оброблено {studentDataList.Count} записів студентів.");
            return studentDataList;
        }
    }
}
