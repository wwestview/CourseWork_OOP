using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data; // Потрібно для Document, BatchUpdateDocumentRequest, Request, InsertTextRequest etc.
using Google.Apis.Services;
using System;
using System.Collections.Generic; // Потрібно для List<Request>
using System.Diagnostics;
using System.Threading.Tasks;

namespace CourseWork_OOP.Services
{
    public class Docs : CoverPageGenerator
    {
        public Docs(CoverPageData data) : base(data)
        {
        }

        public override async Task GenerateAsync(string baseFileName)
        {
            // baseFileName використовується як ЗАГОЛОВОК Google Документа
            Debug.WriteLine($"Початок генерації Google Doc з назвою: {baseFileName}");
            string documentId = null; // Зберігатимемо ID створеного документа

            try
            {
                // 1. Автентифікація
                UserCredential credential = await GoogleAuth.GetCredentialsAsync();

                // 2. Створення сервісу Docs API
                var docsService = new DocsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "CourseWork OOP Title Page Generator"
                });

                // 3. Створення ПОРОЖНЬОГО документа (щоб отримати його ID)
                var document = new Document { Title = baseFileName };
                Debug.WriteLine($"Надсилаємо запит на створення Google Doc: {document.Title}");
                var createdDocument = await docsService.Documents.Create(document).ExecuteAsync();
                documentId = createdDocument.DocumentId;
                Debug.WriteLine($"Google Doc створено! ID: {documentId}");

                // 4. Підготовка запитів на ВСТАВКУ ТЕКСТУ
                // Ми будемо вставляти текст з кінця до початку, використовуючи InsertTextRequest
                // на позицію 1 (початок тіла документа). Кожна нова вставка буде перед попередньою.

                List<Request> requests = new List<Request>();
                int currentIndex = 1; // Всі вставки будуть на початку тіла документа (body)

                // Функція-помічник для створення запиту на вставку тексту
                Request CreateInsertTextRequest(string textToInsert)
                {
                    return new Request()
                    {
                        InsertText = new InsertTextRequest()
                        {
                            // Вставляємо текст на вказану позицію (завжди 1)
                            Location = new Location() { Index = currentIndex },
                            Text = textToInsert
                        }
                    };
                }

                // Додаємо запити у ЗВОРОТНОМУ порядку (знизу вгору)
                requests.Add(CreateInsertTextRequest($"{Data.City ?? "Місто"}, {Data.Year}\n")); // Місто, Рік + новий рядок
                requests.Add(CreateInsertTextRequest($"\n\n\n")); // Відступ перед містом/роком (3 нових рядки)

                // Керівник
                requests.Add(CreateInsertTextRequest($"{Data.SuperVisorFullName ?? "ПІБ Керівника"}\n"));
                requests.Add(CreateInsertTextRequest($"{Data.SuperVisorPosition ?? "Посада"}\n"));
                requests.Add(CreateInsertTextRequest($"Перевірив:\n")); // Заголовок блоку + новий рядок
                requests.Add(CreateInsertTextRequest($"\n\n")); // Відступ між блоками

                // Виконавець
                requests.Add(CreateInsertTextRequest($"{Data.StudentsFullName ?? "ПІБ Студента"}\n"));
                requests.Add(CreateInsertTextRequest($"студент гр. {Data.Group ?? "Група"}\n"));
                requests.Add(CreateInsertTextRequest($"Виконав:\n")); // Заголовок блоку + новий рядок
                requests.Add(CreateInsertTextRequest($"\n\n\n\n")); // Великий відступ перед виконавцем

                // Блок теми
                requests.Add(CreateInsertTextRequest($"На тему: «{Data.Topic ?? "Тема роботи"}»\n"));
                requests.Add(CreateInsertTextRequest($"по дисципліні «{Data.Discipline ?? "Назва дисципліни"}»\n"));
                requests.Add(CreateInsertTextRequest($"Курсова Робота\n")); // Тип роботи + новий рядок
                requests.Add(CreateInsertTextRequest($"\n\n\n\n\n")); // Великий відступ

                // Кафедра, Факультет
                requests.Add(CreateInsertTextRequest($"Кафедра {Data.Department ?? "Назва Кафедри"}\n"));
                requests.Add(CreateInsertTextRequest($"Факультет {Data.Faculty ?? "Назва Факультету"}\n"));
                requests.Add(CreateInsertTextRequest($"\n\n\n\n\n")); // Великий відступ

                // Міністерство, Університет
                requests.Add(CreateInsertTextRequest($"{Data.University ?? "Назва Університету"}\n"));
                requests.Add(CreateInsertTextRequest($"Міністерство освіти і науки України\n")); // Верхній рядок

                // 5. Створення та виконання запиту BatchUpdate
                if (requests.Count > 0)
                {
                    BatchUpdateDocumentRequest batchUpdateRequest = new BatchUpdateDocumentRequest { Requests = requests };
                    Debug.WriteLine($"Надсилаємо {requests.Count} запитів BatchUpdate для document ID: {documentId}");
                    await docsService.Documents.BatchUpdate(batchUpdateRequest, documentId).ExecuteAsync();
                    Debug.WriteLine($"Текст успішно додано до Google Doc ID: {documentId}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка при генерації або оновленні Google Doc (ID: {documentId ?? "не створено"}): {ex.ToString()}");
                throw; // Повторно кидаємо помилку для обробки у ViewModel
            }
        }
    }
}