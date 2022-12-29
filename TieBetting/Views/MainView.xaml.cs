namespace TieBetting.Views;

public partial class MainView : ContentPage
{
	public MainView(MainViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
    }

    protected override bool OnBackButtonPressed()
    {
        if (Application.Current != null) 
            Application.Current.Quit();

        return true;
    }
}