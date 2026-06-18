using System.Collections.Generic;
using System.Threading.Tasks;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public interface IGatewayClient
{
    Task<string> GetAccessTokenAsync();
    Task<GenericV3Response?> PostToGatewayAsync<T>(string path, T body, Dictionary<string, string>? customHeaders = null);
    Task<GenericV3Response?> PatchToGatewayAsync<T>(string path, T body, Dictionary<string, string>? customHeaders = null);
    Task<string> GetFromGatewayAsync(string path, Dictionary<string, string>? customHeaders = null);
}
