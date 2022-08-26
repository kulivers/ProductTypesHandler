using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;

namespace ProductCodeAnalyser;

public class HttpPlatformRequester
{
    public async Task<HttpResponseMessage> GetTypeName(string typeName)
    {
        return await Send(HttpMethod.Get, $"/api/reflection/type/{typeName}");
    }

    public async Task<HttpResponseMessage> GetControllers()
    {
        return await Send(HttpMethod.Get, "/api/reflection/actions");
    }
    
    private async Task<HttpResponseMessage> Send(HttpMethod method, string requestUri,
        string url = "http://localhost:8081")
    {
        try
        {
            var baseAddress = new Uri(url);
            using var handler = new HttpClientHandler { UseCookies = false };
            using var client = new HttpClient(handler) { BaseAddress = baseAddress };
            // var message = new HttpRequestMessage(HttpMethod.Get, "/api/AboutApi/Get");
            var message = new HttpRequestMessage(method, requestUri);
            var defaultHeaders = GetDefaultHeaders();
            foreach (var httpRequestHeader in defaultHeaders)
                message.Headers.Add(httpRequestHeader.Key, httpRequestHeader.Value);
            return await client.SendAsync(message);
        }
        catch
        {
            throw new Exception("Cant access to server");
        }
    }

    private static HttpRequestHeaders GetDefaultHeaders()
    {
        var mes = new HttpRequestMessage();
        mes.Headers.Clear();
        mes.Headers.Add("Connection", "keep-alive");
        mes.Headers.Add("X-Requested-With", "XMLHttpRequest");
        mes.Headers.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.93 Safari/537.36");
        mes.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        mes.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        mes.Headers.Add("Cookie",
            "Language=ru; ProtectorId=7a631b80-513c-4a01-802d-26cb27eadfc4; " +
            "RequestVerificationToken=kg6yGMSczUVms3sU_-qKmv8zuXMiQ2EY16KoC7Zm9-j4XDucjPDXd0G4Hth9QvKaXoMxbr7aR5zp125x3grpllN5qkYoee4k_2-dJpsQ0xc1; " +
            "SessionId=7a631b80-513c-4a01-802d-26cb27eadfc4; " +
            "TZ=180; " +
            "UserId=account.1");
        return mes.Headers;
    }
}