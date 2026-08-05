using System.Threading.Tasks;
using MongoDB.Driver;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class ConsentPatientV3Service : IConsentPatientV3Service
{
    private readonly MongoDbContext _context;

    public ConsentPatientV3Service(MongoDbContext context)
    {
        _context = context;
    }

    public async Task SaveConsentPatientMappingAsync(string consentId, string patientAbhaAddress, string entityType, string hipId)
    {
        var filter = Builders<ConsentPatient>.Filter.And(
            Builders<ConsentPatient>.Filter.Eq(cp => cp.ConsentId, consentId),
            Builders<ConsentPatient>.Filter.Eq(cp => cp.EntityType, entityType),
            Builders<ConsentPatient>.Filter.Eq(cp => cp.HipId, hipId)
        );

        var update = Builders<ConsentPatient>.Update
            .Set(cp => cp.AbhaAddress, patientAbhaAddress)
            .Set(cp => cp.EntityType, entityType)
            .Set(cp => cp.HipId, hipId);

        var options = new UpdateOptions { IsUpsert = true };

        await _context.ConsentPatients.UpdateOneAsync(filter, update, options);
    }

    public async Task<ConsentPatient?> FindMappingByConsentIdAsync(string consentId, string entityType, string hipId)
    {
        var filter = Builders<ConsentPatient>.Filter.And(
            Builders<ConsentPatient>.Filter.Eq(cp => cp.ConsentId, consentId),
            Builders<ConsentPatient>.Filter.Eq(cp => cp.EntityType, entityType),
            Builders<ConsentPatient>.Filter.Eq(cp => cp.HipId, hipId)
        );

        return await _context.ConsentPatients.Find(filter).FirstOrDefaultAsync();
    }
}
