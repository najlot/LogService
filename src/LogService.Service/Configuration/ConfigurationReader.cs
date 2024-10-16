﻿using Najlot.Log;
using System.IO;
using System.Text.Json;

namespace LogService.Service.Configuration
{
	public static class ConfigurationReader
	{
		private static readonly Logger _logger = LogAdministrator.Instance
				.GetLogger(typeof(ConfigurationReader));

		public static T ReadConfiguration<T>() where T : class, new()
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

			return JsonSerializer.Deserialize<T>(configContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
		}
	}
}