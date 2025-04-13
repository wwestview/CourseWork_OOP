using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseWork_OOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
    /// <summary>
    /// Stores the data required to generate the title page of a term paper.
    /// </summary>
    public class CoverPageData
    {
        public string? StudentsFullName { get; set; }
        public string? SuperVisorFullName { get; set; }
        public string? SuperVisorPosition { get; set; }
        public string? Group {  get; set; }
        public string? Topic { get; set; }
        public string? Discipline { get; set; }
        public string? University { get; set; }
        public string? Faculty { get; set; }
        public string? Department { get; set; }
        public string? City { get; set; } = "Черкаси";
        public int Year { get; set; } = DateTime.Now.Year;
    }

}