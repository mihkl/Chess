using Avalonia.Controls;
using Male.ViewModels;

namespace Male.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
