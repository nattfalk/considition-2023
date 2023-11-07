using System.Runtime.InteropServices;
using Considition2023_Cs;
using System.Text.Json.Serialization;

const string apikey = "24857943-542a-4e7d-bbe4-a7a1115b0527";


//if (string.IsNullOrWhiteSpace(apikey))
//{
//    Console.WriteLine("Configure apiKey");
//    return;
//}

//Console.WriteLine($"1: {MapNames.Stockholm}");
//Console.WriteLine($"2: {MapNames.Goteborg}");
//Console.WriteLine($"3: {MapNames.Malmo}");
//Console.WriteLine($"4: {MapNames.Uppsala}");
//Console.WriteLine($"5: {MapNames.Vasteras}");
//Console.WriteLine($"6: {MapNames.Orebro}");
//Console.WriteLine($"7: {MapNames.London}");
//Console.WriteLine($"8: {MapNames.Linkoping}");
//Console.WriteLine($"9: {MapNames.Berlin}");

//Console.Write("Select the map you wish to play: ");
//string option = Console.ReadLine();

//var mapName = option switch
//{
//    "1" => MapNames.Stockholm,
//    "2" => MapNames.Goteborg,
//    "3" => MapNames.Malmo,
//    "4" => MapNames.Uppsala,
//    "5" => MapNames.Vasteras,
//    "6" => MapNames.Orebro,
//    "7" => MapNames.London,
//    "8" => MapNames.Linkoping,
//    "9" => MapNames.Berlin,
//    _ => null
//};

//if (mapName is null)
//{
//    Console.WriteLine("Invalid map selected");
//    return;
//}

//var mapName = MapNames.Goteborg;

