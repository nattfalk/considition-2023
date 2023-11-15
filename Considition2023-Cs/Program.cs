using System.ComponentModel;
using System.Security.Cryptography;
using Considition2023_Cs;
using Considition2023_Cs.Optimizers;

const bool isFinals = false;

const string apikey = "24857943-542a-4e7d-bbe4-a7a1115b0527";

// ReSharper disable once ConditionIsAlwaysTrueOrFalse
if (!isFinals)
{
    Console.WriteLine($"1: {MapNames.Goteborg}");
    Console.WriteLine($"2: {MapNames.Linkoping}");
    Console.WriteLine($"3: {MapNames.Uppsala}");
    Console.WriteLine($"4: {MapNames.Vasteras}");
    Console.WriteLine($"5: {MapNames.GSandbox}");
}
else
{
    Console.WriteLine($"6: {MapNames.Stockholm}");
    Console.WriteLine($"7: {MapNames.Malmo}");
    Console.WriteLine($"8: {MapNames.Orebro}");
    Console.WriteLine($"9: {MapNames.London}");
    Console.WriteLine($"10: {MapNames.Berlin}");
    Console.WriteLine($"11: {MapNames.SSandbox}");
}
Console.Write("Select the map you wish to play: ");
var option = Console.ReadLine();

var mapName = option switch
{
    "1" => MapNames.Goteborg,
    "2" => MapNames.Linkoping,
    "3" => MapNames.Uppsala,
    "4" => MapNames.Vasteras,
    "5" => MapNames.GSandbox,
    "6" => MapNames.Stockholm,
    "7" => MapNames.Malmo,
    "8" => MapNames.Orebro,
    "9" => MapNames.London,
    "10" => MapNames.Berlin,
    "11" => MapNames.SSandbox,
    _ => null
};

if (mapName is null)
{
    Console.WriteLine("Invalid map selected");
    return;
}

HttpClient client = new();
Api api = new(client);
var generalData = await api.GetGeneralDataAsync();

var _3100Capacity = generalData.Freestyle3100Data.RefillCapacityPerWeek;

var mapData = await api.GetMapDataAsync(mapName, apikey);

Dictionary<string, PlacedLocations> locations = new();
SubmitSolution solution = new()
{
    Locations = null
};

if (mapName == MapNames.GSandbox || mapName == MapNames.SSandbox)
{
    locations = CreateSandboxMap();
}

solution.Locations = InitializeLocations(mapData.locations, locations)
    .Where(x => x.Value.Freestyle3100Count > 0 || x.Value.Freestyle9100Count > 0)
    .ToDictionary(x => x.Key, y => y.Value);

var score = new Scoring().CalculateScore(mapName, solution, mapData, generalData);
Console.SetCursorPosition(0, 7);
Console.WriteLine($"Map: {mapName}, Initial GameScore: {score.GameScore!.Total}");
var scoreValue = score.GameScore!.Total;

var optimizeRunCount = 1;
var optimizers = new HashSet<IOptimizer>()
{
    new Optimizer2(generalData, mapData),       // Linköping 699.57
    //new Optimizer3_sorted_dec(generalData, mapData),    // Göteborg, 6147.40
    //new Optimizer3_sorted(generalData, mapData),    // Uppsala, 2410.65, Västerås, 1498.38
    
    //new Optimizer1(generalData, mapData),
    //new Optimizer3_2(generalData, mapData),
    //new Optimizer3(generalData, mapData),
    //new Optimizer4(generalData, mapData),
    //new Optimizer5(generalData, mapData),
    //new Optimizer6(generalData, mapData),
    //new Optimizer6_2(generalData, mapData)
    //new Optimizer6_sorted(generalData, mapData),
};

Dictionary<string, PlacedLocations> currentBestLocations = new();
while (true)
{
    var previousScore = scoreValue;
    foreach (var optimizer in optimizers)
    {
        var tempLocations = InitializeLocations(mapData.locations, locations);
        var newScore = optimizer.Optimize(tempLocations, scoreValue, ref optimizeRunCount);
        if (newScore > previousScore)
        {
            previousScore = newScore;
            currentBestLocations = CopyLocations(tempLocations);
        }

        Console.SetCursorPosition(0, 8);
        Console.WriteLine($"- Optimize step: {optimizeRunCount,3:0}, New score: {scoreValue,11:#.00}");
    }

    if (Math.Abs(previousScore - scoreValue) < 0.0000001d) break;
    locations = currentBestLocations;
    scoreValue = previousScore;
}

solution.Locations = locations
    .Where(x => x.Value.Freestyle3100Count > 0 || x.Value.Freestyle9100Count > 0)
    .ToDictionary(x => x.Key, y => y.Value);

score = new Scoring().CalculateScore(mapName, solution, mapData, generalData);
Console.SetCursorPosition(0, 9);
Console.WriteLine($"Optimized GameScore: {score.GameScore!.Total}");
Console.WriteLine();
var prodScore = await api.SumbitAsync(mapName, solution, apikey);
Console.WriteLine($"GameId: {prodScore.Id}");
Console.WriteLine($"Submitted GameScore: {prodScore.GameScore!.Total}");
Console.WriteLine();
return;

