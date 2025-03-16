using LogService.Client.MVVM.Validation;
using LogService.Client.MVVM.ViewModel;
using System.Runtime.CompilerServices;

namespace LogService.Maui.Controls;

public class ValidationLabel : Label
{
	public static BindableProperty PropertyNameProperty =
		BindableProperty.Create(nameof(PropertyName), typeof(string), typeof(ValidationLabel));

	public static BindableProperty HasErrorsProperty =
		BindableProperty.Create(nameof(HasErrors), typeof(bool), typeof(ValidationLabel));

	public string PropertyName
	{
		get => (string)GetValue(PropertyNameProperty);
		set => SetValue(PropertyNameProperty, value);
	}

	public bool HasErrors
	{
		get => (bool)GetValue(HasErrorsProperty);
		set => SetValue(HasErrorsProperty, value);
	}

	public ValidationLabel()
	{
		IsVisible = false;
		Margin = new Thickness
		{
			Bottom = 15
		};
	}

	protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		base.OnPropertyChanged(propertyName);

		if (propertyName == nameof(HasErrors))
		{
			if (BindingContext is AbstractValidationViewModel model)
			{
				if (model.HasErrors)
				{
					var results = model.Errors.Where(err => err.PropertyName == PropertyName);

					var errors = results
						.Where(e => e.Severity == ValidationSeverity.Error)
						.Select(e => e.Text)
						.ToList();

					if (errors.Count != 0)
					{
						var errorText = string.Join(Environment.NewLine, errors);
						Text = errorText;
						TextColor = Colors.Red;
						IsVisible = true;
						return;
					}

					var warnings = results
						.Where(e => e.Severity == ValidationSeverity.Warning)
						.Select(w => w.Text)
						.ToList();

					if (warnings.Count != 0)
					{
						var warningText = string.Join(Environment.NewLine, warnings);
						Text = warningText;
						TextColor = Colors.Orange;
						IsVisible = true;
						return;
					}

					var infos = results
						.Where(e => e.Severity == ValidationSeverity.Info)
						.Select(i => i.Text)
						.ToList();

					if (infos.Count != 0)
					{
						var infoText = string.Join(Environment.NewLine, infos);
						Text = infoText;
						TextColor = Colors.DarkCyan;
						IsVisible = true;
						return;
					}
				}

				IsVisible = false;
			}
		}
	}
}