using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data; 
using Google.Apis.Services;      
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CourseWork_OOP.Services 
{
    public static class GoogleSheet
    {
        public static async Task<List<CoverPageData>> ReadSheetAsync(string spreadSheetId, string range, CancellationToken cancellationToken = default)
        {
            List<CoverPageData> studentDataList = new List<CoverPageData>();
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

                        const int expectedColumnCount = 5;
                        if (row.Count < expectedColumnCount)
                        {
                            Debug.WriteLine($"Пропущено рядок {rowNumber} через недостатню кількість стовпців (очікувалось {expectedColumnCount}, отримано {row.Count}).");
                            rowNumber++;
                            continue;
                        }

                        try
                        {
                            var studentData = new CoverPageData
                            {
                                StudentsFullName = row[2]?.ToString(),        // Стовпець D
                                Group = row[3]?.ToString(),                   // Стовпець E
                                Topic = row[1]?.ToString(),                   // Стовпець C
                                SuperVisorFullName = row[0]?.ToString(),      // Стовпець B
                                SuperVisorPosition = row[4]?.ToString(),      // Стовпець F
                                University = "Черкаський національний університет імені Богдана Хмельницького",
                                Faculty = "Обчислювальної Техніки Інтелектуальних та Управляючих Систем",
                                Department = "Кафедра програмного забезпечення автоматизованих систем",
                                Discipline = "Об’єктно-орієнтоване програмування",
                                City = "Черкаси", 
                                Year = DateTime.Now.Year 
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