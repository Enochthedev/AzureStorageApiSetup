using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class MasqueryClient : IMasqueryClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MasqueryClient> _logger;
    private readonly string _masqueryBaseUrl;

    public MasqueryClient(HttpClient httpClient, ILogger<MasqueryClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _masqueryBaseUrl = "https://masquery.example.com/api"; // Replace with actual Masquery API base URL
    }

    public async Task<bool> CreateUserFileRecordAsync(Guid userId, string company, string category, string fileUrl, string relativePath)
    {
        var requestBody = new
        {
            UserId = userId,
            Company = company,
            Category = category,
            FileUrl = fileUrl,
            RelativePath = relativePath,
            CreatedAt = DateTime.UtcNow
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            _logger.LogInformation("Sending request to Masquery to create user file record. UserId: {UserId}", userId);
            var response = await _httpClient.PostAsync($"{_masqueryBaseUrl}/files", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully created file record in Masquery for UserId: {UserId}", userId);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to create file record in Masquery. StatusCode: {StatusCode}", response.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user file record in Masquery.");
            return false;
        }
    }
}