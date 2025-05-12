using CourseWork_OOP.Interfaces;
using CourseWork_OOP.Services; 
using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;
using Google.Apis.Docs.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork_OOP.Services 
{
    public class Docs : ITitlePageGenerator
    {
        public Docs() { }

        private static string GetText(string? text, string defaultText = "") => text ?? defaultText;
        private static string Indent(int spaces) => new string(' ', spaces);

        public async Task GenerateAsync(TitlePageData data, string argument)
        {
            string documentTitle = argument;
            Debug.WriteLine($"Початок генерації Google Doc з назвою: {documentTitle}");
            string documentId = null;
            int insertionIndex = 1; 

            try
            {
                UserCredential credential = await GoogleAuth.GetCredentialsAsync();
                var docsService = new DocsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "CourseWork OOP Title Page Generator"
                });

                var document = new Document { Title = documentTitle };
                var createdDocument = await docsService.Documents.Create(document).ExecuteAsync();
                documentId = createdDocument.DocumentId;
                Debug.WriteLine($"Google Doc створено! ID: {documentId}");

                List<Request> requests = new List<Request>();

                Action<string, string, bool, bool> AddFormattedInsert = (text, alignment, isBold, isItalic) => {
                    string currentText = string.IsNullOrEmpty(text) ? "\n" : text;

                    requests.Add(new Request { InsertText = new InsertTextRequest { Text = currentText, Location = new Location { Index = insertionIndex } } });

                    if (!(currentText == "\n" && string.IsNullOrWhiteSpace(text))) 
                    {
                        requests.Add(new Request
                        {
                            UpdateParagraphStyle = new UpdateParagraphStyleRequest
                            {
                                Range = new Google.Apis.Docs.v1.Data.Range { StartIndex = insertionIndex, EndIndex = insertionIndex + 1 },
                                ParagraphStyle = new ParagraphStyle { Alignment = alignment },
                                Fields = "alignment"
                            }
                        });
                        requests.Add(new Request
                        {
                            UpdateTextStyle = new UpdateTextStyleRequest
                            {
                                Range = new Google.Apis.Docs.v1.Data.Range { StartIndex = insertionIndex, EndIndex = insertionIndex + 1 },
                                TextStyle = new TextStyle { Bold = isBold, Italic = isItalic }, 
                                Fields = "bold,italic"
                            } 
                        });
                    }
                };

                AddFormattedInsert($"{GetText(data.City)} – {data.Year} рік\n", "CENTER", false, false);
                AddFormattedInsert("\n\n", "START", false, false);

                string sigPlaceholderUser = "______________"; string piiPlaceholderUser = "________________________"; string spacingUser = "       ";
                string subSigUser = "(підпис)"; string subPiiUser = "(прізвище та ініціали)";
                string commissionRowUser1 = $"{sigPlaceholderUser}{spacingUser}{piiPlaceholderUser}\n";
                string commissionRowUser2 = $"{Indent(15)}{subSigUser}{spacingUser}{Indent(6)}{subPiiUser}\n";
                for (int i = 0; i < 3; i++) { AddFormattedInsert(commissionRowUser2, "START", false, true); AddFormattedInsert(commissionRowUser1, "START", false, false); if (i < 2) AddFormattedInsert("\n", "START", false, false); }
                AddFormattedInsert("Члени комісії:\n", "START", false, false);
                AddFormattedInsert("\n", "START", false, false);

                AddFormattedInsert("(національною, кількість балів, ECTS)\n", "START", false, true);
                AddFormattedInsert("_________________________________\n", "START", false, false);
                AddFormattedInsert("Оцінка за шкалою:\n", "START", false, false);
                AddFormattedInsert("\n", "START", false, false);

                AddFormattedInsert("(посада, вчене звання, науковий ступінь, прізвище та ініціали)\n", "START", false, true);
                AddFormattedInsert($"Керівник: {GetText(data.SuperVisorPosition)}, {GetText(data.SuperVisorFullName)}\n", "START", false, false);
                AddFormattedInsert("\n\n", "START", false, false);

                AddFormattedInsert("(прізвище та ініціали)\n", "START", false, true);
                AddFormattedInsert($"{GetText(data.StudentsFullName)}\n", "START", false, false);
                AddFormattedInsert("\n", "START", false, false);
                AddFormattedInsert($"{GetText(data.SpecialtyName, "спеціальності 121 «Інженерія програмного забезпечення»")}\n", "START", false, false);
                string studentLabel = (data.Sex == "Жін") ? "Студентки" : "Студента";
                AddFormattedInsert($"{studentLabel} {GetText(data.CourseNumber, "2")} курсу, групи {GetText(data.Group)}\n", "START", false, false);
                AddFormattedInsert("\n\n\n", "START", false, false);

                string workTypeUser = "КУРСОВА РОБОТА"; if ((data.Discipline ?? "").ToLower().Contains("програмування")) workTypeUser = "КУРСОВА РОБОТА З ОБ’ЄКТНО-ОРІЄНТОВАНОГО ПРОГРАМУВАННЯ";
                AddFormattedInsert($"на тему «{GetText(data.Topic)}»\n", "CENTER", false, true);
                AddFormattedInsert($"{workTypeUser}\n", "CENTER", true, false);
                AddFormattedInsert("\n\n\n\n\n\n\n", "CENTER", false, false);

                AddFormattedInsert($"Кафедра {GetText(data.Department)}\n", "CENTER", false, false);
                AddFormattedInsert($"Факультет {GetText(data.Faculty)}\n", "CENTER", false, false);
                AddFormattedInsert($"{GetText(data.University)}\n", "CENTER", true, false);
                AddFormattedInsert("Міністерство освіти і науки України\n", "CENTER", false, false);
                AddFormattedInsert("\n\n\n", "CENTER", false, false);


                if (requests.Count > 0)
                {
                    requests.Reverse();
                    BatchUpdateDocumentRequest batchUpdateRequest = new BatchUpdateDocumentRequest { Requests = requests };
                    Debug.WriteLine($"Надсилаємо {requests.Count} запитів BatchUpdate для document ID: {documentId}");
                    await docsService.Documents.BatchUpdate(batchUpdateRequest, documentId).ExecuteAsync();
                    Debug.WriteLine($"Документ Google Doc ID: {documentId} оновлено.");
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Помилка при генерації Google Doc: {ex.ToString()}"); throw; }
        }
    }
}