using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AbdmWrapperNet.Common;
using AbdmWrapperNet.Data;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public class SqlServerLinkTokenService : ILinkTokenService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SqlServerLinkTokenService> _logger;

    public SqlServerLinkTokenService(AppDbContext context, ILogger<SqlServerLinkTokenService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string?> GetLinkTokenAsync(string abhaAddress, string entity)
    {
        var linkToken = await _context.LinkTokens
            .FirstOrDefaultAsync(lt => lt.AbhaAddress == abhaAddress && lt.HipId == entity);

        if (linkToken != null && !string.IsNullOrEmpty(linkToken.LinkTokenVal) && !Utils.CheckExpiry(linkToken.Expiry))
        {
            return linkToken.LinkTokenVal;
        }

        return null;
    }

    public async Task SaveLinkTokenAsync(string abhaAddress, string linkToken, string entity)
    {
        _logger.LogInformation($"Saving linkToken of {abhaAddress} {entity}");

        var existingToken = await _context.LinkTokens
            .FirstOrDefaultAsync(lt => lt.AbhaAddress == abhaAddress && lt.HipId == entity);

        if (existingToken != null)
        {
            existingToken.LinkTokenVal = linkToken;
            existingToken.Expiry = Utils.SetLinkTokenExpiry();
            _context.LinkTokens.Update(existingToken);
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
            await _context.LinkTokens.AddAsync(newToken);
        }

        await _context.SaveChangesAsync();
    }

    public async Task SaveLinkTokenRequestIdAsync(string abhaAddress, string entity, string linkTokenRequestId)
    {
        var existingToken = await _context.LinkTokens
            .FirstOrDefaultAsync(lt => lt.AbhaAddress == abhaAddress && lt.HipId == entity);

        if (existingToken != null)
        {
            existingToken.LinkTokenRequestId = linkTokenRequestId;
            _context.LinkTokens.Update(existingToken);
        }
        else
        {
            var newToken = new LinkToken
            {
                AbhaAddress = abhaAddress,
                HipId = entity,
                LinkTokenRequestId = linkTokenRequestId
            };
            await _context.LinkTokens.AddAsync(newToken);
        }

        await _context.SaveChangesAsync();
    }
}
