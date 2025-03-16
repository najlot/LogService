using Microsoft.Extensions.DependencyInjection;
using LogService.ClientBase.ViewModel;
using System;

namespace LogService.ClientBase;

public static class ServiceCollectionExtensions
{
	public static void RegisterViewModels(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<LoginViewModel>();
		serviceCollection.AddTransient<ProfileViewModel>();
		serviceCollection.AddSingleton<Func<ProfileViewModel>>(c => () => c.GetRequiredService<ProfileViewModel>());
		serviceCollection.AddTransient<LoginProfileViewModel>();
		serviceCollection.AddSingleton<Func<LoginProfileViewModel>>(c => () => c.GetRequiredService<LoginProfileViewModel>());
		serviceCollection.AddScoped<MenuViewModel>();

		serviceCollection.AddScoped<AllLogMessagesViewModel>();

		serviceCollection.AddTransient<LogArgumentViewModel>();
		serviceCollection.AddSingleton<Func<LogArgumentViewModel>>(c => () => c.GetRequiredService<LogArgumentViewModel>());
		serviceCollection.AddTransient<LogMessageViewModel>();
		serviceCollection.AddSingleton<Func<LogMessageViewModel>>(c => () => c.GetRequiredService<LogMessageViewModel>());
	}
}