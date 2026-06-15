using Microsoft.Extensions.Options;
using MongoDB.Driver;
using AbdmWrapperNet.Configuration;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<AbdmConfig> config)
    {
        var client = new MongoClient(config.Value.MongoDb.ConnectionString);
        _database = client.GetDatabase(config.Value.MongoDb.DatabaseName);
    }

    public IMongoCollection<Patient> Patients => _database.GetCollection<Patient>("patients");
    public IMongoCollection<RequestLog> RequestLogs => _database.GetCollection<RequestLog>("request-logs");
    public IMongoCollection<LinkToken> LinkTokens => _database.GetCollection<LinkToken>("link-tokens");
    public IMongoCollection<ConsentPatient> ConsentPatients => _database.GetCollection<ConsentPatient>("consent-patient");
    public IMongoCollection<AbhaConsentLog> AbhaConsentLogs => _database.GetCollection<AbhaConsentLog>("abha-consent-logs");
}
