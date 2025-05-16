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
        private const double NORMAL_FONT_SIZE = 14.0;
        private const double SMALL_FONT_SIZE = 8;
        private const string DEFAULT_FONT = "Times New Roman";

        public Docs() { }

        private static string GetText(string? text, string defaultText = "") =>
            string.IsNullOrWhiteSpace(text) ? defaultText : text;

        private int AddStyledParagraphSequential(
            List<Request> requests,
            int currentIndex,
            string text,
            string? alignment = null,
            bool bold = false,
            double? fontSize = null,
            bool italic = false,
            bool underline = false,
            double? spaceBefore = null,
            double? spaceAfter = null,
            string? fontName = DEFAULT_FONT)
        {
            string normalizedText = text ?? "";
            normalizedText = normalizedText.Replace("\r\n", "\n").Replace("\r", "\n");

            string textToInsert;
            if (string.IsNullOrEmpty(normalizedText) && text != "\n")
            {
                textToInsert = "\n";
            }
            else if (text == "\n")
            {
                textToInsert = "\n";
            }
            else
            {
                textToInsert = normalizedText.EndsWith("\n") ? normalizedText : normalizedText + "\n";
            }

            int textLength = textToInsert.Length;
            int textContentLength;

            if (textToInsert == "\n")
            {
                textContentLength = 0;
            }
            else
            {
                textContentLength = textLength - 1;
            }

            Debug.WriteLine($"[Docs.AddStyledParagraphSequential] Index: {currentIndex}, OrigText: \"{text?.Replace("\r", "\\r").Replace("\n", "\\n")}\", NormText: \"{normalizedText.Replace("\n", "\\n")}\", ToInsert: \"{textToInsert.Replace("\n", "\\n")}\", textLength: {textLength}, textContentLength: {textContentLength}");

            requests.Add(new Request
            {
                InsertText = new InsertTextRequest
                {
                    Text = textToInsert,
                    Location = new Location { Index = currentIndex }
                }
            });

            if (textContentLength > 0 || (textToInsert == "\n" && (bold || italic || underline || fontSize.HasValue)))
            {
                var textStyle = new TextStyle();
                var textStyleFields = new List<string>();

                textStyle.WeightedFontFamily = new WeightedFontFamily { FontFamily = fontName ?? DEFAULT_FONT };
                textStyleFields.Add("weightedFontFamily");

                textStyle.FontSize = new Dimension { Magnitude = fontSize ?? NORMAL_FONT_SIZE, Unit = "PT" };
                textStyleFields.Add("fontSize");

                if (bold) { textStyle.Bold = true; textStyleFields.Add("bold"); }
                if (italic) { textStyle.Italic = true; textStyleFields.Add("italic"); }
                if (underline) { textStyle.Underline = true; textStyleFields.Add("underline"); }

                if (textStyleFields.Any())
                {
                    requests.Add(new Request
                    {
                        UpdateTextStyle = new UpdateTextStyleRequest
                        {
                            Range = new Google.Apis.Docs.v1.Data.Range { StartIndex = currentIndex, EndIndex = currentIndex + (textContentLength > 0 ? textContentLength : 1) },
                            TextStyle = textStyle,
                            Fields = string.Join(",", textStyleFields.Distinct())
                        }
                    });
                }
            }

            var paragraphStyle = new ParagraphStyle();
            var paragraphStyleFields = new List<string>();
            if (alignment != null) { paragraphStyle.Alignment = alignment.ToUpperInvariant(); paragraphStyleFields.Add("alignment"); }
            if (spaceBefore.HasValue) { paragraphStyle.SpaceAbove = new Dimension { Magnitude = spaceBefore.Value, Unit = "PT" }; paragraphStyleFields.Add("spaceAbove"); }
            if (spaceAfter.HasValue) { paragraphStyle.SpaceBelow = new Dimension { Magnitude = spaceAfter.Value, Unit = "PT" }; paragraphStyleFields.Add("spaceBelow"); }

            if (paragraphStyleFields.Any())
            {
                requests.Add(new Request
                {
                    UpdateParagraphStyle = new UpdateParagraphStyleRequest
                    {
                        Range = new Google.Apis.Docs.v1.Data.Range { StartIndex = currentIndex, EndIndex = currentIndex + textLength },
                        ParagraphStyle = paragraphStyle,
                        Fields = string.Join(",", paragraphStyleFields.Distinct())
                    }
                });
            }
            return currentIndex + textLength;
        }

        public async Task GenerateAsync(TitlePageData data, string documentTitle)
        {
            Debug.WriteLine($"[Docs.GenerateAsync] Початок генерації Google Doc з назвою: \"{documentTitle}\"");
            string? documentId = null;

            try
            {
                UserCredential credential = await GoogleAuth.GetCredentialsAsync();
                var docsService = new DocsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "CourseWork OOP Title Page Generator"
                });

                var document = new Document { Title = documentTitle };
                Debug.WriteLine($"[Docs.GenerateAsync] Створення документа Google з назвою: \"{document.Title}\"");
                var createdDocument = await docsService.Documents.Create(document).ExecuteAsync();
                documentId = createdDocument.DocumentId;
                Debug.WriteLine($"[Docs.GenerateAsync] Google Doc успішно створено! ID: {documentId}.");

                List<Request> requests = new List<Request>();
                int currentIndex = 1;

                currentIndex = AddStyledParagraphSequential(requests, currentIndex, "Міністерство освіти і науки України", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, GetText(data.University, "НАЗВА УНІВЕРСИТЕТУ"), alignment: "CENTER", fontSize: NORMAL_FONT_SIZE);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, $"Факультет {GetText(data.Faculty, "Факультет не вказано")}", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, $"Кафедра {GetText(data.Department, "Кафедру не вказано")}", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE, spaceAfter: 25);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, "КУРСОВА РОБОТА З ДИСЦИПЛІНИ", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, $"«{GetText(data.Discipline, "НАЗВА ДИСЦИПЛІНИ").ToUpperInvariant()}»", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, $"на тему: «{GetText(data.Topic, "Тема не вказана").ToUpperInvariant()}»", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE, spaceAfter: 50);
                string studentLabel = (GetText(data.Sex) == "Жін") ? "Студентки" : "Студента";
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, $"{studentLabel} {GetText(data.CourseNumber, "X")} курсу, групи {GetText(data.Group, "XXXX")}", alignment: "END", fontSize: NORMAL_FONT_SIZE);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, GetText(data.SpecialtyName, "Спеціальність не вказана"), alignment: "END", fontSize: NORMAL_FONT_SIZE);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, GetText(data.StudentsFullName, "ПІБ Студента"), alignment: "END", fontSize: NORMAL_FONT_SIZE, spaceAfter: 18);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, $"Керівник: {GetText(data.SuperVisorPosition, "Посада керівника")}, {GetText(data.SuperVisorFullName, "ПІБ Керівника")}", alignment: "END", fontSize: NORMAL_FONT_SIZE);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, "\t(посада, вчене звання, науковий ступінь, прізвище та ініціали)", alignment: "END", fontSize: SMALL_FONT_SIZE, spaceAfter: 30);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, "Оцінка за шкалою: " + new string('_', 22), alignment: "END", fontSize: NORMAL_FONT_SIZE);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, "(національною, кількість балів, ECTS)"+"  ", alignment: "END", fontSize: SMALL_FONT_SIZE, spaceAfter: 30);

                currentIndex = AddStyledParagraphSequential(requests, currentIndex, "Члени комісії:",
                    alignment: "START", 
                    fontSize: NORMAL_FONT_SIZE,
                    spaceAfter: 8);

                string fixedSignatureLine = new string('_', 25);    
                string piiPlaceholderLine = new string('_', 25);    
                string signatureLabelText = "\t\t\t\t\t\t(підпис)";
                string tabsToShiftPii = "\t\t\t\t"; 

                for (int i = 0; i < 3; i++)
                {
                    string piiFieldText; 
                    if (data.CommissionMemberNames != null && i < data.CommissionMemberNames.Count && !string.IsNullOrWhiteSpace(data.CommissionMemberNames[i]))
                    {
                        piiFieldText = GetText(data.CommissionMemberNames[i]);
                    }
                    else
                    {
                        piiFieldText = piiPlaceholderLine; 
                    }

                    string line1Text = $"{tabsToShiftPii}{fixedSignatureLine}{"   "}{piiFieldText}";
                    currentIndex = AddStyledParagraphSequential(requests, currentIndex, line1Text,
                        alignment: "START", 
                        fontSize: NORMAL_FONT_SIZE,
                        spaceAfter: 1); 

                    int labelIndentSpaces = 0;
                    if (fixedSignatureLine.Length > signatureLabelText.Length)
                    {
                        labelIndentSpaces = (fixedSignatureLine.Length - signatureLabelText.Length) / 2;
                    }
                    if (labelIndentSpaces < 0) labelIndentSpaces = 0;

                    string labelLineText = $"{new string(' ', labelIndentSpaces)}{signatureLabelText}";

                    currentIndex = AddStyledParagraphSequential(requests, currentIndex, labelLineText,
                        alignment: "START", 
                        fontSize: SMALL_FONT_SIZE,
                        spaceAfter: (i < 2) ? 10.0 : 4.0);
                }

                currentIndex = AddStyledParagraphSequential(requests, currentIndex, "", spaceBefore: 60);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, GetText(data.City, "Місто") + " – " + GetText(data.Year.ToString(), DateTime.Now.Year.ToString()) + " рік", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE);
                currentIndex = AddStyledParagraphSequential(requests, currentIndex, "", spaceAfter: 20);

                Debug.WriteLine($"[Docs.GenerateAsync] Кількість сформованих запитів: {requests.Count}");

                if (requests.Any())
                {
                    BatchUpdateDocumentRequest batchUpdateRequest = new BatchUpdateDocumentRequest { Requests = requests };
                    Debug.WriteLine($"[Docs.GenerateAsync] Надсилаємо BatchUpdate з {requests.Count} запитами для document ID: {documentId}");

                    Google.Apis.Docs.v1.Data.BatchUpdateDocumentResponse? response = null;
                    try
                    {
                        response = await docsService.Documents.BatchUpdate(batchUpdateRequest, documentId).ExecuteAsync();
                        Debug.WriteLine($"[Docs.GenerateAsync] BatchUpdate виконано (або не викликало винятку). Document ID: {documentId}.");
                        if (response != null)
                        {
                            Debug.WriteLine($"[Docs.GenerateAsync] Відповідь BatchUpdate: DocumentId='{response.DocumentId}', WriteControl={response.WriteControl != null}, Replies.Count={response.Replies?.Count ?? 0}");
                        }
                    }
                    catch (Google.GoogleApiException apiEx)
                    {
                        Debug.WriteLine($"[Docs.GenerateAsync] GoogleApiException під час BatchUpdate для ID {documentId}: {apiEx.Message}");
                        Debug.WriteLine($"[Docs.GenerateAsync] Повний текст помилки GoogleApiException: {apiEx.ToString()}");
                        if (apiEx.Error != null)
                        {
                            Debug.WriteLine($"[Docs.GenerateAsync] Помилка сервісу: {apiEx.Error.Message}, Код: {apiEx.Error.Code}");
                            if (apiEx.Error.Errors != null)
                            {
                                foreach (var errDetail in apiEx.Error.Errors)
                                {
                                    Debug.WriteLine($"  - Деталь помилки: Domain={errDetail.Domain}, Reason={errDetail.Reason}, Message={errDetail.Message}");
                                }
                            }
                        }
                        throw;
                    }
                    Debug.WriteLine($"[Docs.GenerateAsync] Запит на оновлення документа {documentId} надіслано.");
                }
                else
                {
                    Debug.WriteLine("[Docs.GenerateAsync] Список запитів порожній. Оновлення не буде надіслано. Документ залишиться порожнім, якщо він щойно створений.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Docs.GenerateAsync] Загальна помилка при генерації Google Doc (назва: \"{documentTitle}\"), ID документа (якщо створено): {documentId}. Помилка: {ex.ToString()}");
                throw;
            }
        }
    }
}