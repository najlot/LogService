using System.Collections;
using System.Globalization;

namespace LogService.Maui.Converter;

public class EnumToTranslationConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}

		if (parameter is not Type type)
		{
			return null;
		}

		var resourceManager = new System.Resources.ResourceManager(type);

		if (value.GetType().IsConstructedGenericType)
		{
			if (value is not ICollection collection)
			{
				return null;
			}

			var newCollection = new List<object>();

			foreach (var item in collection)
			{
				var translation = Translate((Enum)item, resourceManager) ?? item?.ToString() ?? "";
				newCollection.Add(translation);
			}

			return newCollection;
		}

		return Translate((Enum)value, resourceManager);
	}

	private string? Translate(Enum value, System.Resources.ResourceManager resourceManager)
	{
		return resourceManager.GetString(value.ToString());
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}

		if (parameter is not Type type)
		{
			return null;
		}

		var resourceManager = new System.Resources.ResourceManager(type);

		if (targetType.IsConstructedGenericType)
		{
			if (value is not ICollection collection)
			{
				return null;
			}

			var newCollection = new List<object?>();

			foreach (var item in collection)
			{
				newCollection.Add(TranslateBack(item.ToString(), resourceManager, culture, targetType));
			}

			return newCollection;
		}

		return TranslateBack(value.ToString(), resourceManager, culture, targetType);
	}

	private object? TranslateBack(string? value, System.Resources.ResourceManager resourceManager, CultureInfo culture, Type targetType)
	{
		var resourceSet = resourceManager.GetResourceSet(culture, true, true);

		if (resourceSet == null)
		{
			return value;
		}

		foreach (DictionaryEntry entry in resourceSet)
		{
			if (entry.Value?.ToString() == value)
			{
				var key = entry.Key.ToString();

				if (key == null)
				{
					return null;
				}

				return Enum.Parse(targetType, key);
			}
		}

		return value;
	}
}