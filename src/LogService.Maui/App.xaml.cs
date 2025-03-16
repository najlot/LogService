[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace LogService.Maui;

public partial class App : Application
{
	private readonly Page _firstPage;

	public App(IServiceProvider serviceProvider)
	{
		InitializeComponent();
		_firstPage = serviceProvider.GetRequiredService<NavigationPage>();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(_firstPage);
	}
}