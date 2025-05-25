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
    public class Docs : BaseTitlePageGenerator
    {
        private const double NORMAL_FONT_SIZE = 14.0;
        private const double SMALL_FONT_SIZE = 8;
        private const string DEFAULT_FONT = "Times New Roman";

        public override string FormatName => "GoogleDoc";
        public override string FileExtension => "";

        private List<Request> _requests;
        private int _currentIndex;
        private string _documentTitleForCreation;

        public Docs() { }

        private void AddStyledParagraph(
            string text,
            string? alignment = null, bool bold = false, double? fontSize = null,
            bool italic = false, bool underline = false, double? spaceBefore = null,
            double? spaceAfter = null, string? fontName = DEFAULT_FONT)
        {
            string normalizedText = text ?? "";
            normalizedText = normalizedText.Replace("\r\n", "\n").Replace("\r", "\n");
            string textToInsert;
            if (string.IsNullOrEmpty(normalizedText) && text != "\n") { textToInsert = "\n"; }
            else if (text == "\n") { textToInsert = "\n"; }
            else { textToInsert = normalizedText.EndsWith("\n") ? normalizedText : normalizedText + "\n"; }

            int textLength = textToInsert.Length;
            int textContentLength = (textToInsert == "\n") ? 0 : textLength - 1;

            Debug.WriteLine($"[Docs.AddStyledParagraph] Index: {this._currentIndex}, ToInsert: \"{textToInsert.Replace("\n", "\\n")}\"");

            this._requests.Add(new Request { InsertText = new InsertTextRequest { Text = textToInsert, Location = new Location { Index = this._currentIndex } } });

            if (textContentLength > 0 || (textToInsert == "\n" && (bold || italic || underline || fontSize.HasValue)))
            {
                var textStyle = new TextStyle();
                var textStyleFields = new List<string>();
                textStyle.WeightedFontFamily = new WeightedFontFamily { FontFamily = fontName ?? DEFAULT_FONT }; textStyleFields.Add("weightedFontFamily");
                textStyle.FontSize = new Dimension { Magnitude = fontSize ?? NORMAL_FONT_SIZE, Unit = "PT" }; textStyleFields.Add("fontSize");
                if (bold) { textStyle.Bold = true; textStyleFields.Add("bold"); }
                if (italic) { textStyle.Italic = true; textStyleFields.Add("italic"); }
                if (underline) { textStyle.Underline = true; textStyleFields.Add("underline"); }
                if (textStyleFields.Any())
                {
                    this._requests.Add(new Request { UpdateTextStyle = new UpdateTextStyleRequest { Range = new Google.Apis.Docs.v1.Data.Range { StartIndex = this._currentIndex, EndIndex = this._currentIndex + (textContentLength > 0 ? textContentLength : 1) }, TextStyle = textStyle, Fields = string.Join(",", textStyleFields.Distinct()) } });
                }
            }
            var paragraphStyle = new ParagraphStyle();
            var paragraphStyleFields = new List<string>();
            if (alignment != null) { paragraphStyle.Alignment = alignment.ToUpperInvariant(); paragraphStyleFields.Add("alignment"); }
            if (spaceBefore.HasValue) { paragraphStyle.SpaceAbove = new Dimension { Magnitude = spaceBefore.Value, Unit = "PT" }; paragraphStyleFields.Add("spaceAbove"); }
            if (spaceAfter.HasValue) { paragraphStyle.SpaceBelow = new Dimension { Magnitude = spaceAfter.Value, Unit = "PT" }; paragraphStyleFields.Add("spaceBelow"); }
            if (paragraphStyleFields.Any())
            {
                this._requests.Add(new Request { UpdateParagraphStyle = new UpdateParagraphStyleRequest { Range = new Google.Apis.Docs.v1.Data.Range { StartIndex = this._currentIndex, EndIndex = this._currentIndex + textLength }, ParagraphStyle = paragraphStyle, Fields = string.Join(",", paragraphStyleFields.Distinct()) } });
            }
            this._currentIndex += textLength;
        }

        protected override void InitializeInternalContext(TitlePageData data, string argument)
        {
            this._requests = new List<Request>();
            this._currentIndex = 1;
            this._documentTitleForCreation = argument;
            Debug.WriteLine($"[{FormatName}.InitializeInternalContext] Context initialized. Document title: \"{_documentTitleForCreation}\"");
        }

        protected override void BuildHeader(TitlePageData data)
        {
            AddStyledParagraph("Міністерство освіти і науки України", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE);
            AddStyledParagraph(GetText(data.University, "НАЗВА УНІВЕРСИТЕТУ"), alignment: "CENTER", fontSize: NORMAL_FONT_SIZE);
        }

        protected override void BuildFacultyAndDepartment(TitlePageData data)
        {
            AddStyledParagraph($"Факультет {GetText(data.Faculty, "Факультет не вказано")}", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE);
            AddStyledParagraph($"Кафедра {GetText(data.Department, "Кафедру не вказано")}", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE, spaceAfter: 25);
        }

        protected override void BuildWorkTitle(TitlePageData data)
        {
            AddStyledParagraph($"КУРСОВА РОБОТА З «{GetText(data.Discipline, "НАЗВА ДИСЦИПЛІНИ").ToUpperInvariant()}»", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE); 
            AddStyledParagraph($"на тему: «{GetText(data.Topic, "Тема не вказана")}»", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE, spaceAfter: 50); 
        }

        protected override void BuildStudentAndSupervisor(TitlePageData data)
        {
            string studentLabel = (GetText(data.Sex) == "Жін") ? "Студентки" : "Студента";
            AddStyledParagraph($"{studentLabel} {GetText(data.CourseNumber, "X")} курсу, групи {GetText(data.Group, "XXXX")}", alignment: "END", fontSize: NORMAL_FONT_SIZE);
            AddStyledParagraph(GetText(data.SpecialtyName, "Спеціальність не вказана"), alignment: "END", fontSize: NORMAL_FONT_SIZE);
            AddStyledParagraph(GetText(data.StudentsFullName, "ПІБ Студента"), alignment: "END", fontSize: NORMAL_FONT_SIZE, spaceAfter: 18); 
            AddStyledParagraph($"Керівник: {GetText(data.SuperVisorPosition, "Посада керівника")}, {GetText(data.SuperVisorFullName, "ПІБ Керівника")}", alignment: "END", fontSize: NORMAL_FONT_SIZE);
            AddStyledParagraph("\t(посада, вчене звання, науковий ступінь, прізвище та ініціали)", alignment: "END", fontSize: SMALL_FONT_SIZE, spaceAfter: 30);
        }

        protected override void BuildEvaluation(TitlePageData data)
        {
            AddStyledParagraph("Оцінка за шкалою: " + new string('_', 22), alignment: "END", fontSize: NORMAL_FONT_SIZE);
            AddStyledParagraph("(національною, кількість балів, ECTS)" + "  ", alignment: "END", fontSize: SMALL_FONT_SIZE, spaceAfter: 30);
        }

        protected override void BuildCommissionMembers(TitlePageData data)
        {
            AddStyledParagraph("Члени комісії:", alignment: "START", fontSize: NORMAL_FONT_SIZE, spaceAfter: 8);
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
                string line1Text = $"{tabsToShiftPii}{fixedSignatureLine}{"    "}{piiFieldText}";
                AddStyledParagraph(line1Text, alignment: "START", fontSize: NORMAL_FONT_SIZE, spaceAfter: 1);

                int labelIndentSpaces = 0;
                if (fixedSignatureLine.Length > signatureLabelText.Length)
                { 
                    labelIndentSpaces = (fixedSignatureLine.Length - signatureLabelText.Length) / 2;
                }
                if (labelIndentSpaces < 0) labelIndentSpaces = 0;
                string labelLineText = $"{new string(' ', labelIndentSpaces)}{signatureLabelText}";
                AddStyledParagraph(labelLineText, alignment: "START", fontSize: SMALL_FONT_SIZE, spaceAfter: (i < 2) ? 10.0 : 4.0);
            }
        }

        protected override void BuildFooter(TitlePageData data)
        {
            AddStyledParagraph("", spaceBefore: 60);
            AddStyledParagraph(GetText(data.City, "Місто") + " – " + GetText(data.Year.ToString(), DateTime.Now.Year.ToString()) + " рік", alignment: "CENTER", fontSize: NORMAL_FONT_SIZE);
            AddStyledParagraph("", spaceAfter: 20);
        }

        protected override async Task FinalizeAndPersistOutputAsync(TitlePageData data, string argument)
        {
            Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] Finalizing Google Doc. Title: \"{_documentTitleForCreation}\"");
            string? currentDocumentId = null;
            try
            {
                UserCredential credential = await GoogleAuth.GetCredentialsAsync();
                var docsService = new DocsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "CourseWork OOP Title Page Generator"
                });

                var document = new Document { Title = _documentTitleForCreation };
                Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] Creating document: \"{document.Title}\"");
                var createdDocument = await docsService.Documents.Create(document).ExecuteAsync();
                currentDocumentId = createdDocument.DocumentId;
                Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] Google Doc created with ID: {currentDocumentId}");

                if (_requests.Any())
                {
                    BatchUpdateDocumentRequest batchUpdateRequest = new BatchUpdateDocumentRequest { Requests = _requests };
                    Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] Sending BatchUpdate with {_requests.Count} requests for document ID: {currentDocumentId}");
                    await docsService.Documents.BatchUpdate(batchUpdateRequest, currentDocumentId).ExecuteAsync();
                    Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] Google Doc ID: {currentDocumentId} updated.");
                }
                else
                {
                    Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] No requests to send. Document might be blank.");
                }
            }
            catch (Google.GoogleApiException apiEx)
            {
                Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] GoogleApiException: ID={currentDocumentId}, Error={apiEx.ToString()}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{FormatName}.FinalizeAndPersistOutputAsync] General error: ID={currentDocumentId}, Error={ex.ToString()}");
                throw;
            }
        }
    }
}