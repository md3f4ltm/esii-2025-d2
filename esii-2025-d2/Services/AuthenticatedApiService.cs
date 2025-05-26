using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using esii_2025_d2.Models;

namespace esii_2025_d2.Services
{
    public interface IAuthenticatedApiService
    {
        Task<List<T>?> GetAsync<T>(string endpoint);
        Task<T?> PostAsync<T>(string endpoint, object data);
        Task<bool> DeleteAsync(string endpoint);
    }

    public class AuthenticatedApiService : IAuthenticatedApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticatedApiService(
            HttpClient httpClient,
            AuthenticationStateProvider authenticationStateProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<HttpClient> GetAuthenticatedHttpClientAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                // Copy authentication cookies from the current HTTP context
                var cookieHeader = httpContext.Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookieHeader))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Cookie");
                    _httpClient.DefaultRequestHeaders.Add("Cookie", cookieHeader);
                }
            }
            return _httpClient;
        }

        public async Task<List<T>?> GetAsync<T>(string endpoint)
        {
            try
            {
                var client = await GetAuthenticatedHttpClientAsync();
                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                return new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var client = await GetAuthenticatedHttpClientAsync();
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                return default(T);
            }
            catch
            {
                return default(T);
            }
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var client = await GetAuthenticatedHttpClientAsync();
                var response = await client.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
