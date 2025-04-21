using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web;
using CourseWork_OOP.Services;

namespace CourseWork_OOP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                this.DataContext = new MainWindowViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при ініціалізації ViewModel: {ex.Message}\n\n{ex.StackTrace}",
                                "Критична помилка DataContext", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
       

    }
    
    
}