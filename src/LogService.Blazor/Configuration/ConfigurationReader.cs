using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Najlot.Log;

namespace LogService.Blazor.Configuration;

public static class ConfigurationReader
{
	private static readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };
	private static readonly Logger _logger = LogAdministrator.Instance
			.GetLogger(typeof(ConfigurationReader));

	public static T? ReadConfiguration<T>(this IConfiguration configuration) where T : class, new()
	{
		var key = typeof(T).Name;
		return ReadConfiguration<T>(configuration, key);
	}

	public static T? ReadConfiguration<T>(this IConfiguration configuration, string key) where T : class, new()
	{
		var section = configuration.GetSection(key);

		if (!section.Exists())
		{
			return ReadConfiguration<T>();
		}

		var t = new T();
		section.Bind(t);
		return t;
	}

	public static T? ReadConfiguration<T>() where T : class, new()
	{
		var configDir = "config";
		var configPath = Path.Combine(configDir, typeof(T).Name + ".json");
		configPath = Path.GetFullPath(configPath);

		if (!File.Exists(configPath))
		{
			_logger.Info(configPath + " not found.");

			if (!File.Exists(configPath + ".example"))
			{
				_logger.Info("Writing " + configPath + ".example...");

				if (!Directory.Exists(configDir))
				{
					Directory.CreateDirectory(configDir);
				}

				File.WriteAllText(configPath + ".example", JsonSerializer.Serialize(new T()));
			}

			return null;
		}

		var configContent = File.ReadAllText(configPath);

		return JsonSerializer.Deserialize<T>(configContent, _options);
	}
}