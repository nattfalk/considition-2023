using System.Runtime.InteropServices;
using Considition2023_Cs.Optimizers;

namespace Considition2023_Cs;

internal class SandboxMapSolver
{
    private string[] _locationTypes;
    private readonly GeneralData _generalData;

    public SandboxMapSolver(GeneralData generalData) => _generalData = generalData;

    public Dictionary<string, PlacedLocations> Solve(MapData mapData)
    {
        GameData score = null;
        var initialScoreValue = 0d;
        Dictionary<string, PlacedLocations> bestLocations = new();

        var i = 2000;
        while (i-- >= 0)
        {
            CreateLocationTypeList(true);
            var locations = CreateSandboxMap(mapData);
            
            SubmitSolution solution = new()
            {
                Locations = LocationsHelper.GetUsedLocations(LocationsHelper.InitializeLocations(mapData.locations, locations))
            };

            if (solution.Locations.Count > 0)
                score = new Scoring().CalculateScore(mapData.MapName, solution, mapData, _generalData);

            if ((score?.GameScore?.Total ?? 0d) >= initialScoreValue)
            {
                initialScoreValue = score?.GameScore?.Total ?? 0d;
                bestLocations = LocationsHelper.CopyLocations(solution.Locations);
            }

            if (i % 20 == 0)
            {
                Console.SetCursorPosition(0, 7);
                Console.WriteLine($"** Creating initial sandboxmap. Iterations left: {i,4:0}");
            }
        }
        
        Console.SetCursorPosition(0, 7);
        Console.WriteLine($"Map: {mapData.MapName}, Initial GameScore: {initialScoreValue}");
        var scoreValue = score?.GameScore?.Total ?? 0d;

        var optimizeRunCount = 1;
        var optimizers = new HashSet<IOptimizer>()
        {
            new Optimizer10(_generalData, mapData, OptimizerSort.None),
            new Optimizer10(_generalData, mapData, OptimizerSort.Ascending),
            new Optimizer10(_generalData, mapData, OptimizerSort.Descending),
            new Optimizer11(_generalData, mapData, OptimizerSort.None),
            new Optimizer11(_generalData, mapData, OptimizerSort.Ascending),
            new Optimizer11(_generalData, mapData, OptimizerSort.Descending),
            //new Optimizer12(_generalData, mapData, OptimizerSort.None),
            //new Optimizer12(_generalData, mapData, OptimizerSort.Ascending),
            //new Optimizer12(_generalData, mapData, OptimizerSort.Descending),
            //new Optimizer13(_generalData, mapData, OptimizerSort.None),
            //new Optimizer13(_generalData, mapData, OptimizerSort.Ascending),
            //new Optimizer13(_generalData, mapData, OptimizerSort.Descending),
            
            //new Optimizer2(generalData, mapData),       // Linköping 699.57
            //new Optimizer3_sorted_dec(generalData, mapData),    // Göteborg, 6147.40, G-Sandbox, 2342.08
            //new Optimizer3_sorted(generalData, mapData),    // Uppsala, 2412.25, Västerås, 1498.38
            //new Optimizer1(generalData, mapData),
            //new Optimizer3(generalData, mapData),
            
            //new Optimizer3_2(generalData, mapData),
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
                var tempLocations = LocationsHelper.InitializeLocations(mapData.locations, bestLocations);
                var newScore = optimizer.Optimize(tempLocations, scoreValue, ref optimizeRunCount);
                if (newScore > previousScore)
                {
                    previousScore = newScore;
                    currentBestLocations = LocationsHelper.CopyLocations(tempLocations);
                }
                //var tempLocations = LocationsHelper.InitializeLocations(mapData.locations, bestLocations);
                //var newScore = optimizer.Optimize(tempLocations, scoreValue, ref optimizeRunCount);
                //if (newScore > previousScore)
                //{
                //    previousScore = newScore;
                //    scoreValue = newScore;
                //    currentBestLocations = LocationsHelper.CopyLocations(tempLocations);
                //    bestLocations = LocationsHelper.CopyLocations(tempLocations);
                //}

                Console.SetCursorPosition(0, 8);
                Console.WriteLine($"* Optimize step: {(optimizeRunCount-1),3:0}, New score: {previousScore,11:#.00}     ");
            }

            if (Math.Abs(previousScore - scoreValue) < 0.0000001d) break;
            bestLocations = currentBestLocations;
            scoreValue = previousScore;
        }
        while (true)
        {
            var previousScore = scoreValue;
            foreach (var optimizer in optimizers)
            {
                //var tempLocations = LocationsHelper.InitializeLocations(mapData.locations, locations);
                //var newScore = optimizer.Optimize(tempLocations, scoreValue, ref optimizeRunCount);
                //if (newScore > previousScore)
                //{
                //    previousScore = newScore;
                //    currentBestLocations = LocationsHelper.CopyLocations(tempLocations);
                //}
                var tempLocations = LocationsHelper.InitializeLocations(mapData.locations, bestLocations);
                var newScore = optimizer.Optimize(tempLocations, scoreValue, ref optimizeRunCount);
                if (newScore > previousScore)
                {
                    previousScore = newScore;
                    scoreValue = newScore;
                    currentBestLocations = LocationsHelper.CopyLocations(tempLocations);
                    bestLocations = LocationsHelper.CopyLocations(tempLocations);
                }

                Console.SetCursorPosition(0, 8);
                Console.WriteLine($"** Optimize step: {(optimizeRunCount - 1),3:0}, New score: {previousScore,11:#.00}   ");
            }

            if (Math.Abs(previousScore - scoreValue) < 0.0000001d) break;
            bestLocations = currentBestLocations;
            scoreValue = previousScore;
        }

