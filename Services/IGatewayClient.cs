using System.Collections.Generic;
using System.Threading.Tasks;
using AbdmWrapperNet.Models;

namespace AbdmWrapperNet.Services;

public interface IGatewayClient
{
    Task<string> GetAccessTokenAsync();
    Task<GenericV3Response?> PostToGatewayAsync<T>(string path, T body, Dictionary<string, string>? customHeaders = null);
}
