using System.Windows.Input;
namespace BatteryLogix.ViewModels;
public sealed class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? p) => canExecute?.Invoke() ?? true;
    public void Execute(object? p) => execute();
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
