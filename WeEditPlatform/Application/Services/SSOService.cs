using Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace Application.Services
{
    public class SSOService : ISSOService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SSOService(HttpClient httpClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            _httpClient.BaseAddress = new Uri(_configuration.GetValue<string>("AppSettings:ssoUrl"));
        }

        public async Task UpdateRoles(AddRolesVM addRolesVM)
        {
            try
            {
                var data = new StringContent(JsonConvert.SerializeObject(addRolesVM), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}api/account/roles", data);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Add Roles error");
                }

                var result = await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException taskCanceledException)
            {
                throw taskCanceledException;
            }
        }
    }
}
