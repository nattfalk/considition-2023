using System.Diagnostics;
using Considition2023_Cs;

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
var normalSolver = new NormalMapSolver(generalData);
var sandboxSolver = new SandboxMapSolver(generalData);

var mapData = await api.GetMapDataAsync(mapName, apikey);
LocationsHelper.InitializeHelper(generalData, mapData);

Stopwatch sw = new Stopwatch();
sw.Start();
var locations = IsSandboxMap(mapData.MapName)
    ? sandboxSolver.Solve(mapData)
    : normalSolver.Solve(mapData);
sw.Stop();

SubmitSolution solution = new()
{
    Locations = LocationsHelper.GetUsedLocations(locations)
};
var score = new Scoring().CalculateScore(mapName, solution, mapData, generalData);
Console.SetCursorPosition(0, 9);
Console.WriteLine($"Optimized GameScore: {score.GameScore!.Total}");
Console.WriteLine($"Total execution time: {sw.Elapsed:mm\\:ss\\.fff}");
Console.WriteLine();

var prodScore = await api.SumbitAsync(mapName, solution, apikey);
Console.WriteLine($"GameId: {prodScore.Id}");
Console.WriteLine($"Submitted GameScore: {prodScore.GameScore!.Total}");
Console.WriteLine();

return;


bool IsSandboxMap(string mapName) =>
    (mapName == MapNames.GSandbox || mapName == MapNames.SSandbox);