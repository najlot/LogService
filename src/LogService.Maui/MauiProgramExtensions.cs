using LogService.Client.Data;
using LogService.Client.MVVM;
using LogService.Client.MVVM.Services;
using LogService.ClientBase;
using LogService.ClientBase.Models;
using LogService.ClientBase.ProfileHandler;
using LogService.ClientBase.Services;
using LogService.ClientBase.Services.Implementation;
using LogService.Maui.Services;
using LogService.Maui.View;

namespace LogService.Maui;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
    {
		builder.UseMauiApp<App>();

		var map = new Najlot.Map.Map().RegisterDataMappings().RegisterViewModelMappings();
		builder.Services.AddSingleton(map);

		// Register services
		builder.Services.AddSingleton<IDispatcherHelper, DispatcherHelper>();
		builder.Services.AddSingleton<INavigationService, NavigationService>();
		builder.Services.AddSingleton<IErrorService, ErrorService>();
		builder.Services.AddSingleton<IProfilesService, ProfilesService>();
		builder.Services.AddSingleton<IMessenger, Messenger>();

		builder.Services.AddSingleton(c => c.GetRequiredKeyedService<IProfileHandler>(nameof(Source.Local)));
		builder.Services.AddKeyedSingleton<IProfileHandler, LocalProfileHandler>(nameof(Source.Local));
		builder.Services.AddKeyedSingleton<IProfileHandler, RestProfileHandler>(nameof(Source.REST));
		builder.Services.AddKeyedSingleton<IProfileHandler, RmqProfileHandler>(nameof(Source.RMQ));

		builder.Services.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetUserService());
		builder.Services.AddTransient((c) => c.GetRequiredService<IProfileHandler>().GetLogMessageService());

		// Register views and view models
		builder.Services.RegisterViewModels();

		builder.Services.AddSingleton<LoginView>();
		builder.Services.AddSingleton(c => new NavigationPage(c.GetRequiredService<LoginView>()));
		builder.Services.AddSingleton(c => c.GetRequiredService<LoginView>().Dispatcher);
		builder.Services.AddSingleton(c => c.GetRequiredService<NavigationPage>().Navigation);
		
		return builder;
    }
}
