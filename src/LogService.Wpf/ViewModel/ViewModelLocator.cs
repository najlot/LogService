using System;
using Microsoft.Extensions.DependencyInjection;
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
	public ViewModelLocator()
	{
		var serviceCollection = new ServiceCollection();
		var map = new Najlot.Map.Map().RegisterDataMappings().RegisterViewModelMappings();
		serviceCollection.AddSingleton(map);

		// Register services
		serviceCollection.AddSingleton<IDispatcherHelper, DispatcherHelper>();
		serviceCollection.AddSingleton<INavigationService>(Main);
		serviceCollection.AddSingleton<IErrorService, ErrorService>();
		serviceCollection.AddSingleton<IProfilesService, ProfilesService>();
		serviceCollection.AddSingleton<IMessenger, Messenger>();

		serviceCollection.AddSingleton(c => c.GetRequiredKeyedService<IProfileHandler>(nameof(Source.Local)));
		serviceCollection.AddKeyedSingleton<IProfileHandler, LocalProfileHandler>(nameof(Source.Local));
		serviceCollection.AddKeyedSingleton<IProfileHandler, RestProfileHandler>(nameof(Source.REST));
		serviceCollection.AddKeyedSingleton<IProfileHandler, RmqProfileHandler>(nameof(Source.RMQ));

		serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetUserService());
		serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetLogMessageService());

		// Register views and view models
		serviceCollection.RegisterViewModels();

		serviceCollection.AddSingleton<INavigationService>(Main);

		var serviceProvider = serviceCollection.BuildServiceProvider();

		serviceProvider.GetRequiredService<Najlot.Map.IMap>().RegisterFactory(t =>
		{
			if (t.GetConstructor(Type.EmptyTypes) is not null)
			{
				return Activator.CreateInstance(t) ?? throw new NullReferenceException("Could not create " + t.FullName);
			}

			return serviceProvider.GetRequiredService(t);
		});

		var localProfileHandler = serviceProvider.GetRequiredKeyedService<IProfileHandler>(nameof(Source.Local));
		var restProfileHandler = serviceProvider.GetRequiredKeyedService<IProfileHandler>(nameof(Source.REST));
		var rmqProfileHandler = serviceProvider.GetRequiredKeyedService<IProfileHandler>(nameof(Source.RMQ));
		localProfileHandler.SetNext(restProfileHandler).SetNext(rmqProfileHandler);

		var loginViewModel = serviceProvider.GetRequiredService<LoginViewModel>();
		serviceProvider.GetRequiredService<INavigationService>().NavigateForward(loginViewModel);
	}

	public MainViewModel Main { get; }
}