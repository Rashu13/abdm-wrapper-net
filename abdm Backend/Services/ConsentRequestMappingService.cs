using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

/// <summary>
/// Tracks the mapping between consent-request-id (from gateway on-init) and the original client gateway-request-id.
/// This is needed because on-status / hiu-notify callbacks only carry consentRequestId, not the original requestId.
/// </summary>
public interface IConsentRequestMappingService
{
    Task SaveAsync(string consentRequestId, string gatewayRequestId);
    Task<string?> GetGatewayRequestIdAsync(string consentRequestId);
}

public class SqlServerConsentRequestMappingService : IConsentRequestMappingService
{
    private readonly AppDbContext _context;

    public SqlServerConsentRequestMappingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(string consentRequestId, string gatewayRequestId)
    {
        var existing = await _context.ConsentRequestMappings
            .FirstOrDefaultAsync(m => m.ConsentRequestId == consentRequestId);

        if (existing != null)
        {
            existing.GatewayRequestId = gatewayRequestId;
            _context.ConsentRequestMappings.Update(existing);
        }
        else
        {
            await _context.ConsentRequestMappings.AddAsync(new ConsentRequestMapping
            {
                ConsentRequestId = consentRequestId,
                GatewayRequestId = gatewayRequestId
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<string?> GetGatewayRequestIdAsync(string consentRequestId)
    {
        var mapping = await _context.ConsentRequestMappings
            .FirstOrDefaultAsync(m => m.ConsentRequestId == consentRequestId);
        return mapping?.GatewayRequestId;
    }
}

public class MongoConsentRequestMappingService : IConsentRequestMappingService
{
    // Mongo stub – reuses RequestLog collection (persists as in-memory dictionary for now)
    private static readonly Dictionary<string, string> _map = new();

    public Task SaveAsync(string consentRequestId, string gatewayRequestId)
    {
        _map[consentRequestId] = gatewayRequestId;
        return Task.CompletedTask;
    }

    public Task<string?> GetGatewayRequestIdAsync(string consentRequestId)
    {
        _map.TryGetValue(consentRequestId, out var id);
        return Task.FromResult<string?>(id);
    }
}
