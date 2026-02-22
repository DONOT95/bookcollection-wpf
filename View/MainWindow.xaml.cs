using BookCollection.ViewModel;
using System;
using System.Windows;

namespace BookCollection.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel _vm = new MainWindowViewModel();
            DataContext = _vm;
        }
    }
}
