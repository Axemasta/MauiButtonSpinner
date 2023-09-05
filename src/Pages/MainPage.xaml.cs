using ButtonSpinner.ViewModels;

namespace ButtonSpinner.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		this.BindingContext = viewModel;

		InitializeComponent();
	}
}