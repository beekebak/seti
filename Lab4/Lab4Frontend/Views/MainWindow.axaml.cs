using Avalonia.Controls;
using Lab4Frontend.ViewModels;

namespace Lab4Frontend.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}