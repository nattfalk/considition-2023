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
    int i = 0;
    foreach (KeyValuePair<string, StoreLocation> locationKeyPair in mapData.locations)
    {
        StoreLocation location = locationKeyPair.Value;
        //string name = locationKeyPair.Key;
        //if (location.Footfall == 0d) continue;
        var salesVolume = location.SalesVolume;
        if (salesVolume > 0)
        {
            //var _9100 = Random.Shared.Next(0, 2) * Random.Shared.Next(0, 6);
            //var _3100 = Random.Shared.Next(0, 2) * Random.Shared.Next(0, 6);

            //if (_3100 + _9100 == 0) continue;
            //var _3100 = i++ % 5 == 0 ? 1 : 0;
            //var _9100 = salesVolume > 0 ? 1 : 0;
            var _3100 = salesVolume < 75 ?  1 : 0;
            var _9100 = salesVolume > 25 ? 1 : 0;

            if (_3100 + _9100 == 0) continue;

            solution.Locations[location.LocationName] = new PlacedLocations()
            {
                Freestyle3100Count = _3100, //_3100,
                Freestyle9100Count = _9100, //i % 3 == 0 ? 2 : 1, //_9100,
                Footfall = location.Footfall,
                SalesVolume = location.SalesVolume
            };
        }
    }

    //var json = System.Text.Json.JsonSerializer.Serialize(solution);
    //File.WriteAllText($@"c:\temp\considition\{mapName}.json", json);

    GameData score = new Scoring().CalculateScore(string.Empty, solution, mapData, generalData);
    if (scoreValue < score.GameScore.Total)
    {
        scoreValue = score.GameScore.Total;
        Console.WriteLine($"Map: {mapName}, GameScore: {score.GameScore.Total}");
    }

    //}
    //GameData prodScore = await api.SumbitAsync(mapName, solution, apikey);
    //Console.WriteLine($"GameId: {prodScore.Id}");
}

int a = 1;

