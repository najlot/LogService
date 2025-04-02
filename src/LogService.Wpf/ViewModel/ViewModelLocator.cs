using Microsoft.Extensions.DependencyInjection;
using Najlot.Map;
using LogService.Client.Data;
using LogService.Client.MVVM;
using LogService.Client.MVVM.Services;
using LogService.ClientBase;
using LogService.ClientBase.ProfileHandler;
using LogService.ClientBase.Services;
using LogService.ClientBase.Services.Implementation;
using LogService.ClientBase.ViewModel;
using LogService.ClientBase.Models;

namespace LogService.Wpf.ViewModel;

public class ViewModelLocator
{
	/// <summary>
	/// Initializes a new instance of the ViewModelLocator class.
	/// </summary>
	public ViewModelLocator()
	{
		var messenger = new Messenger();
		var dispatcher = new DispatcherHelper();
		var serviceCollection = new ServiceCollection();
		var map = new Map().RegisterDataMappings().RegisterViewModelMappings();
		serviceCollection.AddSingleton(map);

		// Register services
		serviceCollection.AddSingleton<IErrorService>(errorService);
		serviceCollection.AddSingleton<IProfilesService, ProfilesService>();
		serviceCollection.AddSingleton<IMessenger>(messenger);

		var profileHandler = new LocalProfileHandler(messenger, dispatcher, map);
		profileHandler
			.SetNext(new RestProfileHandler(messenger, dispatcher, errorService, map))
			.SetNext(new RmqProfileHandler(messenger, dispatcher, errorService, map));

		serviceCollection.AddSingleton<IProfileHandler>(profileHandler);
		serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetUserService());
		serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetLogMessageService());

		serviceCollection.RegisterViewModels();

		serviceCollection.AddSingleton<INavigationService>(Main);

		var serviceProvider = serviceCollection.BuildServiceProvider();

		Main.NavigateForward(serviceProvider.GetRequiredService<LoginViewModel>());
	}

	public MainViewModel Main { get; }
}