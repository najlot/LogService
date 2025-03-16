﻿using System.Globalization;

namespace LogService.Maui.Converter;

public class IdToItemConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value == null)
		{
			return null;
		}

		if (parameter is Picker picker)
		{
			if (picker.ItemsSource == null)
			{
				return null;
			}

			foreach (var item in picker.ItemsSource)
			{
				var property = item.GetType().GetProperty("Id");
				if (property?.GetValue(item)?.Equals(value) ?? false)
				{
					return item;
				}
			}
		}

		return null;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		var property = value?.GetType()?.GetProperty("Id");
		return property?.GetValue(value);
	}
}