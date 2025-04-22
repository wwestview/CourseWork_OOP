// File: GoogleAuth.cs
using Google.Apis.Auth.OAuth2;
<<<<<<< HEAD
using Google.Apis.Docs.v1;      // Для DocsService.Scope
using Google.Apis.Sheets.v4;    // Для SheetsService.Scope
using Google.Apis.Util.Store;   // Для FileDataStore
=======
using Google.Apis.Docs.v1;      
using Google.Apis.Sheets.v4;    
using Google.Apis.Util.Store;   
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CourseWork_OOP.Services
{
    public static class GoogleAuth
    {
        private static readonly string PathToClientSecrets = "client_secrets.json";
        private static readonly string PathToTokenDataStore = "GoogleApiTokens";

<<<<<<< HEAD
        // Області доступу (scopes) з вашого коду
        // Переконайтесь, що SheetsService.Scope.Spreadsheets - це те, що вам потрібно
        private static readonly string[] Scopes = {
            DocsService.Scope.Documents,        // Дозвіл на роботу з Google Docs
            SheetsService.Scope.Spreadsheets  // Дозвіл на читання ТА ЗАПИС у Google Sheets
=======
        private static readonly string[] Scopes = {
            DocsService.Scope.Documents,      
            SheetsService.Scope.Spreadsheets  
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
        };

        public static async Task<UserCredential> GetCredentialsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!File.Exists(PathToClientSecrets))
                {
                    throw new FileNotFoundException($"Файл секретів '{PathToClientSecrets}' не знайдено...", PathToClientSecrets);
                }

<<<<<<< HEAD
                GoogleClientSecrets googleSecrets; // Змінили назву для ясності
                using (var stream = new FileStream(PathToClientSecrets, FileMode.Open, FileAccess.Read))
                {
                    // Завантажуємо контейнер секретів
                    googleSecrets = await GoogleClientSecrets.FromStreamAsync(stream, cancellationToken);
                }

                // !!! ВИПРАВЛЕНО ТУТ: Передаємо внутрішній об'єкт secrets.Installed !!!
                // Оскільки ви створювали облікові дані для "Desktop app", використовуємо Installed.
                // Якби це був веб-додаток, використовували б googleSecrets.Web
                UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    googleSecrets.Secrets, // <--- ВИПРАВЛЕНО
=======
                GoogleClientSecrets googleSecrets; 
                using (var stream = new FileStream(PathToClientSecrets, FileMode.Open, FileAccess.Read))
                {
                    googleSecrets = await GoogleClientSecrets.FromStreamAsync(stream, cancellationToken);
                }

                UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    googleSecrets.Secrets, 
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
                    Scopes,
                    "user",
                    cancellationToken,
                    new FileDataStore(PathToTokenDataStore, true)
                );

                Console.WriteLine("Автентифікація Google успішна.");
<<<<<<< HEAD
                // System.Diagnostics.Debug.WriteLine("Автентифікація Google успішна."); // Краще для WPF
=======
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
                return credential;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
<<<<<<< HEAD
                // System.Diagnostics.Debug.WriteLine($"Помилка: {ex.Message}");
=======
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
                throw;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Помилка автентифікації Google (AggregateException):");
                foreach (var e in ex.InnerExceptions) { Console.WriteLine($"- {e.Message}"); }
<<<<<<< HEAD
                // System.Diagnostics.Debug.WriteLine($"Помилка автентифікації Google (AggregateException): {ex.InnerExceptions.FirstOrDefault()?.Message}");
=======
>>>>>>> 5740e0c410773f82d0abade7996c5d53f3dc7b8d
                throw new Exception("Помилка під час автентифікації Google.", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Невідома помилка автентифікації Google: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Google Auth Error: {ex.ToString()}");
                throw new Exception("Невідома помилка під час автентифікації Google.", ex);
            }
        }
    }
}