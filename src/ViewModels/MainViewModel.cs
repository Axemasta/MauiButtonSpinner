using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace ButtonSpinner.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [RelayCommand]
    public async Task DoSomething()
    {
        Debug.WriteLine("Starting long running command...");

        await Task.Delay(2000);

        Debug.WriteLine("Ending long running command...");
    }

    [RelayCommand]
    public async Task DoSomethingWithParameter(string parameter)
    {
        Debug.WriteLine($"Starting long running command with argument: {parameter}...");

        await Task.Delay(2000);

        Debug.WriteLine($"Ending long running command with argument: {parameter}...");
    }
}
