using MongoDB.Driver;
using LogService.Service.Configuration;
using System;

namespace LogService.Service.Repository
{
	public class MongoDbContext : IDisposable
	{
		private readonly MongoDbConfiguration _configuration;

		public IMongoClient Client { get; }
        public IMongoDatabase Database { get; }

        public MongoDbContext(MongoDbConfiguration configuration)
        {
			_configuration = configuration;

			string connectionString;

			if (configuration.UseDnsSrv)
			{
				connectionString = $"mongodb+srv://{configuration.User}:{configuration.Password}" +
					$"@{configuration.Host}/{configuration.Database}";
			}
			else
			{
				connectionString = $"mongodb://{configuration.User}:{configuration.Password}" +
					$"@{configuration.Host}:{configuration.Port}/{configuration.Database}";
			}

			Client = new MongoClient(connectionString);
			Database = Client.GetDatabase(configuration.Database);
		}

		private bool _disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				_disposedValue = true;

				if (disposing)
				{
				}
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}