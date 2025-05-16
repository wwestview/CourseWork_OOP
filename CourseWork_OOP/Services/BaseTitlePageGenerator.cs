using CourseWork_OOP.Interfaces;
using CourseWork_OOP.Services; 
using System.Threading.Tasks;

namespace CourseWork_OOP.Services
{
    public abstract class BaseTitlePageGenerator : ITitlePageGenerator
    {
        public abstract string FormatName { get; }
        public abstract string FileExtension { get; }

        protected static string GetText(string? text, string defaultText = "")
        {
            return string.IsNullOrWhiteSpace(text) ? defaultText : text;
        }

        public abstract Task GenerateAsync(TitlePageData data, string argument);
    }
}