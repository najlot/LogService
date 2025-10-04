using MongoDB.Driver;
using LogService.Blazor.Configuration;

namespace LogService.Blazor.Repository;

public class MongoDbContext
{
	public IMongoClient Client { get; }
	public IMongoDatabase Database { get; }

	public MongoDbContext(MongoDbConfiguration configuration)
	{
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
}