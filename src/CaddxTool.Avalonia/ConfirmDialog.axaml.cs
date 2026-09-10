using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CaddxTool.Avalonia;

public partial class ConfirmDialog : Window
{
    public ConfirmDialog()
    {
        InitializeComponent();
    }

    public static Task<bool> Ask(Window owner, string message)
    {
        var dialog = new ConfirmDialog();
        dialog.FindControl<TextBlock>("TxtMessage")!.Text = message;
        return dialog.ShowDialog<bool>(owner);
    }

    private void OnYesClick(object? sender, RoutedEventArgs e) => Close(true);

    private void OnNoClick(object? sender, RoutedEventArgs e) => Close(false);
}
