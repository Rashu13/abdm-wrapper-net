using System.Threading.Tasks;

namespace AbdmWrapperNet.Services;

public interface ILinkTokenService
{
    Task<string?> GetLinkTokenAsync(string abhaAddress, string entity);
    Task SaveLinkTokenAsync(string abhaAddress, string linkToken, string entity);
    Task SaveLinkTokenRequestIdAsync(string abhaAddress, string entity, string linkTokenRequestId);
}
