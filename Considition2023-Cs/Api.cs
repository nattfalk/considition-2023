using System.Net;
using Newtonsoft.Json;
using System.Text.Json;

namespace Considition2023_Cs;

internal class Api
{
    private readonly HttpClient _httpClient;

    public Api(HttpClient httpClient)
    {
        _httpClient = httpClient;
        httpClient.BaseAddress = new Uri("https://api.considition.com/");
    }

    public async Task<MapData> GetMapDataAsync(string mapName, string apiKey)
    {
        HttpRequestMessage request = new();
        request.Method = HttpMethod.Get;
        request.RequestUri = new Uri($"/api/game/getmapdata?mapName={Uri.EscapeDataString(mapName)}", UriKind.Relative);
        request.Headers.Add("x-api-key", apiKey);
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseText = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<MapData>(responseText);
    }

    public async Task<GeneralData> GetGeneralDataAsync()
    {
        var response = await _httpClient.GetAsync("/api/game/getgeneralgamedata");
        response.EnsureSuccessStatusCode();
        string responseText = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<GeneralData>(responseText);
    }

    public async Task<GameData> GetGameAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/api/game/getgamedata{id}");
        response.EnsureSuccessStatusCode();
        string responseText = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<GameData>(responseText);
    }

    public async Task<GameData> SumbitAsync(string mapName, SubmitSolution solution, string apiKey)
    {
        HttpResponseMessage response;
        while(true)
        {
            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"/api/Game/submitSolution?mapName={Uri.EscapeDataString(mapName)}", UriKind.Relative),
                Content = new StringContent(JsonConvert.SerializeObject(solution), System.Text.Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-api-key", apiKey);
            response = _httpClient.Send(request);
            if (response.StatusCode != HttpStatusCode.TooManyRequests)
                break;
            Thread.Sleep(1000);
        }
        string responseText = await response.Content.ReadAsStringAsync();
        try
        {
            return JsonConvert.DeserializeObject<GameData>(responseText);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}   
