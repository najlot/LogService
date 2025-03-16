namespace LogService.Maui.View;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class YesNoPageView : ContentPage
{
	public YesNoPageView()
	{
		InitializeComponent();
	}

	protected override bool OnBackButtonPressed()
	{
		// Reject going back with back-button
		return true;
	}
}