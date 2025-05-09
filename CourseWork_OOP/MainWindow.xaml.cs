using CourseWork_OOP.Interfaces;
using CourseWork_OOP.Services; 
using System;
using System.Collections.Generic;
using System.Windows;

namespace CourseWork_OOP
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                List<ITitlePageGenerator> availableGenerators = new List<ITitlePageGenerator>
                {
                    new LaTeX(),
                    new Html(),
                    new PlainText(),
                    new Docs()
                };
                var viewModel = new MainWindowViewModel(availableGenerators);
                this.DataContext = viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при ініціалізації програми: {ex.Message}\n\n{ex.StackTrace}",
                                "Критична помилка запуску", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
