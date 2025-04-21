// File: GoogleAuth.cs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Docs.v1;      
using Google.Apis.Sheets.v4;    
using Google.Apis.Util.Store;   
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

        private static readonly string[] Scopes = {
            DocsService.Scope.Documents,      
            SheetsService.Scope.Spreadsheets  
        };

        public static async Task<UserCredential> GetCredentialsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!File.Exists(PathToClientSecrets))
                {
                    throw new FileNotFoundException($"Файл секретів '{PathToClientSecrets}' не знайдено...", PathToClientSecrets);
                }

                GoogleClientSecrets googleSecrets; 
                using (var stream = new FileStream(PathToClientSecrets, FileMode.Open, FileAccess.Read))
                {
                    googleSecrets = await GoogleClientSecrets.FromStreamAsync(stream, cancellationToken);
                }

                UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    googleSecrets.Secrets, 
                    Scopes,
                    "user",
                    cancellationToken,
                    new FileDataStore(PathToTokenDataStore, true)
                );

                Console.WriteLine("Автентифікація Google успішна.");
                return credential;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
                throw;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Помилка автентифікації Google (AggregateException):");
                foreach (var e in ex.InnerExceptions) { Console.WriteLine($"- {e.Message}"); }
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