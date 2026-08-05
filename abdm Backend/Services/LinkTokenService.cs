using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class LinkTokenService : ILinkTokenService
{
    private readonly MongoDbContext _context;
    private readonly ILogger<LinkTokenService> _logger;

    public LinkTokenService(MongoDbContext context, ILogger<LinkTokenService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string?> GetLinkTokenAsync(string abhaAddress, string entity)
    {
        var filter = Builders<LinkToken>.Filter.And(
            Builders<LinkToken>.Filter.Eq(lt => lt.AbhaAddress, abhaAddress),
            Builders<LinkToken>.Filter.Eq(lt => lt.HipId, entity)
        );

        var linkToken = await _context.LinkTokens.Find(filter).FirstOrDefaultAsync();
        if (linkToken != null && !string.IsNullOrEmpty(linkToken.LinkTokenVal) && !Utils.CheckExpiry(linkToken.Expiry))
        {
            return linkToken.LinkTokenVal;
        }

        return null;
    }

    public async Task SaveLinkTokenAsync(string abhaAddress, string linkToken, string entity)
    {
        _logger.LogInformation($"Saving linkToken of {abhaAddress} {entity}");

        var filter = Builders<LinkToken>.Filter.And(
            Builders<LinkToken>.Filter.Eq(lt => lt.AbhaAddress, abhaAddress),
            Builders<LinkToken>.Filter.Eq(lt => lt.HipId, entity)
        );

        var existingToken = await _context.LinkTokens.Find(filter).FirstOrDefaultAsync();
        if (existingToken != null)
        {
            var update = Builders<LinkToken>.Update
                .Set(lt => lt.LinkTokenVal, linkToken)
                .Set(lt => lt.Expiry, Utils.SetLinkTokenExpiry());

            await _context.LinkTokens.UpdateOneAsync(filter, update);
        }
        else
        {
            var newToken = new LinkToken
            {
                AbhaAddress = abhaAddress,
                LinkTokenVal = linkToken,
                Expiry = Utils.SetLinkTokenExpiry(),
                HipId = entity
            };

            await _context.LinkTokens.InsertOneAsync(newToken);
        }
    }

    public async Task SaveLinkTokenRequestIdAsync(string abhaAddress, string entity, string linkTokenRequestId)
    {
        var filter = Builders<LinkToken>.Filter.And(
            Builders<LinkToken>.Filter.Eq(lt => lt.AbhaAddress, abhaAddress),
            Builders<LinkToken>.Filter.Eq(lt => lt.HipId, entity)
        );

        var existingToken = await _context.LinkTokens.Find(filter).FirstOrDefaultAsync();
        if (existingToken != null)
        {
            var update = Builders<LinkToken>.Update.Set(lt => lt.LinkTokenRequestId, linkTokenRequestId);
            await _context.LinkTokens.UpdateOneAsync(filter, update);
        }
        else
        {
            var newToken = new LinkToken
            {
                AbhaAddress = abhaAddress,
                HipId = entity,
                LinkTokenRequestId = linkTokenRequestId
            };

            await _context.LinkTokens.InsertOneAsync(newToken);
        }
    }
}
