using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data; 
using Google.Apis.Services;
using System;
using System.Collections.Generic; 
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
            Debug.WriteLine($"Початок генерації Google Doc з назвою: {baseFileName}");
            string documentId = null; 

            try
            {
                UserCredential credential = await GoogleAuth.GetCredentialsAsync();

                var docsService = new DocsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "CourseWork OOP Title Page Generator"
                });

                var document = new Document { Title = baseFileName };
                Debug.WriteLine($"Надсилаємо запит на створення Google Doc: {document.Title}");
                var createdDocument = await docsService.Documents.Create(document).ExecuteAsync();
                documentId = createdDocument.DocumentId;
                Debug.WriteLine($"Google Doc створено! ID: {documentId}");


                List<Request> requests = new List<Request>();
                int currentIndex = 1; 

                Request CreateInsertTextRequest(string textToInsert)
                {
                    return new Request()
                    {
                        InsertText = new InsertTextRequest()
                        {
                            Location = new Location() { Index = currentIndex },
                            Text = textToInsert
                        }
                    };
                }

                requests.Add(CreateInsertTextRequest($"{Data.City ?? "Місто"}, {Data.Year}\n")); 
                requests.Add(CreateInsertTextRequest($"\n\n\n")); 

                requests.Add(CreateInsertTextRequest($"{Data.SuperVisorFullName ?? "ПІБ Керівника"}\n"));
                requests.Add(CreateInsertTextRequest($"{Data.SuperVisorPosition ?? "Посада"}\n"));
                requests.Add(CreateInsertTextRequest($"Перевірив:\n")); 
                requests.Add(CreateInsertTextRequest($"\n\n")); 

                requests.Add(CreateInsertTextRequest($"{Data.StudentsFullName ?? "ПІБ Студента"}\n"));
                requests.Add(CreateInsertTextRequest($"студент гр. {Data.Group ?? "Група"}\n"));
                requests.Add(CreateInsertTextRequest($"Виконав:\n")); 
                requests.Add(CreateInsertTextRequest($"\n\n\n\n")); 

                requests.Add(CreateInsertTextRequest($"На тему: «{Data.Topic ?? "Тема роботи"}»\n"));
                requests.Add(CreateInsertTextRequest($"по дисципліні «{Data.Discipline ?? "Назва дисципліни"}»\n"));
                requests.Add(CreateInsertTextRequest($"Курсова Робота\n")); 
                requests.Add(CreateInsertTextRequest($"\n\n\n\n\n")); 

                requests.Add(CreateInsertTextRequest($"Кафедра {Data.Department ?? "Назва Кафедри"}\n"));
                requests.Add(CreateInsertTextRequest($"Факультет {Data.Faculty ?? "Назва Факультету"}\n"));
                requests.Add(CreateInsertTextRequest($"\n\n\n\n\n")); 

                requests.Add(CreateInsertTextRequest($"{Data.University ?? "Назва Університету"}\n"));
                requests.Add(CreateInsertTextRequest($"Міністерство освіти і науки України\n")); 

                if (requests.Count > 0)
                {
                    BatchUpdateDocumentRequest batchUpdateRequest = new BatchUpdateDocumentRequest { Requests = requests };
                    Debug.WriteLine($"Надсилаємо {requests.Count} запитів BatchUpdate для document ID: {documentId}");
                    await docsService.Documents.BatchUpdate(batchUpdateRequest, documentId).ExecuteAsync();
                    Debug.WriteLine($"Текст успішно додано до Google Doc IDd: {documentId}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка при генерації або оновленні Google Doc (ID: {documentId ?? "не створено"}): {ex.ToString()}");
                throw; 
            }
        }
    }
}