HttpClient client = new();
Api api = new(client);
GeneralData generalData = await api.GetGeneralDataAsync();
foreach (var mapName in generalData.TrainingMapNames)
{
    MapData mapData = await api.GetMapDataAsync(mapName, apikey);

    double scoreValue = 0d;
//while (true)
//{
    SubmitSolution solution = new()
    {
        Locations = new()
    };
    //int i = 0;
    foreach (KeyValuePair<string, StoreLocation> locationKeyPair in mapData.locations)
    {
        StoreLocation location = locationKeyPair.Value;


        var salesVolume = location.SalesVolume;

        //var _9100 = 0;
        //generalData.Freestyle3100Data.RefillCapacityPerWeek
        //var volumeLeft = salesVolume;

        //var need9100 = salesVolume / generalData.Freestyle9100Data.RefillCapacityPerWeek;
        //if (need9100 > 0.85d)
        //{
        //    _9100 = (int)Math.Round(need9100, MidpointRounding.AwayFromZero);
        //    volumeLeft = Math.Max(0, volumeLeft - (_9100 * generalData.Freestyle9100Data.RefillCapacityPerWeek));
        //}

        //var need3100 = volumeLeft / generalData.Freestyle3100Data.RefillCapacityPerWeek;
        ////var _3100 = (int) need3100;
        //var _3100 = (int)Math.Round(need3100, MidpointRounding.AwayFromZero);
        //if (_3100 == 0 && need3100 > 0.3d) _3100 = 1;
        //if (salesVolume == 0) continue;

        
        //var _3100 = salesVolume <= 15 ? 1 : 0;
        //if (salesVolume >= 24) _3100++;
        //if (salesVolume >= 21) _3100++;
        //if (salesVolume >= 373) _3100++;
        var _3100 = salesVolume switch
        {
            3 => 1,
            15 => 1,
            24 => 1,
            121 => 0,
            373 => 0,
            _ => 1
        };

        var _9100 = salesVolume switch
        {
            3 => 0,
            15 => 0,
            24 => 0,
            121 => 1,
            373 => 1,
            _ => 0
        };

        //var _9100 = salesVolume > 24 ? 1 : 0;
        //var _9100 = 0;
        if (_3100 + _9100 == 0) continue;

        //_3100 *= (location.Footfall > 0.8d ? 2 : 1);
        //_9100 *= (location.Footfall > 0.9d ? 2 : 1);
        //if (_9100 > 0 && location.Footfall > 0.9d)
        //    _9100++;

        solution.Locations[location.LocationName] = new PlacedLocations
        {
            Freestyle3100Count = _3100,
            Freestyle9100Count = _9100
        };


        ////string name = locationKeyPair.Key;
        ////if (location.Footfall == 0d) continue;
        //var salesVolume = location.SalesVolume;
        //if (salesVolume > 0)
        //{
        //    //var _9100 = Random.Shared.Next(0, 2) * Random.Shared.Next(0, 6);
        //    //var _3100 = Random.Shared.Next(0, 2) * Random.Shared.Next(0, 6);

        //    //if (_3100 + _9100 == 0) continue;
        //    //var _3100 = i++ % 5 == 0 ? 1 : 0;
        //    //var _9100 = salesVolume > 0 ? 1 : 0;
        //    var _3100 = salesVolume < 100  ?  1 : 0;
        //    var _9100 = salesVolume > 24 ? 1 : 0;
        //    //if (location.Footfall > 0.8d) _9100++;

        //    if (_3100 + _9100 == 0) continue;

        //    solution.Locations[location.LocationName] = new PlacedLocations()
        //    {
        //        Freestyle3100Count = _3100, //_3100,
        //        Freestyle9100Count = _9100, //i % 3 == 0 ? 2 : 1, //_9100,
        //        Footfall = location.Footfall,
        //        SalesVolume = location.SalesVolume
        //    };
        //}
    }

    //var json = System.Text.Json.JsonSerializer.Serialize(solution);
    //File.WriteAllText($@"c:\temp\considition\{mapName}.json", json);

    GameData score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
    if (scoreValue < score.GameScore.Total)
    {
        scoreValue = score.GameScore.Total;
        Console.WriteLine($"Map: {mapName}, GameScore: {score.GameScore.Total}");
    }

    solution.Locations = solution.Locations
        .Where(x =>
            x.Key != "location73"
            && x.Key != "location139"
            && x.Key ! != "location160")
        .ToDictionary(x => x.Key, y => y.Value);
    score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
    Console.WriteLine($"Recalculated score - Map: {mapName}, GameScore: {score.GameScore.Total}");


    //foreach (var locationScore in score.Locations)
    //{
    //    if (locationScore.Value.Earnings <= 0d && locationScore.Value.Freestyle3100Count > 0)
    //    {
    //        Console.WriteLine($"{locationScore.Key}, {locationScore.Value.Earnings}");
    //        solution.Locations = solution.Locations
    //            .Where(x => x.Key != locationScore.Key)
    //            .ToDictionary(x => x.Key, y => y.Value);
    //        break;
    //    }
    //}
    //score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
    //Console.WriteLine($"Recalculated score - Map: {mapName}, GameScore: {score.GameScore.Total}");

    //foreach (var location in solution.Locations)
    //{
    //    var oldList = solution.Locations;
    //    var oldValue = location.Value.Freestyle3100Count;
    //    location.Value.Freestyle3100Count = 0;

    //    if (location.Value.Freestyle3100Count + location.Value.Freestyle9100Count == 0)
    //    {
    //        solution.Locations = solution.Locations.Where(x => x.Key != location.Key).ToDictionary(x => x.Key, y => y.Value);
    //    }

    //    score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
    //    if (scoreValue < score.GameScore.Total)
    //    {
    //        scoreValue = score.GameScore.Total;
    //        Console.WriteLine($"Map: {mapName}, Location: {location.Key}-{location.Value.Freestyle3100Count}-{location.Value.Freestyle9100Count}, GameScore: {score.GameScore.Total}");
    //        //GameData prodScore = await api.SumbitAsync(mapName, solution, apikey);
    //        //Console.WriteLine($"GameId: {prodScore.Id}");
    //    }

    //    solution.Locations = oldList;
    //    location.Value.Freestyle3100Count = oldValue;

    //}

    //}
    //GameData prodScore = await api.SumbitAsync(mapName, solution, apikey);
    //Console.WriteLine($"GameId: {prodScore.Id}");
    //break;
}

