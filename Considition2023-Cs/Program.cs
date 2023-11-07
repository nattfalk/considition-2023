using System.Runtime.InteropServices;
using Considition2023_Cs;
using System.Text.Json.Serialization;
using static System.Formats.Asn1.AsnWriter;

const string apikey = "24857943-542a-4e7d-bbe4-a7a1115b0527";

HttpClient client = new();
Api api = new(client);
var generalData = await api.GetGeneralDataAsync();

var _3100Capacity = generalData.Freestyle3100Data.RefillCapacityPerWeek;
var _9100Capacity = generalData.Freestyle9100Data.RefillCapacityPerWeek;

foreach (var mapName in generalData.TrainingMapNames)
{
    var mapData = await api.GetMapDataAsync(mapName, apikey);

    double scoreValue = 0d;

    SubmitSolution solution = new()
    {
        Locations = null
    };
    var locations = new Dictionary<string, PlacedLocations>();

    foreach (var locationKeyPair in mapData.locations)
    {
        var location = locationKeyPair.Value;
        locations[location.LocationName] = new PlacedLocations
        {
            Freestyle3100Count = location.SalesVolume <= _3100Capacity ? 1 : 0,
            Freestyle9100Count = location.SalesVolume > _3100Capacity ? 1 : 0
        };
    }

    solution.Locations = locations;
    var score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
    Console.WriteLine($"Map: {mapName}, GameScore: {score.GameScore!.Total}");
    scoreValue = score.GameScore!.Total;

    Console.WriteLine("** RUN 1");
    var bestScore = Algo_No9100_Inc3100(locations, ref scoreValue, mapData);
    Console.WriteLine("** RUN 2");
    bestScore = Algo_Dec3100(bestScore, ref scoreValue, mapData);

    File.WriteAllText($"{mapName}.json", System.Text.Json.JsonSerializer.Serialize(bestScore));
    
    solution.Locations = bestScore;
    score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
    Console.WriteLine($"Map: {mapName}, GameScore: {score.GameScore!.Total}");
    var prodScore = await api.SumbitAsync(mapName, solution, apikey);
    Console.WriteLine($"GameId: {prodScore.Id}");
}

Dictionary<string, PlacedLocations> Algo_No9100_Inc3100(Dictionary<string, PlacedLocations> locations, ref double scoreValue, MapData mapData)
{
    Dictionary<string, PlacedLocations> bestScore = new(locations);
    foreach (var location in locations)
    {
        var old3100 = location.Value.Freestyle3100Count;
        var old9100 = location.Value.Freestyle9100Count;
        location.Value.Freestyle9100Count = 0;
        location.Value.Freestyle3100Count += 1;

        var solution = new SubmitSolution
        {
            Locations = locations
                .Where(x => x.Value.Freestyle3100Count > 0 || x.Value.Freestyle9100Count > 0)
                .ToDictionary(x => x.Key, y => y.Value)
        };

        var score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
        if (scoreValue < score.GameScore!.Total)
        {
            scoreValue = score.GameScore.Total;
            Console.WriteLine($" - Map: {mapData.MapName}, GameScore: {score.GameScore.Total}");

            old9100 = location.Value.Freestyle9100Count;
            old3100 = location.Value.Freestyle3100Count;
            bestScore = new Dictionary<string, PlacedLocations>(solution.Locations);
        }

        location.Value.Freestyle3100Count = old3100;
        location.Value.Freestyle9100Count = old9100;
    }

    return bestScore;
}

Dictionary<string, PlacedLocations> Algo_Dec3100(Dictionary<string, PlacedLocations> locations, ref double scoreValue, MapData mapData)
{
    Dictionary<string, PlacedLocations> bestScore = new(locations);
    foreach (var location in locations)
    {
        var old3100 = location.Value.Freestyle3100Count;
        location.Value.Freestyle3100Count = 0;

        var solution = new SubmitSolution
        {
            Locations = locations
                .Where(x => x.Value.Freestyle3100Count > 0 || x.Value.Freestyle9100Count > 0)
                .ToDictionary(x => x.Key, y => y.Value)
        };

        var score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
        if (scoreValue < score.GameScore!.Total)
        {
            scoreValue = score.GameScore.Total;
            Console.WriteLine($" - Map: {mapData.MapName}, GameScore: {score.GameScore.Total}");

            old3100 = location.Value.Freestyle3100Count;
            bestScore = new Dictionary<string, PlacedLocations>(solution.Locations);
        }

        location.Value.Freestyle3100Count = old3100;
    }

    return bestScore;
}