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

        private static string GetText(string? text, string defaultText = "") => text ?? defaultText;

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
                    int rowNumber = 5; 

                    foreach (var row in values)
                    {
                        if (row == null || row.All(cell => cell == null || string.IsNullOrWhiteSpace(cell.ToString())))
                        {
                            Debug.WriteLine($"Пропущено порожній рядок {rowNumber}.");
                            rowNumber++;
                            continue;
                        }

                        //  діапазон B:H 7 стовпців 
                        const int expectedColumnCount = 7;

                        if (row.Count < expectedColumnCount)
                        {
                            Debug.WriteLine($"Пропущено рядок {rowNumber} через недостатню кількість стовпців (очікувалось >= {expectedColumnCount}, отримано {row.Count}). Діапазон: {range}");
                            rowNumber++;
                            continue;
                        }

                        try
                        {
                            // row[0] -> стовпець B
                            // row[1] -> стовпець C
                            // row[6] -> стовпець H 
                            var studentData = new TitlePageData
                            {
                                SuperVisorFullName = GetRowValue(row, 0), //  стовпець B
                                Topic = GetRowValue(row, 1), //  стовпець C
                                StudentsFullName = GetRowValue(row, 2), //  стовпець D
                                Group = GetRowValue(row, 3), //  стовпець E
                                SuperVisorPosition = GetRowValue(row, 4), //  стовпець F
                                Sex = GetRowValue(row, 5, "Чол"), //  стовпець G 

                                CommissionMemberNames = (row.Count > 6 && row[6] != null)
                                                      ? GetRowValue(row, 6, "")
                                                            ?.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries) 
                                                            .Select(s => s.Trim())
                                                            .Take(3)
                                                            .ToList() ?? new List<string>()
                                                      : new List<string>(),

                                CourseNumber = GetText(null, "2"),
                                SpecialtyName = GetText(null, "спеціальності 121 «Інженерія програмного забезпечення»"),
                                Discipline = GetText(null, "Об’єктно-орієнтоване програмування".ToUpper()),
                                University = GetText(null, "Черкаський національний університет імені Богдана Хмельницького"),
                                Faculty = GetText(null, "обчислювальної Техніки Інтелектуальних та Управляючих Систем"),
                                Department = GetText(null, "програмного забезпечення автоматизованих систем"),
                            };

                            if (!string.IsNullOrWhiteSpace(studentData.StudentsFullName))
                            {
                                studentDataList.Add(studentData);
                                Debug.WriteLine($"Додано дані для студента: {studentData.StudentsFullName}");
                            }
                            else
                            {
                                Debug.WriteLine($"Пропущено рядок {rowNumber} через відсутність ПІБ студента.");
                            }
                        }
                        catch (Exception parseEx)
                        {
                            Debug.WriteLine($"Помилка парсингу рядка {rowNumber}: {parseEx.Message}");
                        }
                        rowNumber++;
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