Dictionary<string, PlacedLocations> CopyLocations(Dictionary<string, PlacedLocations> inputLocations)
{
    var copiedLocations = new Dictionary<string, PlacedLocations>();
    foreach (var kvp in inputLocations)
    {
        copiedLocations[kvp.Key] = new PlacedLocations
        {
            Freestyle3100Count = kvp.Value.Freestyle3100Count,
            Freestyle9100Count = kvp.Value.Freestyle9100Count,
            LocationType = kvp.Value.LocationType,
            Longitude = kvp.Value.Longitude,
            Latitude = kvp.Value.Latitude
        };
    }
    return copiedLocations;
}

Dictionary<string, PlacedLocations> InitializeLocations(Dictionary<string, StoreLocation> inputLocations, IReadOnlyDictionary<string, PlacedLocations> calculatedLocations)
{
    var initializedLocations = new Dictionary<string, PlacedLocations>();
    if (mapData.locations.Count > 0)
    {
        foreach (var locationKeyPair in mapData.locations)
        {
            var location = locationKeyPair.Value;
            var calculatedLocation = calculatedLocations.TryGetValue(locationKeyPair.Key, out var location1) ? location1 : null;
            initializedLocations[location.LocationName] = new PlacedLocations
            {
                Freestyle3100Count = calculatedLocation?.Freestyle3100Count ?? (location.SalesVolume <= _3100Capacity ? 1 : 0),
                Freestyle9100Count = calculatedLocation?.Freestyle9100Count ?? (location.SalesVolume > _3100Capacity ? 1 : 0)
            };
        }
    }
    else
    {
        foreach (var location in locations)
        {
            initializedLocations[location.Key] = new PlacedLocations
            {
                Freestyle3100Count = location.Value.Freestyle3100Count,
                Freestyle9100Count = location.Value.Freestyle9100Count,
                Latitude = location.Value.Latitude,
                Longitude = location.Value.Longitude,
                LocationType = location.Value.LocationType
            };
        }
    }
    return initializedLocations;
}

Dictionary<string, PlacedLocations> CreateSandboxMap()
{
    var locations = new Dictionary<string, PlacedLocations>();

    //const int maxGroceryStoreLarge = 5;
    //const int maxGroceryStore = 20;
    //const int maxConvenience = 20;
    //const int maxGasStation = 8;
    //const int maxKiosk = 3;

    var hotspots = mapData.Hotspots.OrderByDescending(h => h.Footfall * h.Spread).ToList();

    var locCount = 1;
    for (var i = 0; i < 5; i++)
    {
        locations.Add($"location{locCount++}", new PlacedLocations
        {
            Longitude = hotspots[i].Longitude,
            Latitude = hotspots[i].Latitude,
            LocationType = "Grocery-store-large",
            Freestyle9100Count = i % 3 == 0 ? 2 : 1,
            Freestyle3100Count = i % 3 == 0 ? 1 : 0
        });
    }
    for (var i = 0; i < 20; i++)
    {
        locations.Add($"location{locCount++}", new PlacedLocations
        {
            Longitude = hotspots[i].Longitude,
            Latitude = hotspots[i].Latitude,
            LocationType = "Grocery-store",
            Freestyle9100Count = i % 20 == 0 ? 2 : 1,
            Freestyle3100Count = i % 20 == 0 ? 1 : 0
            //Freestyle9100Count = i % 2 == 0 ? 0 : 1,
            //Freestyle3100Count = i % 2 == 0 ? 1 : 0
        });
    }
    for (var i = 0; i < 20; i++)
    {
        locations.Add($"location{locCount++}", new PlacedLocations
        {
            Longitude = hotspots[i].Longitude,
            Latitude = hotspots[i].Latitude,
            LocationType = "Convenience",
            Freestyle3100Count = 0,
            Freestyle9100Count = 0
            //Freestyle9100Count = i % 4 == 0 ? 2 : 1,
            //Freestyle9100Count = 0,
            //Freestyle3100Count = i % 4 == 0 ? 1 : 0
            //Freestyle3100Count = 1
        });
    }
    for (var i = 0; i < 8; i++)
    {
        locations.Add($"location{locCount++}", new PlacedLocations
        {
            Longitude = hotspots[i].Longitude,
            Latitude = hotspots[i].Latitude,
            LocationType = "Gas-station",
            Freestyle3100Count = 0,
            Freestyle9100Count = 0
            //Freestyle9100Count = i % 4 == 0 ? 2 : 1,
            //Freestyle3100Count = 0, //i % 4 == 0 ? 1 : 0
        });
    }
    for (var i = 0; i < 3; i++)
    {
        locations.Add($"location{locCount++}", new PlacedLocations
        {
            Longitude = hotspots[i].Longitude,
            Latitude = hotspots[i].Latitude,
            LocationType = "Kiosk",
            Freestyle3100Count = 0,
            Freestyle9100Count = 0
            //Freestyle9100Count = 0, //i % 4 == 0 ? 2 : 1,
            //Freestyle3100Count = i % 2 == 0 ? 1 : 0
        });
    }

    return locations;
}