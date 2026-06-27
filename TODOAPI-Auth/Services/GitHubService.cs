using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TODOAPI_Auth.DTOs;
using TODOAPI_Auth.Models;

namespace TODOAPI_Auth.Services
{
    public class GitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GitHubService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            var token = _configuration["ExternalApis:GitHub:Token"];

            // Configure authentication context headers
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            // GitHub strict API requirements [INDEX]
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
            _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TODOAPI-Auth-App"); 
        }

        public async Task<List<GitHubIssueDto>?> GetOpenIssuesAsync()
        {
            try
            {
                var owner = _configuration["ExternalApis:GitHub:Owner"];
                var repo = _configuration["ExternalApis:GitHub:Repo"];
                var url = $"https://api.github.com/repos/{owner}/{repo}/issues?state=open";

                Console.WriteLine($"Calling GitHub: {url}");

                var response = await _httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GitHub Error: {responseBody}");
                    return null;
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true 
                };

                var issues = JsonSerializer.Deserialize<List<GitHubIssue>>(
                    responseBody, options);

                Console.WriteLine($"Issues found: {issues?.Count ?? 0}");

                if (issues == null) return new List<GitHubIssueDto>();

                return issues.Select(i => new GitHubIssueDto
                {
                    Id = i.Id,          
                    Number = i.Number,
                    Title = i.Title,
                    Body = i.Body ?? "No description provided.",
                    CreatedAt = i.CreatedAt,
                    Url = i.HtmlUrl,
                    Labels = i.Labels.Select(l => l.Name).ToList()
                }).ToList();
            }
            catch (JsonException ex)         
            {
                Console.WriteLine($"JSON Error: {ex.Message}");
                Console.WriteLine($"Path: {ex.Path}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
