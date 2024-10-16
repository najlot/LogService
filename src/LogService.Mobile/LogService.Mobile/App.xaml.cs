﻿using Microsoft.Extensions.DependencyInjection;
using LogService.Client.MVVM;
using LogService.Client.MVVM.Services;
using LogService.ClientBase;
using LogService.ClientBase.ProfileHandler;
using LogService.ClientBase.Services;
using LogService.ClientBase.Services.Implementation;
using LogService.ClientBase.ViewModel;
using LogService.Mobile.View;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace LogService.Mobile
{
	public partial class App : Application
	{
		private static NavigationServicePage _navigationPage;

		public App()
		{
			if (_navigationPage == null)
			{
				var loginView = new LoginView();
				_navigationPage = new NavigationServicePage(loginView);

				var messenger = new Messenger();
				var dispatcher = new DispatcherHelper();
				var serviceCollection = new ServiceCollection();
				var errorService = new ErrorService(_navigationPage);

				serviceCollection.AddSingleton<IDispatcherHelper, DispatcherHelper>();

				// Register services
				serviceCollection.AddSingleton<IErrorService>(errorService);
				serviceCollection.AddSingleton<IProfilesService, ProfilesService>();
				serviceCollection.AddSingleton<IMessenger>(messenger);

				var profileHandler = new LocalProfileHandler(messenger, dispatcher);
				profileHandler
					.SetNext(new RestProfileHandler(messenger, dispatcher, errorService))
					.SetNext(new RmqProfileHandler(messenger, dispatcher, errorService));

				serviceCollection.AddSingleton<IProfileHandler>(profileHandler);
				serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetUserService());
				serviceCollection.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetLogMessageService());

				serviceCollection.RegisterViewModels();

				serviceCollection.AddSingleton<INavigationService>(_navigationPage);

				var serviceProvider = serviceCollection.BuildServiceProvider();

				loginView.BindingContext = serviceProvider.GetRequiredService<LoginViewModel>();
			}

			MainPage = _navigationPage;
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
