using System.ComponentModel;
using Considition2023_Cs;

const string apikey = "24857943-542a-4e7d-bbe4-a7a1115b0527";

HttpClient client = new();
Api api = new(client);
var generalData = await api.GetGeneralDataAsync();

var _3100Capacity = generalData.Freestyle3100Data.RefillCapacityPerWeek;

foreach (var mapName in generalData.TrainingMapNames.Where(m => !m.ToLower().EndsWith("sandbox")))
{
    var mapData = await api.GetMapDataAsync(mapName, apikey);

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
    Console.WriteLine($"Map: {mapName}, Initial GameScore: {score.GameScore!.Total}");
    var scoreValue = score.GameScore!.Total;

    var optimizationFunctions = new List<Action<PlacedLocations>>
    {
        loc =>
        {
            loc.Freestyle9100Count = Math.Max(loc.Freestyle9100Count - 1, 0);
            loc.Freestyle3100Count = Math.Min(loc.Freestyle3100Count + 1, 2);
        },
        loc =>
        {
            loc.Freestyle3100Count = Math.Max(loc.Freestyle3100Count - 1, 0);
        },
        loc =>
        {
            loc.Freestyle3100Count = Math.Min(loc.Freestyle3100Count + 1, 2);
        },
        loc =>
        {
            loc.Freestyle3100Count = Math.Max(loc.Freestyle3100Count - 1, 0);
            loc.Freestyle9100Count = Math.Min(loc.Freestyle9100Count + 1, 2);
        },
        loc =>
        {
            loc.Freestyle9100Count = Math.Max(loc.Freestyle9100Count - 1, 0);
        },
        loc =>
        {
            loc.Freestyle9100Count = Math.Min(loc.Freestyle9100Count + 1, 2);
        },
    };

    var optimizeRunCount = 1;
    while (true)
    {   
        var previousScore = scoreValue;

        foreach (var optimizationFunction in optimizationFunctions)
        {
            Console.WriteLine($"** Optimize {optimizeRunCount++}");
            Optimize(locations, ref scoreValue, mapData, optimizationFunction);
        }

        if (Math.Abs(previousScore - scoreValue) < 0.0000001d) break;
    }

    File.WriteAllText($"{mapName}.json", System.Text.Json.JsonSerializer.Serialize(locations));

    solution.Locations = locations
        .Where(x => x.Value.Freestyle3100Count > 0 || x.Value.Freestyle9100Count > 0)
        .ToDictionary(x => x.Key, y => y.Value);

    score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
    Console.WriteLine($"Map: {mapName}, Optimized GameScore: {score.GameScore!.Total}");
    var prodScore = await api.SumbitAsync(mapName, solution, apikey);
    Console.WriteLine($"GameId: {prodScore.Id}");
    Console.WriteLine();
}
return;

void Optimize(
    Dictionary<string, PlacedLocations> locations, 
    ref double scoreValue, 
    MapData mapData, 
    Action<PlacedLocations> processLocation)
{
    Dictionary<string, PlacedLocations> bestScore = new(locations);
    foreach (var location in locations)
    {
        var old3100 = location.Value.Freestyle3100Count;
        var old9100 = location.Value.Freestyle9100Count;
        
        processLocation(location.Value);

        var solution = new SubmitSolution
        {
            Locations = locations
                .Where(x => x.Value.Freestyle3100Count > 0 || x.Value.Freestyle9100Count > 0)
                .ToDictionary(x => x.Key, y => y.Value)
        };

        var score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
        if (scoreValue >= score.GameScore!.Total)
        {
            location.Value.Freestyle3100Count = old3100;
            location.Value.Freestyle9100Count = old9100;
            continue;
        }

        scoreValue = score.GameScore.Total;
        Console.WriteLine($" - GameScore: {score.GameScore.Total}");
    }
}