        bestLocations = LocationsHelper.CopyLocations(currentBestLocations);
        while (true)
        {
            var previousScore = scoreValue;
            var optimizer = new Optimizer13(_generalData, mapData, OptimizerSort.Ascending);
            //foreach (var optimizer in optimizers)
            int ii = 10;
            while (ii-- >= 0)
            {
                var tempLocations = LocationsHelper.InitializeLocations(mapData.locations, bestLocations);
                var newScore = optimizer.Optimize(tempLocations, scoreValue, ref optimizeRunCount);
                if (newScore > previousScore)
                {
                    previousScore = newScore;
                    currentBestLocations = LocationsHelper.CopyLocations(tempLocations);
                }
                //var tempLocations = LocationsHelper.InitializeLocations(mapData.locations, bestLocations);
                //var newScore = optimizer.Optimize(tempLocations, scoreValue, ref optimizeRunCount);
                //if (newScore > previousScore)
                //{
                //    previousScore = newScore;
                //    scoreValue = newScore;
                //    currentBestLocations = LocationsHelper.CopyLocations(tempLocations);
                //    bestLocations = LocationsHelper.CopyLocations(tempLocations);
                //}

                Console.SetCursorPosition(0, 8);
                Console.WriteLine($"*** Optimize step: {(optimizeRunCount - 1),3:0}, New score: {previousScore,11:#.00}      ");
            }

            if (Math.Abs(previousScore - scoreValue) < 0.0000001d) break;
            bestLocations = currentBestLocations;
            scoreValue = previousScore;
        }

        return currentBestLocations;
    }

    public void CreateLocationTypeList(bool randomize)
    {
        _locationTypes = new[] {
            "Grocery-store-large",
            "Grocery-store-large",
            "Grocery-store-large",
            "Grocery-store-large",
            "Grocery-store-large",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Grocery-store",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Convenience",
            "Gas-station",
            "Gas-station",
            "Gas-station",
            "Gas-station",
            "Gas-station",
            "Gas-station",
            "Gas-station",
            "Gas-station",
            "Kiosk",
            "Kiosk",
            "Kiosk",
        };

        if (randomize)
            new Random().Shuffle(_locationTypes);
    }

    public Dictionary<string, PlacedLocations> CreateSandboxMap(MapData mapData)
    {
        var locations = new Dictionary<string, PlacedLocations>();

        //const int maxGroceryStoreLarge = 5;
        //const int maxGroceryStore = 20;
        //const int maxConvenience = 20;
        //const int maxGasStation = 8;5
        //const int maxKiosk = 3;5

        var hotspots = mapData.Hotspots
            .Where(h =>
                h.Longitude >= mapData.Border.LongitudeMin && h.Longitude <= mapData.Border.LongitudeMax
                && h.Latitude >= mapData.Border.LatitudeMin && h.Latitude <= mapData.Border.LatitudeMax)
            .OrderBy(h => h.Spread / h.Footfall) //h.Footfall * h.Spread)
            .Where((x, i) => i < 2 || (i % 2 == 0))
            .ToList();

        for (var i = 0; i < _locationTypes.Length; i++)
        {
            var locationType = GetLocationType(i + 1);
            locations.Add($"location{i + 1}", new PlacedLocations
            {
                Longitude = hotspots[i].Longitude,
                Latitude = hotspots[i].Latitude,
                Footfall = hotspots[i].Footfall,
                Spread = hotspots[i].Spread,
                LocationType = locationType,
                Freestyle3100Count = Get3100Count(locationType, i),
                Freestyle9100Count = Get9100Count(locationType, i),
            });
        }

        return locations;

        int Get3100Count(string locationType, int i)
        {
            return locationType switch
            {
                //"Grocery-store-large" => 2, //i % 3 == 0 ? 1 : 0,
                //"Grocery-store" => 2, //i % 20 == 0 ? 1 : 0,
                //"Convenience" => 2,
                //"Gas-station" => 2,
                //"Kiosk" => 2,
                _ => 1
            };
        }

        int Get9100Count(string locationType, int i)
        {
            return locationType switch
            {
                //"Grocery-store-large" => 2, //i % 3 == 0 ? 2 : 1,
                //"Grocery-store" => 2, //i % 20 == 0 ? 2 : 1,
                //"Convenience" => 2,
                //"Gas-station" => 2,
                //"Kiosk" => 2,
                _ => 1
            };
        }

        string GetLocationType(int locationCounter)
        {
            //return (locationCounter % 5) switch
            //{
            //    1 => "Grocery-store-large",
            //    2 => "Grocery-store",
            //    3 => "Convenience",
            //    4 => "Gas-station",
            //    _ => "Kiosk"
            //};

            //return locationCounter switch
            //{
            //    >= 1 and <= 5 => "Grocery-store-large",
            //    <= 25 => "Grocery-store",
            //    <= 45 => "Convenience",
            //    <= 53 => "Gas-station",
            //    _ => "Kiosk"
            //};

            return _locationTypes[locationCounter - 1];
        }
    }
}