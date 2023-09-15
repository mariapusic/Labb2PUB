using Labb2PUB.Server.Data.Models;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

namespace Labb2PUB.Server.Data;

public class CountryContext
{
    private CosmosClient _cosmosClient;
    private Container _container;

    public CountryContext(string connectionstring, string containerName, string databaseName)
    {
        _cosmosClient = new CosmosClient(connectionstring);
        _container = _cosmosClient.GetContainer(databaseName, containerName);
    }

    public async Task<List<CountryModel>> GetAllCountries()
    {
        FeedIterator<CountryModel>? query =
            _container.GetItemQueryIterator<CountryModel>(new QueryDefinition("select * from c"));
        List<CountryModel> Countries = new();
        while (query.HasMoreResults)
        {
            FeedResponse<CountryModel> res = await query.ReadNextAsync();
            Countries.AddRange(res.ToList());
        }

        return Countries;
    }

    public async Task<CountryModel> GetCountry(string Id)
    {
        ItemResponse<CountryModel> res =
            await _container.ReadItemAsync<CountryModel>(id: Id, partitionKey: PartitionKey.None);
        return res.Resource;
    }

    public async Task AddCountry(CountryModel cModel)
    {
        await _container.CreateItemAsync(cModel, partitionKey: PartitionKey.None);
    }
}