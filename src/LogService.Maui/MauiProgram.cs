using LogService.ClientBase.Models;
using LogService.ClientBase.ProfileHandler;
using LogService.ClientBase.ViewModel;
using LogService.Maui.Fonts;
using LogService.Maui.View;

namespace LogService.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
		var builder = MauiApp.CreateBuilder()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
				fonts.AddFont("MaterialIcons-Regular.ttf", MaterialIcons.FontFamily);
			})
			.UseSharedMauiApp();

		var app = builder.Build();

		app.Services.GetRequiredService<Najlot.Map.IMap>().RegisterFactory(t =>
		{
			if (t.GetConstructor(Type.EmptyTypes) is not null)
			{
				return Activator.CreateInstance(t) ?? throw new NullReferenceException("Could not create " + t.FullName);
			}

			return app.Services.GetRequiredService(t);
		});

		var localProfileHandler = app.Services.GetRequiredKeyedService<IProfileHandler>(nameof(Source.Local));
		var restProfileHandler = app.Services.GetRequiredKeyedService<IProfileHandler>(nameof(Source.REST));
		var rmqProfileHandler = app.Services.GetRequiredKeyedService<IProfileHandler>(nameof(Source.RMQ));
		localProfileHandler.SetNext(restProfileHandler).SetNext(rmqProfileHandler);

		var loginView = app.Services.GetRequiredService<LoginView>();
		loginView.BindingContext = app.Services.GetRequiredService<LoginViewModel>();

		return app;
	}
}
