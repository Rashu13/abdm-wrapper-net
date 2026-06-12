using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class SqlServerConsentPatientV3Service : IConsentPatientV3Service
{
    private readonly AppDbContext _context;

    public SqlServerConsentPatientV3Service(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveConsentPatientMappingAsync(string consentId, string patientAbhaAddress, string entityType, string hipId)
    {
        var existing = await _context.ConsentPatients
            .FirstOrDefaultAsync(cp => cp.ConsentId == consentId && cp.EntityType == entityType && cp.HipId == hipId);

        if (existing != null)
        {
            existing.AbhaAddress = patientAbhaAddress;
            _context.ConsentPatients.Update(existing);
        }
        else
        {
            var mapping = new ConsentPatient
            {
                ConsentId = consentId,
                AbhaAddress = patientAbhaAddress,
                EntityType = entityType,
                HipId = hipId
            };
            await _context.ConsentPatients.AddAsync(mapping);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<ConsentPatient?> FindMappingByConsentIdAsync(string consentId, string entityType, string hipId)
    {
        return await _context.ConsentPatients
            .FirstOrDefaultAsync(cp => cp.ConsentId == consentId && cp.EntityType == entityType && cp.HipId == hipId);
    }
}
